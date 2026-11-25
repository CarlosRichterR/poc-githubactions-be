using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        HttpLoggingFields.RequestMethod |
        HttpLoggingFields.RequestPath |
        HttpLoggingFields.RequestQuery |
        HttpLoggingFields.RequestBody |
        HttpLoggingFields.ResponseStatusCode |
        HttpLoggingFields.Duration;
    options.RequestBodyLogLimit = 4096;
});

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5048", "http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseHttpLogging();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Frontend");

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "poc-githubactions-be",
    utc = DateTime.UtcNow
}))
.WithName("Health")
.WithOpenApi();

app.MapGet("/api/info", (IConfiguration config, IHostEnvironment env) => Results.Ok(new
{
    name = "POC GitHub Actions + Terraform",
    environment = env.EnvironmentName,
    version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0",
    message = config["App:WelcomeMessage"] ?? "Backend listo"
}))
.WithName("Info")
.WithOpenApi();

var messages = new List<MessageDto>
{
    new(1, "Hello from the backend", DateTime.UtcNow.AddMinutes(-5)),
    new(2, "This POC deploys FE + BE with GitHub Actions and Terraform", DateTime.UtcNow.AddMinutes(-1))
};

app.MapGet("/api/messages", () => Results.Ok(messages))
.WithName("GetMessages")
.WithOpenApi();

app.MapPost("/api/messages", (CreateMessageRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Text))
        return Results.BadRequest(new { error = "Text is required" });

    var item = new MessageDto(messages.Count + 1, request.Text.Trim(), DateTime.UtcNow);
    messages.Add(item);
    return Results.Created($"/api/messages/{item.Id}", item);
})
.WithName("CreateMessage")
.WithOpenApi();

app.Run();

record MessageDto(int Id, string Text, DateTime CreatedUtc);
record CreateMessageRequest(string Text);

public partial class Program;
