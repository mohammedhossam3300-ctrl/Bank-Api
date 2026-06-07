# Project Setup Guide

## Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 (recommended) or VS Code
- PostgreSQL 14+ (or any hosted PostgreSQL such as Neon, Supabase, or Railway)
- Git

## Installation Steps

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/bank-management-system.git
cd bank-management-system
```

### 2. Database Configuration

Set the `DATABASE_URL` environment variable to your PostgreSQL connection string (standard URI format):

#### Option A: Local PostgreSQL (Development)
```bash
export DATABASE_URL="postgresql://postgres:password@localhost:5432/bankdb"
```

#### Option B: Neon (Production / Hosted)
```bash
export DATABASE_URL="postgresql://user:password@host.neon.tech/neondb?sslmode=require"
```

#### Option C: Any hosted PostgreSQL
```bash
export DATABASE_URL="postgresql://user:password@host:5432/dbname?sslmode=require"
```

The app reads `DATABASE_URL` (or `NEON_DATABASE_URL` override) at startup and automatically converts it to the correct Npgsql connection string.

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

The API will be available at `http://localhost:5000`

### 6. Verify Installation

1. Open `http://localhost:5000/swagger` in your browser
2. Verify the API endpoints are accessible
3. Check database tables were created (use psql or any PostgreSQL client)

## Development Environment

### Recommended Tools

- **IDE**: Visual Studio 2022 Professional or Community Edition
- **Database**: DBeaver, pgAdmin, or TablePlus (for PostgreSQL management)
- **API Testing**: Swagger UI (built-in), Postman, or Thunder Client
- **Version Control**: Git and GitHub Desktop (optional)
- **Code Editor**: Visual Studio Code with C# extensions (alternative to VS 2022)

### IDE Extensions (Visual Studio 2022)
- NuGet Package Manager
- GitHub Copilot (optional)
- REST Client (API testing)
- GitLens (Git visualization)

### VS Code Extensions
- C# Dev Kit
- REST Client
- PostgreSQL (by Chris Kolkman)
- GitLens

## Configuration

### Environment Variables

| Variable | Description | Required |
|---|---|---|
| `DATABASE_URL` | PostgreSQL connection URI | Yes |
| `NEON_DATABASE_URL` | Neon PostgreSQL URI (overrides DATABASE_URL) | No |
| `JWT_KEY` | Secret key for JWT token signing | Yes |
| `EMAIL_SMTP_HOST` | SMTP server hostname | No |
| `EMAIL_SMTP_PORT` | SMTP server port | No |
| `EMAIL_USERNAME` | SMTP username | No |
| `EMAIL_PASSWORD` | SMTP password | No |

### Application Settings

Key settings in `appsettings.json`:

- `DatabaseSettings:SkipMigrations` — skip automatic migrations on startup (default: false)
- `DatabaseSettings:SkipSeeding` — skip data seeding on startup (default: false)
- `Jwt:Issuer` / `Jwt:Audience` — JWT issuer and audience claims
- `Cors:AllowedOrigins` — allowed CORS origins

## Troubleshooting

### Common Issues

1. **Database connection fails**
   - Verify `DATABASE_URL` is set correctly
   - Test connection: `psql "$DATABASE_URL"`
   - Check SSL mode — hosted databases typically require `?sslmode=require`

2. **Migrations fail**
   - Ensure the database user has CREATE TABLE permissions
   - Check the database exists and is accessible
   - Set `DatabaseSettings:SkipMigrations=true` in appsettings to bypass

3. **JWT authentication issues**
   - Verify `JWT_KEY` environment variable is set and is at least 32 characters

4. **CORS errors**
   - Update `Cors:AllowedOrigins` in `appsettings.json` to include your frontend URL

5. **Port conflicts**
   - The API uses port 5000 by default
   - Change via `ASPNETCORE_URLS=http://+:8080` environment variable
