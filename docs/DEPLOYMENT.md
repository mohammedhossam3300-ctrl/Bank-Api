# Deployment Guide

## Database Migration Strategy

### Pre-Deployment Checklist
- [ ] Database migrations tested in staging environment
- [ ] Backups created before deployment
- [ ] Connection strings updated for target environment
- [ ] Firewall rules / allowlist configured for PostgreSQL host
- [ ] Database user permissions verified
- [ ] Rollback plan documented

### Applying Migrations in Production

```bash
# Manual migration application
dotnet ef database update --project Bank.Infrastructure --startup-project Bank.Api --configuration Release

# Or via application startup (recommended)
# Migrations apply automatically when the app starts
dotnet run --project Bank.Api --configuration Release
```

### Rolling Back Migrations
```bash
# Rollback to previous migration
dotnet ef database update <PreviousMigrationName> --project Bank.Infrastructure --startup-project Bank.Api
```

## Docker Deployment

### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Bank.Api/Bank.Api.csproj", "Bank.Api/"]
COPY ["src/Bank.Application/Bank.Application.csproj", "Bank.Application/"]
COPY ["src/Bank.Domain/Bank.Domain.csproj", "Bank.Domain/"]
COPY ["src/Bank.Infrastructure/Bank.Infrastructure.csproj", "Bank.Infrastructure/"]

RUN dotnet restore "Bank.Api/Bank.Api.csproj"
COPY . .
WORKDIR "/src/Bank.Api"
RUN dotnet build "Bank.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bank.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bank.Api.dll"]
```

### Docker Compose with PostgreSQL

#### For Local Development
```yaml
version: '3.8'

services:
  bank-api:
    build: .
    ports:
      - "8080:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - DATABASE_URL=postgresql://postgres:YourPassword123@bank-db:5432/bankdb
    depends_on:
      - bank-db
    networks:
      - bank-network

  bank-db:
    image: postgres:16
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=YourPassword123
      - POSTGRES_DB=bankdb
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data
    networks:
      - bank-network

volumes:
  pgdata:

networks:
  bank-network:
    driver: bridge
```

#### For Production
```yaml
version: '3.8'

services:
  bank-api:
    build:
      context: .
      dockerfile: devops/docker/Dockerfile.backend
    restart: always
    ports:
      - "443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - DATABASE_URL=postgresql://user:password@your-pg-host:5432/bankdb?sslmode=require
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
      - ASPNETCORE_Kestrel__Certificates__Default__Password=${CERTIFICATE_PASSWORD}
    volumes:
      - /etc/letsencrypt/live/yourdomain.com:/https:ro
    deploy:
      replicas: 3
      restart_policy:
        condition: on-failure
      resources:
        limits:
          cpus: '1'
          memory: 1G
        reservations:
          cpus: '0.5'
          memory: 512M
```

## Azure Deployment

### Prerequisites
- Azure subscription
- Azure CLI installed and authenticated
- .NET 9.0 SDK
- Azure App Service Plan

### Step 1: Create Azure Database for PostgreSQL

```bash
# Set variables
$resourceGroup="myResourceGroup"
$pgServer="mybankpgserver"
$pgAdmin="pgadmin"
$pgPassword="YourPassword123!@"
$location="eastus"

# Create resource group
az group create --name $resourceGroup --location $location

# Create Azure Database for PostgreSQL Flexible Server
az postgres flexible-server create `
  --resource-group $resourceGroup `
  --name $pgServer `
  --location $location `
  --admin-user $pgAdmin `
  --admin-password $pgPassword `
  --sku-name Standard_B1ms `
  --tier Burstable `
  --storage-size 32

# Allow Azure services
az postgres flexible-server firewall-rule create `
  --resource-group $resourceGroup `
  --name $pgServer `
  --rule-name "AllowAzureServices" `
  --start-ip-address 0.0.0.0 `
  --end-ip-address 0.0.0.0

# Create database
az postgres flexible-server db create `
  --resource-group $resourceGroup `
  --server-name $pgServer `
  --database-name bankdb
