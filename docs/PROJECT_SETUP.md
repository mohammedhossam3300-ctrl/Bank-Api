# Project Setup Guide

## Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 (recommended) or VS Code
- SQL Server 2019+ or SQL Server Express
- SQL Server Management Studio (optional but recommended)
- Git

## Installation Steps

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/bank-management-system.git
cd bank-management-system
```

### 2. Database Configuration

#### Option A: Local SQL Server (Development)
1. Install SQL Server 2019 Express or higher
2. Create a new database named `BankDB`
3. Update connection string in `src/Bank.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BankDB;Trusted_Connection=true;MultipleActiveResultSets=True;Encrypt=False;"
  }
}
```

#### Option B: SQL Server with Authentication (Development)
If using SQL Server with specific user credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BankDB;User Id=sa;Password=YourPassword123;MultipleActiveResultSets=True;Encrypt=False;"
  }
}
```

#### Option C: Azure SQL Database (Production)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server.database.windows.net;Database=BankDB;User Id=admin-user;Password=YourPassword123;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;"
  }
}
```

### 3. Build the Solution

```bash
cd src
dotnet restore
dotnet build
```

### 4. Run Database Migrations

Migrations run automatically on application startup. To manually apply migrations:

```bash
# From the src directory
dotnet ef database update --project Bank.Infrastructure --startup-project Bank.Api
```

### 5. Run the Application

```bash
cd Bank.Api
dotnet run
```

The API will be available at `https://localhost:7000`

### 6. Verify Installation

1. Open `https://localhost:7000/swagger` in your browser
2. Verify the API endpoints are accessible
3. Check database connection by viewing the database in SQL Server Management Studio

## Development Environment

### Recommended Tools

- **IDE**: Visual Studio 2022 Professional or Community Edition
- **Database**: SQL Server Management Studio 19+
- **API Testing**: Swagger UI (built-in), Postman, or Thunder Client
- **Version Control**: Git and GitHub Desktop (optional)
- **Code Editor**: Visual Studio Code with C# extensions (alternative to VS 2022)

### IDE Extensions (Visual Studio 2022)
- SQL Server Object Explorer (for database management)
- NuGet Package Manager
- GitHub Copilot (optional)

### IDE Extensions (VS Code)
- C# Dev Kit
- REST Client
- SQL Server mssql extension
- GitHub Copilot (optional)

### Environment Variables

Create a `.env` file in the `src/Bank.Api` directory:

```
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Server=localhost;Database=BankDB;Trusted_Connection=true;MultipleActiveResultSets=True;Encrypt=False;
JWT_SECRET=your-super-secret-jwt-key-change-in-production
API_PORT=7000
```

Or set via system environment variables:
```bash
# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=BankDB;..."
$env:JWT_SECRET="your-secret-key"

# Linux/macOS (Bash)
export ConnectionStrings__DefaultConnection="Server=localhost;Database=BankDB;..."
export JWT_SECRET="your-secret-key"
```

## Testing

### Run All Tests

```bash
dotnet test
```

### Run Unit Tests Only

```bash
dotnet test --filter Category=Unit
```

### Run Integration Tests

```bash
dotnet test --filter Category=Integration
```

### Run with Coverage Report

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Run Specific Test Project

```bash
dotnet test Bank.Tests/Bank.Tests.csproj
```

## Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Verify SQL Server is running: `select @@version;` in SQL Server Management Studio
   - Check connection string format in `appsettings.Development.json`
   - Verify database exists: `SELECT name FROM sys.databases WHERE name='BankDB';`
   - For Azure SQL: Verify firewall rules allow your IP address
   - Connection string must include `Encrypt=True;TrustServerCertificate=False;` for Azure

2. **Build Errors**
   - Run `dotnet clean` then `dotnet restore`
   - Check .NET SDK version: `dotnet --version` (should be 9.0+)
   - Rebuild solution in Visual Studio

3. **Port Already in Use**
   - Change port in `src/Bank.Api/Properties/launchSettings.json`
   - Or kill existing process: `netstat -ano | findstr :7000` (Windows)
   - Then: `taskkill /PID <process-id> /F`

4. **Migration Errors**
   - Ensure SQL Server is accessible
   - Check connection string is correct
   - Verify database user has CREATE TABLE permissions
   - Run migrations manually: `dotnet ef database update`

5. **Certificate Errors (Azure SQL)**
   - Use `TrustServerCertificate=False;` in connection string
   - Ensure `Encrypt=True;` is set
   - Verify firewall rules allow your client IP

### Getting Help

- Check the [GitHub Issues](https://github.com/yourusername/bank-management-system/issues) page
- Review the [Contributing Guide](CONTRIBUTING.md)
- Check [DEPLOYMENT.md](DEPLOYMENT.md) for production setup issues
- Contact the development team via email or Discord