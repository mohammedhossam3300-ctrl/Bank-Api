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
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorStatus = table.Column<int>(type: "int", nullable: false),
                    TwoFactorSecretKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorBackupCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorSetupDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastTwoFactorUsed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatchJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    SuccessCount = table.Column<int>(type: "int", nullable: false),
                    FailureCount = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Billers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoutingNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SupportedPaymentMethods = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, defaultValue: "[]"),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0.01m),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 10000.00m),
                    ProcessingDays = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepositProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProductType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MinimumTermDays = table.Column<int>(type: "int", nullable: true),
                    MaximumTermDays = table.Column<int>(type: "int", nullable: true),
                    DefaultTermDays = table.Column<int>(type: "int", nullable: true),
                    MinimumBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaximumBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MinimumOpeningBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BaseInterestRate = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    InterestCalculationMethod = table.Column<int>(type: "int", nullable: false),
                    CompoundingFrequency = table.Column<int>(type: "int", nullable: false),
                    HasTieredRates = table.Column<bool>(type: "bit", nullable: false),
                    AllowPartialWithdrawals = table.Column<bool>(type: "bit", nullable: false),
                    PenaltyType = table.Column<int>(type: "int", nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PenaltyPercentage = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    PenaltyFreeDays = table.Column<int>(type: "int", nullable: true),
                    DefaultMaturityAction = table.Column<int>(type: "int", nullable: false),
                    AllowAutoRenewal = table.Column<bool>(type: "bit", nullable: false),
                    AutoRenewalNoticeDays = table.Column<int>(type: "int", nullable: true),
                    PromotionalRateStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PromotionalRateEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PromotionalRate = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ComplexityLevel = table.Column<int>(type: "int", nullable: false),
                    MinimumLength = table.Column<int>(type: "int", nullable: false),
                    MaximumLength = table.Column<int>(type: "int", nullable: false),
                    RequireUppercase = table.Column<bool>(type: "bit", nullable: false),
                    RequireLowercase = table.Column<bool>(type: "bit", nullable: false),
                    RequireDigits = table.Column<bool>(type: "bit", nullable: false),
                    RequireSpecialCharacters = table.Column<bool>(type: "bit", nullable: false),
                    AllowedSpecialCharacters = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MinimumUniqueCharacters = table.Column<int>(type: "int", nullable: false),
                    PreventCommonPasswords = table.Column<bool>(type: "bit", nullable: false),
                    PreventUserInfoInPassword = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHistoryCount = table.Column<int>(type: "int", nullable: false),
                    MaxPasswordAge = table.Column<TimeSpan>(type: "time", nullable: false),
                    MinPasswordAge = table.Column<TimeSpan>(type: "time", nullable: true),
                    MaxFailedAttempts = table.Column<int>(type: "int", nullable: false),
                    LockoutDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountLockouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false),
                    LockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockoutReason = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastFailedAttempt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSuccessfulLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrentlyLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockoutNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LockedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLockouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountLockouts_AspNetUsers_LockedByUserId",
                        column: x => x.LockedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AccountLockouts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OpenedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosureReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastActivityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DormancyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DormancyPeriodDays = table.Column<int>(type: "int", nullable: false),
                    MinimumBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyMaintenanceFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeeWaiverEligible = table.Column<bool>(type: "bit", nullable: false),
                    LastFeeCalculationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastInterestCalculationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompoundingFrequency = table.Column<int>(type: "int", nullable: false),
                    IsJointAccount = table.Column<bool>(type: "bit", nullable: false),
                    RequiresMultipleSignatures = table.Column<bool>(type: "bit", nullable: false),
                    MultipleSignatureThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinimumSignaturesRequired = table.Column<int>(type: "int", nullable: false),
                    HasHolds = table.Column<bool>(type: "bit", nullable: false),
                    HasRestrictions = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    AdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SwiftCode = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    IbanNumber = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: true),
                    RoutingNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VerifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DailyTransferLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MonthlyTransferLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SingleTransferLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastTransferDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastTransferAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransferCount = table.Column<int>(type: "int", nullable: false),
                    TotalTransferCount = table.Column<int>(type: "int", nullable: false),
                    TotalTransferAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchiveReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_AspNetUsers_VerifiedByUserId",
                        column: x => x.VerifiedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FeeSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MinimumBalanceThreshold = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaximumBalanceThreshold = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DormancyDaysThreshold = table.Column<int>(type: "int", nullable: true),
                    TransactionCountThreshold = table.Column<int>(type: "int", nullable: true),
                    WaiverMinimumBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    WaiverForPremiumAccounts = table.Column<bool>(type: "bit", nullable: false),
                    WaiverConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeSchedules_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IpWhitelists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    IpRange = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpWhitelists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IpWhitelists_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_IpWhitelists_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    TermInMonths = table.Column<int>(type: "int", nullable: false),
                    CalculationMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisbursementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaturityDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OutstandingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalInterestPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrincipalPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyPaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditScore = table.Column<int>(type: "int", nullable: true),
                    CreditScoreRange = table.Column<int>(type: "int", nullable: true),
                    CreditScoringDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DaysOverdue = table.Column<int>(type: "int", nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextPaymentDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_AspNetUsers_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Loans_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HasValue = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionAlerts = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SecurityAlerts = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LowBalanceAlerts = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PaymentReminders = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MarketingNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CardAlerts = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LoanAlerts = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AccountUpdates = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TransactionAlertThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    LowBalanceThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 100m),
                    PreferredChannels = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "[1,2]"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    TimeZone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "UTC"),
                    QuietHoursStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    QuietHoursEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    AllowCriticalDuringQuietHours = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxRetries = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    ExternalReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TemplateId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordSetAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsCurrentPassword = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionToken = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DeviceFingerprint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TerminatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TerminationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdminSession = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TwoFactorTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwoFactorTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillerHealthChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsHealthy = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CheckDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HealthMetricsJson = table.Column<string>(type: "TEXT", nullable: false),
                    ConsecutiveFailures = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastSuccessfulCheck = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillerHealthChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillerHealthChecks_Billers_BillerId",
                        column: x => x.BillerId,
                        principalTable: "Billers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterestTiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepositProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TierName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MinimumBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaximumBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    TierBasis = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    EffectiveFromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPromotional = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestTiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestTiers_DepositProducts_DepositProductId",
                        column: x => x.DepositProductId,
                        principalTable: "DepositProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountHolds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PlacedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleasedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlacedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReleasedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHolds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountHolds_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountHolds_AspNetUsers_PlacedByUserId",
                        column: x => x.PlacedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountHolds_AspNetUsers_ReleasedByUserId",
                        column: x => x.ReleasedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AccountRestrictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AppliedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RemovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TransactionCountLimit = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountRestrictions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountRestrictions_AspNetUsers_AppliedByUserId",
                        column: x => x.AppliedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountRestrictions_AspNetUsers_RemovedByUserId",
                        column: x => x.RemovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AccountStatements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatementNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatementSequence = table.Column<int>(type: "int", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClosingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AverageBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTransactions = table.Column<int>(type: "int", nullable: false),
                    DebitTransactions = table.Column<int>(type: "int", nullable: false),
                    CreditTransactions = table.Column<int>(type: "int", nullable: false),
                    TotalDebits = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    TotalCredits = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    TotalFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestEarned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestCharged = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    Format = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DeliveryMethod = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    FileHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountStatements_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountStatements_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystemGenerated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountStatusHistories_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountStatusHistories_AspNetUsers_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false, comment: "Encrypted card number"),
                    MaskedCardNumber = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false, comment: "Masked card number for display"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ActivationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivationChannel = table.Column<int>(type: "int", nullable: true),
                    SecurityCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Encrypted security code"),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 5000m),
                    MonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 50000m),
                    AtmDailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 2000m),
                    ContactlessEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    OnlineTransactionsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    InternationalTransactionsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PinHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PinSetDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedPinAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastBlockedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastBlockReason = table.Column<int>(type: "int", nullable: true),
                    BlockedMerchantCategories = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "JSON array of blocked merchant categories"),
                    CardName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cards_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FixedDeposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepositNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepositProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkedAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    TermDays = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaturityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InterestCalculationMethod = table.Column<int>(type: "int", nullable: false),
                    CompoundingFrequency = table.Column<int>(type: "int", nullable: false),
                    AccruedInterest = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LastInterestCalculationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaturityAction = table.Column<int>(type: "int", nullable: false),
                    AutoRenewalEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RenewalTermDays = table.Column<int>(type: "int", nullable: true),
                    RenewalNoticeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerConsentReceived = table.Column<bool>(type: "bit", nullable: false),
                    PenaltyType = table.Column<int>(type: "int", nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PenaltyPercentage = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    ClosureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClosureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PenaltyApplied = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    NetAmountPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RenewedFromDepositId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RenewedToDepositId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RenewalCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedDeposits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixedDeposits_Accounts_LinkedAccountId",
                        column: x => x.LinkedAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedDeposits_AspNetUsers_ClosedByUserId",
                        column: x => x.ClosedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FixedDeposits_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedDeposits_DepositProducts_DepositProductId",
                        column: x => x.DepositProductId,
                        principalTable: "DepositProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedDeposits_FixedDeposits_RenewedFromDepositId",
                        column: x => x.RenewedFromDepositId,
                        principalTable: "FixedDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "JointAccountHolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RemovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequiresSignature = table.Column<bool>(type: "bit", nullable: false),
                    TransactionLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JointAccountHolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JointAccountHolders_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JointAccountHolders_AspNetUsers_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JointAccountHolders_AspNetUsers_RemovedByUserId",
                        column: x => x.RemovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_JointAccountHolders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FromAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BeneficiaryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BeneficiaryAccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BeneficiaryBankCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    LastUsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTemplates_Accounts_FromAccountId",
                        column: x => x.FromAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTemplates_Accounts_ToAccountId",
                        column: x => x.ToAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PaymentTemplates_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurringPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxOccurrences = table.Column<int>(type: "int", nullable: true),
                    NextExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExecutionCount = table.Column<int>(type: "int", nullable: false),
                    LastExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PausedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PauseReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FailureCount = table.Column<int>(type: "int", nullable: false),
                    MaxRetries = table.Column<int>(type: "int", nullable: false),
                    LastFailureReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringPayments_Accounts_FromAccountId",
                        column: x => x.FromAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringPayments_Accounts_ToAccountId",
                        column: x => x.ToAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringPayments_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FromAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BeneficiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_FromAccountId",
                        column: x => x.FromAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_ToAccountId",
                        column: x => x.ToAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_BatchJobs_BatchJobId",
                        column: x => x.BatchJobId,
                        principalTable: "BatchJobs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Beneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "Beneficiaries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LoanDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VerificationNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanDocuments_AspNetUsers_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LoanDocuments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OutstandingBalanceAfterPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanPayments_AspNetUsers_ProcessedBy",
                        column: x => x.ProcessedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LoanPayments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    StatusChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SystemReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsSystemGenerated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanStatusHistories_AspNetUsers_ChangedBy",
                        column: x => x.ChangedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LoanStatusHistories_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardAuthorizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantCategory = table.Column<int>(type: "int", nullable: false),
                    MerchantCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsInternational = table.Column<bool>(type: "bit", nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    IsContactless = table.Column<bool>(type: "bit", nullable: false),
                    CapturedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CaptureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoidReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessorResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NetworkTransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardAuthorizations_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardStatements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionCount = table.Column<int>(type: "int", nullable: false),
                    TotalSpent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DeliveryMethod = table.Column<int>(type: "int", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GeneratedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardStatements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardStatements_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardStatusHistories_AspNetUsers_ChangedBy",
                        column: x => x.ChangedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CardStatusHistories_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MerchantCategory = table.Column<int>(type: "int", nullable: false),
                    MerchantCountry = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    SettlementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsInternational = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsContactless = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Fees = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FeeBreakdown = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OriginalTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeclineReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProcessorResponse = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CardId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardTransactions_CardTransactions_OriginalTransactionId",
                        column: x => x.OriginalTransactionId,
                        principalTable: "CardTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CardTransactions_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardTransactions_Cards_CardId1",
                        column: x => x.CardId1,
                        principalTable: "Cards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DepositCertificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FixedDepositId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeliveryReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CertificateTemplate = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificateContent = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CertificatePdf = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PdfFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DigitalSignature = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SecurityHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacedCertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReplacementReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReplacedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GeneratedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IssuedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcessingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepositCertificates_AspNetUsers_GeneratedByUserId",
                        column: x => x.GeneratedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DepositCertificates_AspNetUsers_IssuedByUserId",
                        column: x => x.IssuedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DepositCertificates_AspNetUsers_ReplacedByUserId",
                        column: x => x.ReplacedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DepositCertificates_DepositCertificates_ReplacedCertificateId",
                        column: x => x.ReplacedCertificateId,
                        principalTable: "DepositCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DepositCertificates_FixedDeposits_FixedDepositId",
                        column: x => x.FixedDepositId,
                        principalTable: "FixedDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaturityNotices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FixedDepositId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoticeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NoticeType = table.Column<int>(type: "int", nullable: false),
                    NoticeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaturityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TemplateUsed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveryChannel = table.Column<int>(type: "int", nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveryAttempts = table.Column<int>(type: "int", nullable: false),
                    CustomerResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerChoice = table.Column<int>(type: "int", nullable: true),
                    CustomerInstructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConsentReceived = table.Column<bool>(type: "bit", nullable: false),
                    GeneratedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiresFollowUp = table.Column<bool>(type: "bit", nullable: false),
                    RemindersSent = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaturityNotices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaturityNotices_AspNetUsers_GeneratedByUserId",
                        column: x => x.GeneratedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MaturityNotices_FixedDeposits_FixedDepositId",
                        column: x => x.FixedDepositId,
                        principalTable: "FixedDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: ""),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    RecurringPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillPayments_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillPayments_Billers_BillerId",
                        column: x => x.BillerId,
                        principalTable: "Billers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillPayments_RecurringPayments_RecurringPaymentId",
                        column: x => x.RecurringPaymentId,
                        principalTable: "RecurringPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AccountFees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CalculatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsWaived = table.Column<bool>(type: "bit", nullable: false),
                    WaiverReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WaivedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    NextCalculationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountFees_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountFees_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DepositTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepositId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FixedDepositId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InterestPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InterestPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true),
                    InterestDays = table.Column<int>(type: "int", nullable: true),
                    PenaltyType = table.Column<int>(type: "int", nullable: true),
                    PenaltyReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProcessedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RelatedTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepositTransactions_AspNetUsers_ProcessedByUserId",
                        column: x => x.ProcessedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DepositTransactions_DepositTransactions_RelatedTransactionId",
                        column: x => x.RelatedTransactionId,
                        principalTable: "DepositTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DepositTransactions_FixedDeposits_FixedDepositId",
                        column: x => x.FixedDepositId,
                        principalTable: "FixedDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepositTransactions_Transactions_AccountTransactionId",
                        column: x => x.AccountTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RecurringPaymentExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecurringPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringPaymentExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringPaymentExecutions_RecurringPayments_RecurringPaymentId",
                        column: x => x.RecurringPaymentId,
                        principalTable: "RecurringPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringPaymentExecutions_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StatementTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RunningBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsReconciled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatementTransactions_AccountStatements_StatementId",
                        column: x => x.StatementId,
                        principalTable: "AccountStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatementTransactions_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BillPresentments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AmountDue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinimumPayment = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    BillNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExternalBillId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LineItemsJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillPresentments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillPresentments_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillPresentments_BillPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "BillPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BillPresentments_Billers_BillerId",
                        column: x => x.BillerId,
                        principalTable: "Billers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BillerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    ProcessingFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReceiptDataJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentReceipts_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentReceipts_BillPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "BillPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRetries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    AttemptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextRetryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BackoffDelay = table.Column<TimeSpan>(type: "time", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsMaxRetriesReached = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RetryMetadataJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRetries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRetries_BillPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "BillPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountFees_AccountId",
                table: "AccountFees",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFees_TransactionId",
                table: "AccountFees",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHolds_AccountId",
                table: "AccountHolds",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHolds_PlacedByUserId",
                table: "AccountHolds",
                column: "PlacedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHolds_ReleasedByUserId",
                table: "AccountHolds",
                column: "ReleasedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLockouts_IsCurrentlyLocked",
                table: "AccountLockouts",
                column: "IsCurrentlyLocked");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLockouts_LockedByUserId",
                table: "AccountLockouts",
                column: "LockedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLockouts_LockedUntil",
                table: "AccountLockouts",
                column: "LockedUntil");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLockouts_UserId",
                table: "AccountLockouts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountRestrictions_AccountId",
                table: "AccountRestrictions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRestrictions_AppliedByUserId",
                table: "AccountRestrictions",
                column: "AppliedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRestrictions_RemovedByUserId",
                table: "AccountRestrictions",
                column: "RemovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountNumber",
                table: "Accounts",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_AccountId",
                table: "AccountStatements",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_AccountId_StatementDate",
                table: "AccountStatements",
                columns: new[] { "AccountId", "StatementDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_AccountId_Status",
                table: "AccountStatements",
                columns: new[] { "AccountId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_PeriodEndDate",
                table: "AccountStatements",
                column: "PeriodEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_PeriodStartDate",
                table: "AccountStatements",
                column: "PeriodStartDate");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_RequestedByUserId",
                table: "AccountStatements",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_StatementDate",
                table: "AccountStatements",
                column: "StatementDate");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_Status",
                table: "AccountStatements",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatusHistories_AccountId",
                table: "AccountStatusHistories",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatusHistories_ChangedByUserId",
                table: "AccountStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EventType",
                table: "AuditLogs",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_IpAddress",
                table: "AuditLogs",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_CustomerId_AccountNumber_BankCode",
                table: "Beneficiaries",
                columns: new[] { "CustomerId", "AccountNumber", "BankCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_CustomerId_Category",
                table: "Beneficiaries",
                columns: new[] { "CustomerId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_CustomerId_IsActive",
                table: "Beneficiaries",
                columns: new[] { "CustomerId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_CustomerId_Type",
                table: "Beneficiaries",
                columns: new[] { "CustomerId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_IsVerified",
                table: "Beneficiaries",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_LastTransferDate",
                table: "Beneficiaries",
                column: "LastTransferDate");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_Status",
                table: "Beneficiaries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_VerifiedByUserId",
                table: "Beneficiaries",
                column: "VerifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BillerHealthChecks_BillerId",
                table: "BillerHealthChecks",
                column: "BillerId");

            migrationBuilder.CreateIndex(
                name: "IX_BillerHealthChecks_BillerId_CheckDate",
                table: "BillerHealthChecks",
                columns: new[] { "BillerId", "CheckDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BillerHealthChecks_CheckDate",
                table: "BillerHealthChecks",
                column: "CheckDate");

            migrationBuilder.CreateIndex(
                name: "IX_BillerHealthChecks_IsHealthy",
                table: "BillerHealthChecks",
                column: "IsHealthy");

            migrationBuilder.CreateIndex(
                name: "IX_Billers_AccountNumber_RoutingNumber",
                table: "Billers",
                columns: new[] { "AccountNumber", "RoutingNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Billers_Category",
                table: "Billers",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Billers_IsActive",
                table: "Billers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Billers_Name",
                table: "Billers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayments_BillerId",
                table: "BillPayments",
                column: "BillerId");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayments_CustomerId",
                table: "BillPayments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayments_CustomerId_Status",
                table: "BillPayments",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BillPayments_RecurringPaymentId",
                table: "BillPayments",
                column: "RecurringPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayments_ScheduledDate",
                table: "BillPayments",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayments_Status",
                table: "BillPayments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayments_Status_ScheduledDate",
                table: "BillPayments",
                columns: new[] { "Status", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BillPresentments_BillerId",
                table: "BillPresentments",
                column: "BillerId");

            migrationBuilder.CreateIndex(
                name: "IX_BillPresentments_CustomerId",
                table: "BillPresentments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BillPresentments_CustomerId_Status",
                table: "BillPresentments",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BillPresentments_DueDate",
                table: "BillPresentments",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_BillPresentments_ExternalBillId",
                table: "BillPresentments",
                column: "ExternalBillId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillPresentments_PaymentId",
                table: "BillPresentments",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_BillPresentments_Status",
                table: "BillPresentments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CardAuthorizations_CardId",
                table: "CardAuthorizations",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AccountId",
                table: "Cards",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardNumber",
                table: "Cards",
                column: "CardNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CustomerId",
                table: "Cards",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CustomerId_Status",
                table: "Cards",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_ExpiryDate",
                table: "Cards",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Status",
                table: "Cards",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CardStatements_CardId",
                table: "CardStatements",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardStatusHistories_CardId",
                table: "CardStatusHistories",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardStatusHistories_CardId_ChangeDate",
                table: "CardStatusHistories",
                columns: new[] { "CardId", "ChangeDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CardStatusHistories_ChangeDate",
                table: "CardStatusHistories",
                column: "ChangeDate");

            migrationBuilder.CreateIndex(
                name: "IX_CardStatusHistories_ChangedBy",
                table: "CardStatusHistories",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_AuthorizationCode",
                table: "CardTransactions",
                column: "AuthorizationCode",
                filter: "[AuthorizationCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_CardId",
                table: "CardTransactions",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_CardId_Status",
                table: "CardTransactions",
                columns: new[] { "CardId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_CardId_TransactionDate",
                table: "CardTransactions",
                columns: new[] { "CardId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_CardId1",
                table: "CardTransactions",
                column: "CardId1");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_MerchantId",
                table: "CardTransactions",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_OriginalTransactionId",
                table: "CardTransactions",
                column: "OriginalTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_SettlementDate",
                table: "CardTransactions",
                column: "SettlementDate");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_Status",
                table: "CardTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_TransactionDate",
                table: "CardTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_TransactionType",
                table: "CardTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_DepositCertificates_CertificateNumber",
                table: "DepositCertificates",
                column: "CertificateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepositCertificates_FixedDepositId",
                table: "DepositCertificates",
                column: "FixedDepositId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositCertificates_GeneratedByUserId",
                table: "DepositCertificates",
                column: "GeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositCertificates_IssueDate",
                table: "DepositCertificates",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_DepositCertificates_IssuedByUserId",
                table: "DepositCertificates",
                column: "IssuedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositCertificates_ReplacedByUserId",
                table: "DepositCertificates",
                column: "ReplacedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositCertificates_ReplacedCertificateId",
                table: "DepositCertificates",
                column: "ReplacedCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositCertificates_Status",
                table: "DepositCertificates",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DepositProducts_IsActive",
                table: "DepositProducts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DepositProducts_ProductType",
                table: "DepositProducts",
                column: "ProductType");

            migrationBuilder.CreateIndex(
                name: "IX_DepositProducts_ProductType_IsActive",
                table: "DepositProducts",
                columns: new[] { "ProductType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DepositTransactions_AccountTransactionId",
                table: "DepositTransactions",
                column: "AccountTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositTransactions_FixedDepositId",
                table: "DepositTransactions",
                column: "FixedDepositId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositTransactions_FixedDepositId_TransactionDate",
                table: "DepositTransactions",
                columns: new[] { "FixedDepositId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DepositTransactions_FixedDepositId_TransactionType",
                table: "DepositTransactions",
                columns: new[] { "FixedDepositId", "TransactionType" });

            migrationBuilder.CreateIndex(
                name: "IX_DepositTransactions_ProcessedByUserId",
                table: "DepositTransactions",
                column: "ProcessedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositTransactions_RelatedTransactionId",
                table: "DepositTransactions",
                column: "RelatedTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositTransactions_TransactionDate",
                table: "DepositTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_DepositTransactions_TransactionReference",
                table: "DepositTransactions",
                column: "TransactionReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepositTransactions_TransactionType",
                table: "DepositTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_FeeSchedules_CreatedByUserId",
                table: "FeeSchedules",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposits_ClosedByUserId",
                table: "FixedDeposits",
                column: "ClosedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposits_CustomerId",
                table: "FixedDeposits",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposits_CustomerId_Status",
                table: "FixedDeposits",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposits_DepositNumber",
                table: "FixedDeposits",
                column: "DepositNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposits_DepositProductId",
                table: "FixedDeposits",
                column: "DepositProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposits_LinkedAccountId",
                table: "FixedDeposits",
                column: "LinkedAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposits_MaturityDate",
                table: "FixedDeposits",
                column: "MaturityDate");

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposits_RenewedFromDepositId",
                table: "FixedDeposits",
                column: "RenewedFromDepositId",
                unique: true,
                filter: "[RenewedFromDepositId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposits_Status",
                table: "FixedDeposits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposits_Status_MaturityDate",
                table: "FixedDeposits",
                columns: new[] { "Status", "MaturityDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InterestTiers_DepositProductId",
                table: "InterestTiers",
                column: "DepositProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestTiers_DepositProductId_IsActive",
                table: "InterestTiers",
                columns: new[] { "DepositProductId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_InterestTiers_DepositProductId_MinimumBalance",
                table: "InterestTiers",
                columns: new[] { "DepositProductId", "MinimumBalance" });

            migrationBuilder.CreateIndex(
                name: "IX_InterestTiers_DisplayOrder",
                table: "InterestTiers",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_IpWhitelists_ApprovedByUserId",
                table: "IpWhitelists",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IpWhitelists_CreatedByUserId",
                table: "IpWhitelists",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IpWhitelists_ExpiresAt",
                table: "IpWhitelists",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_IpWhitelists_IpAddress_Type",
                table: "IpWhitelists",
                columns: new[] { "IpAddress", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_IpWhitelists_Type_IsActive",
                table: "IpWhitelists",
                columns: new[] { "Type", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_JointAccountHolders_AccountId_UserId",
                table: "JointAccountHolders",
                columns: new[] { "AccountId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_JointAccountHolders_AddedByUserId",
                table: "JointAccountHolders",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JointAccountHolders_RemovedByUserId",
                table: "JointAccountHolders",
                column: "RemovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JointAccountHolders_UserId",
                table: "JointAccountHolders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDocuments_DocumentType",
                table: "LoanDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDocuments_IsVerified",
                table: "LoanDocuments",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDocuments_LoanId",
                table: "LoanDocuments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDocuments_VerifiedBy",
                table: "LoanDocuments",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LoanPayments_DueDate",
                table: "LoanPayments",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_LoanPayments_LoanId",
                table: "LoanPayments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanPayments_PaymentDate",
                table: "LoanPayments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_LoanPayments_ProcessedBy",
                table: "LoanPayments",
                column: "ProcessedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LoanPayments_Status",
                table: "LoanPayments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ApplicationDate",
                table: "Loans",
                column: "ApplicationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ApprovedBy",
                table: "Loans",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_CustomerId",
                table: "Loans",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_LoanNumber",
                table: "Loans",
                column: "LoanNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_NextPaymentDueDate",
                table: "Loans",
                column: "NextPaymentDueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_Status",
                table: "Loans",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_Type",
                table: "Loans",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_LoanStatusHistories_ChangedBy",
                table: "LoanStatusHistories",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LoanStatusHistories_FromStatus",
                table: "LoanStatusHistories",
                column: "FromStatus");

            migrationBuilder.CreateIndex(
                name: "IX_LoanStatusHistories_LoanId",
                table: "LoanStatusHistories",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanStatusHistories_StatusChangeDate",
                table: "LoanStatusHistories",
                column: "StatusChangeDate");

            migrationBuilder.CreateIndex(
                name: "IX_LoanStatusHistories_ToStatus",
                table: "LoanStatusHistories",
                column: "ToStatus");

            migrationBuilder.CreateIndex(
                name: "IX_MaturityNotices_FixedDepositId",
                table: "MaturityNotices",
                column: "FixedDepositId");

            migrationBuilder.CreateIndex(
                name: "IX_MaturityNotices_GeneratedByUserId",
                table: "MaturityNotices",
                column: "GeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaturityNotices_MaturityDate",
                table: "MaturityNotices",
                column: "MaturityDate");

            migrationBuilder.CreateIndex(
                name: "IX_MaturityNotices_NoticeNumber",
                table: "MaturityNotices",
                column: "NoticeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaturityNotices_NoticeType",
                table: "MaturityNotices",
                column: "NoticeType");

            migrationBuilder.CreateIndex(
                name: "IX_MaturityNotices_Status",
                table: "MaturityNotices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MaturityNotices_Status_NoticeDate",
                table: "MaturityNotices",
                columns: new[] { "Status", "NoticeDate" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_UserId",
                table: "NotificationPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ScheduledAt",
                table: "Notifications",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status",
                table: "Notifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type",
                table: "Notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_Status",
                table: "Notifications",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistories_UserId_IsCurrentPassword",
                table: "PasswordHistories",
                columns: new[] { "UserId", "IsCurrentPassword" });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistories_UserId_PasswordSetAt",
                table: "PasswordHistories",
                columns: new[] { "UserId", "PasswordSetAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordPolicies_ComplexityLevel",
                table: "PasswordPolicies",
                column: "ComplexityLevel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordPolicies_IsDefault",
                table: "PasswordPolicies",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordPolicies_Name",
                table: "PasswordPolicies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_ConfirmationNumber",
                table: "PaymentReceipts",
                column: "ConfirmationNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_CustomerId",
                table: "PaymentReceipts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_PaymentId",
                table: "PaymentReceipts",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_ReceiptNumber",
                table: "PaymentReceipts",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_Status",
                table: "PaymentReceipts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRetries_IsMaxRetriesReached",
                table: "PaymentRetries",
                column: "IsMaxRetriesReached");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRetries_NextRetryDate",
                table: "PaymentRetries",
                column: "NextRetryDate");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRetries_PaymentId",
                table: "PaymentRetries",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRetries_PaymentId_AttemptNumber",
                table: "PaymentRetries",
                columns: new[] { "PaymentId", "AttemptNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRetries_Status",
                table: "PaymentRetries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTemplates_CreatedByUserId_Category",
                table: "PaymentTemplates",
                columns: new[] { "CreatedByUserId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTemplates_CreatedByUserId_IsActive",
                table: "PaymentTemplates",
                columns: new[] { "CreatedByUserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTemplates_CreatedByUserId_LastUsedDate",
                table: "PaymentTemplates",
                columns: new[] { "CreatedByUserId", "LastUsedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTemplates_CreatedByUserId_UsageCount",
                table: "PaymentTemplates",
                columns: new[] { "CreatedByUserId", "UsageCount" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTemplates_FromAccountId",
                table: "PaymentTemplates",
                column: "FromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTemplates_ToAccountId",
                table: "PaymentTemplates",
                column: "ToAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringPaymentExecutions_RecurringPaymentId_ScheduledDate",
                table: "RecurringPaymentExecutions",
                columns: new[] { "RecurringPaymentId", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringPaymentExecutions_TransactionId",
                table: "RecurringPaymentExecutions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringPayments_CreatedByUserId",
                table: "RecurringPayments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringPayments_FromAccountId",
                table: "RecurringPayments",
                column: "FromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringPayments_Status_NextExecutionDate",
                table: "RecurringPayments",
                columns: new[] { "Status", "NextExecutionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringPayments_ToAccountId",
                table: "RecurringPayments",
                column: "ToAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ExpiresAt",
                table: "Sessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IpAddress",
                table: "Sessions",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RefreshToken",
                table: "Sessions",
                column: "RefreshToken");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionToken",
                table: "Sessions",
                column: "SessionToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId_Status",
                table: "Sessions",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StatementTransactions_Category",
                table: "StatementTransactions",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_StatementTransactions_StatementId",
                table: "StatementTransactions",
                column: "StatementId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementTransactions_StatementId_TransactionDate",
                table: "StatementTransactions",
                columns: new[] { "StatementId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StatementTransactions_TransactionDate",
                table: "StatementTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_StatementTransactions_TransactionId",
                table: "StatementTransactions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementTransactions_Type",
                table: "StatementTransactions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BatchJobId",
                table: "Transactions",
                column: "BatchJobId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BeneficiaryId",
                table: "Transactions",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FromAccountId",
                table: "Transactions",
                column: "FromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ToAccountId",
                table: "Transactions",
                column: "ToAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorTokens_ExpiresAt",
                table: "TwoFactorTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorTokens_UserId_Token_ExpiresAt",
                table: "TwoFactorTokens",
                columns: new[] { "UserId", "Token", "ExpiresAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountFees");

            migrationBuilder.DropTable(
                name: "AccountHolds");

            migrationBuilder.DropTable(
                name: "AccountLockouts");

            migrationBuilder.DropTable(
                name: "AccountRestrictions");

            migrationBuilder.DropTable(
                name: "AccountStatusHistories");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BillerHealthChecks");

            migrationBuilder.DropTable(
                name: "BillPresentments");

            migrationBuilder.DropTable(
                name: "CardAuthorizations");

            migrationBuilder.DropTable(
                name: "CardStatements");

            migrationBuilder.DropTable(
                name: "CardStatusHistories");

            migrationBuilder.DropTable(
                name: "CardTransactions");

            migrationBuilder.DropTable(
                name: "DepositCertificates");

            migrationBuilder.DropTable(
                name: "DepositTransactions");

            migrationBuilder.DropTable(
                name: "FeeSchedules");

            migrationBuilder.DropTable(
                name: "InterestTiers");

            migrationBuilder.DropTable(
                name: "IpWhitelists");

            migrationBuilder.DropTable(
                name: "JointAccountHolders");

            migrationBuilder.DropTable(
                name: "LoanDocuments");

            migrationBuilder.DropTable(
                name: "LoanPayments");

            migrationBuilder.DropTable(
                name: "LoanStatusHistories");

            migrationBuilder.DropTable(
                name: "MaturityNotices");

            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PasswordHistories");

            migrationBuilder.DropTable(
                name: "PasswordPolicies");

            migrationBuilder.DropTable(
                name: "PaymentReceipts");

            migrationBuilder.DropTable(
                name: "PaymentRetries");

            migrationBuilder.DropTable(
                name: "PaymentTemplates");

            migrationBuilder.DropTable(
                name: "RecurringPaymentExecutions");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "StatementTransactions");

            migrationBuilder.DropTable(
                name: "TwoFactorTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "FixedDeposits");

            migrationBuilder.DropTable(
                name: "BillPayments");

            migrationBuilder.DropTable(
                name: "AccountStatements");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "DepositProducts");

            migrationBuilder.DropTable(
                name: "Billers");

            migrationBuilder.DropTable(
                name: "RecurringPayments");

            migrationBuilder.DropTable(
                name: "BatchJobs");

            migrationBuilder.DropTable(
                name: "Beneficiaries");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
