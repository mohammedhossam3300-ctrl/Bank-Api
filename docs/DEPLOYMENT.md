# Deployment Guide

## Database Migration Strategy

### Pre-Deployment Checklist
- [ ] Database migrations tested in staging environment
- [ ] Backups created before deployment
- [ ] Connection strings updated for target environment
- [ ] Firewall rules configured (for Azure SQL)
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

### Docker Compose with SQL Server

#### For Local Development
```yaml
version: '3.8'

services:
  bank-api:
    build: .
    ports:
      - "8080:80"
      - "8443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=bank-db;Database=BankDB;User=sa;Password=YourPassword123;MultipleActiveResultSets=True;Encrypt=False;
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
      - ASPNETCORE_Kestrel__Certificates__Default__Password=
    volumes:
      - ~/.aspnet/https:/https:ro
    depends_on:
      - bank-db
    networks:
      - bank-network

  bank-db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123
      - MSSQL_SA_PASSWORD=YourPassword123
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - bank-network

volumes:
  sqldata:

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
      - ConnectionStrings__DefaultConnection=Server=your-sql-server.database.windows.net;Database=BankDB;User Id=admin;Password=YourPassword123;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;
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

### Step 1: Create Azure SQL Server and Database

```bash
# Set variables
$resourceGroup="myResourceGroup"
$sqlServer="mybanksqlserver"
$sqlAdmin="sqladmin"
$sqlPassword="YourPassword123!@"
$location="eastus"

# Create resource group
az group create --name $resourceGroup --location $location

# Create SQL Server
az sql server create `
  --resource-group $resourceGroup `
  --name $sqlServer `
  --location $location `
  --admin-user $sqlAdmin `
  --admin-password $sqlPassword

# Configure firewall to allow Azure services
az sql server firewall-rule create `
  --resource-group $resourceGroup `
  --server $sqlServer `
  --name "AllowAzureServices" `
  --start-ip-address 0.0.0.0 `
  --end-ip-address 0.0.0.0

# Allow your client IP
az sql server firewall-rule create `
  --resource-group $resourceGroup `
  --server $sqlServer `
  --name "ClientIP" `
  --start-ip-address <YOUR_IP> `
  --end-ip-address <YOUR_IP>

# Create database
az sql db create `
  --resource-group $resourceGroup `
  --server $sqlServer `
  --name BankDB `
  --service-objective Basic
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
# Set connection string
az webapp config connection-string set `
  --resource-group $resourceGroup `
  --name myBankApp `
  --settings DefaultConnection="Server=tcp:$sqlServer.database.windows.net,1433;Initial Catalog=BankDB;Persist Security Info=False;User ID=$sqlAdmin;Password=$sqlPassword;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;TrustServerCertificate=False;" `
  --connection-string-type SQLAzure
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
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BankDB;Trusted_Connection=true;MultipleActiveResultSets=True;Encrypt=False;"
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

### Production Settings (appsettings.Production.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server.database.windows.net;Database=BankDB;User Id=admin;Password=YourPassword123;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;Connection Timeout=30;"
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
# Database
$env:ConnectionStrings__DefaultConnection="Server=your-server.database.windows.net;Database=BankDB;User Id=admin;Password=YourPassword123;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;"

# JWT
$env:JwtSettings__SecretKey="your-super-secret-key"

# Logging
$env:ASPNETCORE_ENVIRONMENT="Production"

# Application URL
$env:ASPNETCORE_URLS="https://0.0.0.0:443"
```

### Environment Variables (Bash/Linux)

```bash
# Database
export ConnectionStrings__DefaultConnection="Server=your-server.database.windows.net;Database=BankDB;User Id=admin;Password=YourPassword123;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;"

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
    .AddSqlServer(connectionString)
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