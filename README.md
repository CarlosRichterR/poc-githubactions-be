# poc-githubactions-be

API .NET 8 mínima para el POC de **GitHub Actions + Terraform + Azure** (Pay-as-you-go, SKUs baratos).

## Qué incluye

| Endpoint | Descripción |
|----------|-------------|
| `GET /health` | Health check (útil para App Service / Actions) |
| `GET /api/info` | Info del entorno |
| `GET /api/messages` | Lista de mensajes en memoria |
| `POST /api/messages` | Crea un mensaje `{ "text": "..." }` |
| `/swagger` | OpenAPI UI |

CORS configurable en `appsettings.json` → `Cors:AllowedOrigins`.

## Correr en local

```bash
dotnet run --project src/Poc.Api
```

- API: http://localhost:5080  
- Swagger: http://localhost:5080/swagger  

## Próximo (aún no en este repo)

1. Carpeta `infra/` con Terraform (Resource Group, App Service F1/B1, Storage, Key Vault).
2. Workflow `.github/workflows/deploy.yml` (build → publish → deploy).
3. Conectar el frontend `poc-githubactions-fe` vía CORS con la URL del App Service.