```

### Step 2: Create App Service Plan and Web App

```bash
# Create App Service Plan
az appservice plan create `
  --name myBankAppPlan `
  --resource-group $resourceGroup `
  --sku B2 `
  --is-linux

# Create Web App
az webapp create `
  --resource-group $resourceGroup `
  --plan myBankAppPlan `
  --name myBankApp `
  --runtime "DOTNET|9.0"
```

### Step 3: Configure Connection String

```bash
# Set DATABASE_URL environment variable
az webapp config appsettings set `
  --resource-group $resourceGroup `
  --name myBankApp `
  --settings "DATABASE_URL=postgresql://$pgAdmin:$pgPassword@$pgServer.postgres.database.azure.com:5432/bankdb?sslmode=require"
```

### Step 4: Deploy Application

```bash
# Publish application
dotnet publish -c Release -o ./publish

# Create deployment package
cd publish
Compress-Archive -Path * -DestinationPath ../deploy.zip

# Deploy to App Service
az webapp deployment source config-zip `
  --resource-group $resourceGroup `
  --name myBankApp `
  --src ../deploy.zip
```

### Step 5: Configure Application Settings

```bash
# Set environment to Production
az webapp config appsettings set `
  --resource-group $resourceGroup `
  --name myBankApp `
  --settings ASPNETCORE_ENVIRONMENT=Production

# Set JWT secret (use Key Vault for production)
az webapp config appsettings set `
  --resource-group $resourceGroup `
  --name myBankApp `
  --settings "JwtSettings__SecretKey=your-super-secret-jwt-key"
```

### Step 6: Verify Deployment

```bash
# Check app status
az webapp show --resource-group $resourceGroup --name myBankApp --query state

# View application logs
az webapp log tail --resource-group $resourceGroup --name myBankApp
```

## Environment Configuration

### Development Settings (appsettings.Development.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtSettings": {
    "SecretKey": "dev-secret-key-change-in-production",
    "Issuer": "BankAPI",
    "Audience": "BankClients",
    "ExpirationMinutes": 60
  },
  "AllowedHosts": "*"
}
```

Set `DATABASE_URL` as an environment variable (not in appsettings) to avoid committing credentials:
```bash
export DATABASE_URL="postgresql://postgres:password@localhost:5432/bankdb"
```

### Production Settings (appsettings.Production.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtSettings": {
    "SecretKey": "#{JwtSecretKey}#",
    "Issuer": "BankAPI",
    "Audience": "BankClients",
    "ExpirationMinutes": 60
  },
  "AllowedHosts": "yourdomain.com"
}
```

### Environment Variables (PowerShell)

```powershell
# Database (PostgreSQL URI)
$env:DATABASE_URL="postgresql://user:password@your-pg-host:5432/bankdb?sslmode=require"

# JWT
$env:JwtSettings__SecretKey="your-super-secret-key"

# Logging
$env:ASPNETCORE_ENVIRONMENT="Production"

# Application URL
$env:ASPNETCORE_URLS="https://0.0.0.0:443"
```

### Environment Variables (Bash/Linux)

```bash
# Database (PostgreSQL URI)
export DATABASE_URL="postgresql://user:password@your-pg-host:5432/bankdb?sslmode=require"

# JWT
export JwtSettings__SecretKey="your-super-secret-key"

# Logging
export ASPNETCORE_ENVIRONMENT="Production"

# Application URL
export ASPNETCORE_URLS="https://0.0.0.0:443"
```

## CI/CD Pipeline

### GitHub Actions
```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 9.0.x
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore
      
    - name: Test
      run: dotnet test --no-build --verbosity normal
      
    - name: Publish
      run: dotnet publish -c Release -o ./publish
      
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'myBankApp'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: './publish'
```

## Monitoring and Logging

### Application Insights
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Health Checks
```csharp
builder.Services.AddHealthChecks()
    .AddNpgsql(connectionString)
    .AddCheck("self", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health");
```

## Security Considerations

- Use HTTPS in production
- Implement proper authentication
- Configure CORS appropriately
- Use secure connection strings
- Enable request logging
- Implement rate limiting
- Regular security updates