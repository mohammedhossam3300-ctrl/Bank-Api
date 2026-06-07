using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Account tables
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey("FK_Accounts_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
                    table.UniqueConstraint("AK_Accounts_AccountNumber", x => x.AccountNumber);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    FromAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey("FK_Transactions_Accounts_FromAccountId", x => x.FromAccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Transactions_Accounts_ToAccountId", x => x.ToAccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                });

            // Authentication/Session tables
            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionToken = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey("FK_Sessions_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint("AK_Sessions_SessionToken", x => x.SessionToken);
                });

            migrationBuilder.CreateTable(
                name: "TwoFactorTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorTokens", x => x.Id);
                    table.ForeignKey("FK_TwoFactorTokens_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountLockouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsCurrentlyLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockedUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLockouts", x => x.Id);
                    table.ForeignKey("FK_AccountLockouts_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_AccountLockouts_AspNetUsers_LockedByUserId", x => x.LockedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.SetNull);
                    table.UniqueConstraint("AK_AccountLockouts_UserId", x => x.UserId);
                });

            // Password related tables
            migrationBuilder.CreateTable(
                name: "PasswordPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ComplexityLevel = table.Column<int>(type: "int", nullable: false),
                    AllowedSpecialCharacters = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordPolicies", x => x.Id);
                    table.UniqueConstraint("AK_PasswordPolicies_Name", x => x.Name);
                    table.UniqueConstraint("AK_PasswordPolicies_ComplexityLevel", x => x.ComplexityLevel);
                });

            migrationBuilder.CreateTable(
                name: "PasswordHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    PasswordSetAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCurrentPassword = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordHistories", x => x.Id);
                    table.ForeignKey("FK_PasswordHistories_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IpWhitelists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", nullable: false),
                    IpRange = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpWhitelists", x => x.Id);
                    table.ForeignKey("FK_IpWhitelists_AspNetUsers_CreatedByUserId", x => x.CreatedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_IpWhitelists_AspNetUsers_ApprovedByUserId", x => x.ApprovedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.SetNull);
                });

            // Account detail tables
            migrationBuilder.CreateTable(
                name: "AccountFees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountFees", x => x.Id);
                    table.ForeignKey("FK_AccountFees_Accounts_AccountId", x => x.AccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_AccountFees_Transactions_TransactionId", x => x.TransactionId, "Transactions", "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AccountHolds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlacedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReleasedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHolds", x => x.Id);
                    table.ForeignKey("FK_AccountHolds_Accounts_AccountId", x => x.AccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_AccountHolds_AspNetUsers_PlacedByUserId", x => x.PlacedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_AccountHolds_AspNetUsers_ReleasedByUserId", x => x.ReleasedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AccountRestrictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppliedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RemovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRestrictions", x => x.Id);
                    table.ForeignKey("FK_AccountRestrictions_Accounts_AccountId", x => x.AccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_AccountRestrictions_AspNetUsers_AppliedByUserId", x => x.AppliedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_AccountRestrictions_AspNetUsers_RemovedByUserId", x => x.RemovedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AccountStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatusHistories", x => x.Id);
                    table.ForeignKey("FK_AccountStatusHistories_Accounts_AccountId", x => x.AccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_AccountStatusHistories_AspNetUsers_ChangedByUserId", x => x.ChangedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeeSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumBalanceThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumBalanceThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WaiverMinimumBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeSchedules", x => x.Id);
                    table.ForeignKey("FK_FeeSchedules_AspNetUsers_CreatedByUserId", x => x.CreatedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JointAccountHolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RemovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JointAccountHolders", x => x.Id);
                    table.ForeignKey("FK_JointAccountHolders_Accounts_AccountId", x => x.AccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_JointAccountHolders_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_JointAccountHolders_AspNetUsers_AddedByUserId", x => x.AddedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_JointAccountHolders_AspNetUsers_RemovedByUserId", x => x.RemovedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.SetNull);
                });

            // Create indexes
            migrationBuilder.CreateIndex(name: "IX_Accounts_AccountNumber", table: "Accounts", column: "AccountNumber", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Accounts_UserId", table: "Accounts", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_Accounts_IsDeleted", table: "Accounts", column: "IsDeleted", filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(name: "IX_Transactions_FromAccountId", table: "Transactions", column: "FromAccountId");
            migrationBuilder.CreateIndex(name: "IX_Transactions_ToAccountId", table: "Transactions", column: "ToAccountId");
            migrationBuilder.CreateIndex(name: "IX_Transactions_CreatedAt", table: "Transactions", column: "CreatedAt");
            migrationBuilder.CreateIndex(name: "IX_Transactions_IsDeleted", table: "Transactions", column: "IsDeleted", filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(name: "IX_Sessions_SessionToken", table: "Sessions", column: "SessionToken", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Sessions_RefreshToken", table: "Sessions", column: "RefreshToken");
            migrationBuilder.CreateIndex(name: "IX_Sessions_UserId_Status", table: "Sessions", columns: new[] { "UserId", "Status" });
            migrationBuilder.CreateIndex(name: "IX_Sessions_ExpiresAt", table: "Sessions", column: "ExpiresAt");
            migrationBuilder.CreateIndex(name: "IX_Sessions_IpAddress", table: "Sessions", column: "IpAddress");

            migrationBuilder.CreateIndex(name: "IX_TwoFactorTokens_ExpiresAt", table: "TwoFactorTokens", column: "ExpiresAt");
            migrationBuilder.CreateIndex(name: "IX_TwoFactorTokens_UserId_Token_ExpiresAt", table: "TwoFactorTokens", columns: new[] { "UserId", "Token", "ExpiresAt" });

            migrationBuilder.CreateIndex(name: "IX_AccountLockouts_UserId", table: "AccountLockouts", column: "UserId", unique: true);
            migrationBuilder.CreateIndex(name: "IX_AccountLockouts_IsCurrentlyLocked", table: "AccountLockouts", column: "IsCurrentlyLocked");
            migrationBuilder.CreateIndex(name: "IX_AccountLockouts_LockedUntil", table: "AccountLockouts", column: "LockedUntil");

            migrationBuilder.CreateIndex(name: "IX_PasswordPolicies_IsDefault", table: "PasswordPolicies", column: "IsDefault");
            migrationBuilder.CreateIndex(name: "IX_PasswordPolicies_Name", table: "PasswordPolicies", column: "Name", unique: true);

            migrationBuilder.CreateIndex(name: "IX_PasswordHistories_UserId_PasswordSetAt", table: "PasswordHistories", columns: new[] { "UserId", "PasswordSetAt" });
            migrationBuilder.CreateIndex(name: "IX_PasswordHistories_UserId_IsCurrentPassword", table: "PasswordHistories", columns: new[] { "UserId", "IsCurrentPassword" });

            migrationBuilder.CreateIndex(name: "IX_IpWhitelists_IpAddress_Type", table: "IpWhitelists", columns: new[] { "IpAddress", "Type" });
            migrationBuilder.CreateIndex(name: "IX_IpWhitelists_Type_IsActive", table: "IpWhitelists", columns: new[] { "Type", "IsActive" });
            migrationBuilder.CreateIndex(name: "IX_IpWhitelists_ExpiresAt", table: "IpWhitelists", column: "ExpiresAt");

            migrationBuilder.CreateIndex(name: "IX_AccountFees_AccountId", table: "AccountFees", column: "AccountId");
            migrationBuilder.CreateIndex(name: "IX_AccountFees_TransactionId", table: "AccountFees", column: "TransactionId");

            migrationBuilder.CreateIndex(name: "IX_AccountHolds_AccountId", table: "AccountHolds", column: "AccountId");
            migrationBuilder.CreateIndex(name: "IX_AccountHolds_PlacedByUserId", table: "AccountHolds", column: "PlacedByUserId");
            migrationBuilder.CreateIndex(name: "IX_AccountHolds_ReleasedByUserId", table: "AccountHolds", column: "ReleasedByUserId");

            migrationBuilder.CreateIndex(name: "IX_AccountRestrictions_AccountId", table: "AccountRestrictions", column: "AccountId");
            migrationBuilder.CreateIndex(name: "IX_AccountRestrictions_AppliedByUserId", table: "AccountRestrictions", column: "AppliedByUserId");
            migrationBuilder.CreateIndex(name: "IX_AccountRestrictions_RemovedByUserId", table: "AccountRestrictions", column: "RemovedByUserId");

            migrationBuilder.CreateIndex(name: "IX_AccountStatusHistories_AccountId", table: "AccountStatusHistories", column: "AccountId");
            migrationBuilder.CreateIndex(name: "IX_AccountStatusHistories_ChangedByUserId", table: "AccountStatusHistories", column: "ChangedByUserId");

            migrationBuilder.CreateIndex(name: "IX_FeeSchedules_CreatedByUserId", table: "FeeSchedules", column: "CreatedByUserId");

            migrationBuilder.CreateIndex(name: "IX_JointAccountHolders_AccountId_UserId", table: "JointAccountHolders", columns: new[] { "AccountId", "UserId" });
            migrationBuilder.CreateIndex(name: "IX_JointAccountHolders_UserId", table: "JointAccountHolders", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_JointAccountHolders_AddedByUserId", table: "JointAccountHolders", column: "AddedByUserId");
            migrationBuilder.CreateIndex(name: "IX_JointAccountHolders_RemovedByUserId", table: "JointAccountHolders", column: "RemovedByUserId");

            // NOTE: This migration covers the core account, authentication, and session tables.
            // Additional tables for Cards, Deposits, Loans, Payments, and Statements are pending.
            // These should be created in subsequent migrations to keep this migration file manageable.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop all created tables in reverse order
            migrationBuilder.DropTable(name: "JointAccountHolders");
            migrationBuilder.DropTable(name: "FeeSchedules");
            migrationBuilder.DropTable(name: "AccountStatusHistories");
            migrationBuilder.DropTable(name: "AccountRestrictions");
            migrationBuilder.DropTable(name: "AccountHolds");
            migrationBuilder.DropTable(name: "AccountFees");
            migrationBuilder.DropTable(name: "IpWhitelists");
            migrationBuilder.DropTable(name: "PasswordHistories");
            migrationBuilder.DropTable(name: "PasswordPolicies");
            migrationBuilder.DropTable(name: "AccountLockouts");
            migrationBuilder.DropTable(name: "TwoFactorTokens");
            migrationBuilder.DropTable(name: "Sessions");
            migrationBuilder.DropTable(name: "Transactions");
            migrationBuilder.DropTable(name: "Accounts");
        }
    }
}
