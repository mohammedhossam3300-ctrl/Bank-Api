# 🏦 FinBank API - Enterprise Banking System

> A comprehensive, production-ready banking API built with **.NET 9.0**, **Clean Architecture**, and **CQRS Pattern**

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Mostafa-SAID7_Bank-Api&metric=alert_status&token=64cd308bb15671223d91856900d4d1e23843ad91)](https://sonarcloud.io/summary/new_code?id=Mostafa-SAID7_Bank-Api)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Mostafa-SAID7_Bank-Api&metric=bugs&token=64cd308bb15671223d91856900d4d1e23843ad91)](https://sonarcloud.io/summary/new_code?id=Mostafa-SAID7_Bank-Api)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Mostafa-SAID7_Bank-Api&metric=code_smells&token=64cd308bb15671223d91856900d4d1e23843ad91)](https://sonarcloud.io/summary/new_code?id=Mostafa-SAID7_Bank-Api)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Mostafa-SAID7_Bank-Api&metric=coverage&token=64cd308bb15671223d91856900d4d1e23843ad91)](https://sonarcloud.io/summary/new_code?id=Mostafa-SAID7_Bank-Api)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Mostafa-SAID7_Bank-Api&metric=security_rating&token=64cd308bb15671223d91856900d4d1e23843ad91)](https://sonarcloud.io/summary/new_code?id=Mostafa-SAID7_Bank-Api)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Mostafa-SAID7_Bank-Api&metric=vulnerabilities&token=64cd308bb15671223d91856900d4d1e23843ad91)](https://sonarcloud.io/summary/new_code?id=Mostafa-SAID7_Bank-Api)

## ✨ Key Features

- **💳 Account Management**: Create, manage, and monitor bank accounts with real-time balance tracking
- **💰 Transaction Processing**: Secure fund transfers with comprehensive transaction history and audit trails
- **🔐 Advanced Security**: JWT authentication, 2FA, IP whitelisting, and password policies
- **👥 User Management**: Role-based access control (Admin, Manager, User, Auditor)
- **📊 Comprehensive Banking**: Cards, Loans, Deposits, Bill Payments, Recurring Payments
- **📋 Audit & Compliance**: Complete audit logging and regulatory compliance
- **⚡ High Performance**: Optimized database with 50+ indexes and soft delete support
- **🚀 Production Ready**: Deployed on Azure SQL Server with CI/CD pipelines

## Database

This project uses **SQL Server** as the primary database with Azure SQL Server hosting in production.

### Current Database Configuration
- **Server**: `db48070.public.databaseasp.net`
- **Database**: `db48070`
- **Tables**: 17 core tables + Identity tables
- **Features**: 
  - 50+ optimized indexes
  - 30+ foreign key relationships
  - Soft delete support on all business entities
  - Complete audit trail (CreatedAt, UpdatedAt)
  - Role-based access control via ASP.NET Core Identity

### Database Schema (Phase 1)
- **Accounts Module**: Accounts, AccountFees, AccountHolds, AccountRestrictions, AccountStatusHistories, FeeSchedules, JointAccountHolders
- **Transaction Module**: Transactions
- **Authentication Module**: Sessions, TwoFactorTokens, AccountLockouts, PasswordPolicies, PasswordHistories, IpWhitelists
- **Identity Tables**: AspNetRoles, AspNetUsers, AspNetUserRoles, AspNetUserClaims, AspNetUserLogins, AspNetUserTokens, AspNetRoleClaims

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- SQL Server 2019+ or Azure SQL Database
- Visual Studio 2022 or VS Code
- Git

### Installation
```bash
# Clone the repository
git clone https://github.com/yourusername/bank-management-system.git
cd bank-management-system

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API (migrations will apply automatically on startup)
dotnet run --project src/Bank.Api/Bank.Api.csproj
```

The API will be available at `https://localhost:7000`

### Verify Installation
Visit `https://localhost:7000/swagger` to explore the API using Swagger UI.

### Database Configuration
Update connection string in `src/Bank.Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=your-database;User Id=your-user;Password=your-password;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
  }
}
```

## Documentation

- [📋 Features](docs/FEATURES.md)
- [🏗️ Project Structure](docs/STRUCTURE.md)
- [⚙️ Setup Guide](docs/PROJECT_SETUP.md)
- [🚀 Deployment](docs/DEPLOYMENT.md)
- [🛠️ Technologies](docs/TECHNOLOGIES.md)
- [📖 Use Cases](docs/USE_CASES.md)
- [🤝 Contributing](docs/CONTRIBUTING.md)

## Architecture

This project follows **Clean Architecture** with **CQRS (Command Query Responsibility Segregation)** pattern:

### Project Structure
- **Bank.Api**: API layer with controllers, middleware, and configuration
- **Bank.Application**: Application/business logic layer with commands, queries, and handlers
- **Bank.Domain**: Domain layer with core entities, interfaces, and business rules
- **Bank.Infrastructure**: Infrastructure layer with data access, Entity Framework, and external services

### Key Design Patterns
- **Clean Architecture**: Clear separation of concerns with minimal dependencies
- **CQRS Pattern**: Separate handling of commands (write operations) and queries (read operations)
- **Repository Pattern**: Abstraction over data access logic
- **Dependency Injection**: Loose coupling through IoC container
- **Soft Delete**: Logical deletion with query filters
- **Audit Trail**: Complete tracking of entity changes

## Contributing

We welcome contributions! Please read our [Contributing Guide](docs/CONTRIBUTING.md) and [Code of Conduct](docs/CODE_OF_CONDUCT.md).

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

- 📧 Email: support@bankproject.com
- 🐛 Issues: [GitHub Issues](https://github.com/yourusername/bank-management-system/issues)
- 📖 Documentation: [Project Wiki](https://github.com/yourusername/bank-management-system/wiki)