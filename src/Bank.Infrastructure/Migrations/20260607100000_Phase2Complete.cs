using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase2Complete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ==================== TRANSACTION & PAYMENT TABLES ====================

            migrationBuilder.CreateTable(
                name: "RecurringPayment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    NextScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastExecutedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringPayment", x => x.Id);
                    table.ForeignKey("FK_RecurringPayment_Accounts_AccountId", x => x.AccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_RecurringPayment_AspNetUsers_PayeeId", x => x.PayeeId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurringPaymentExecution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RecurringPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringPaymentExecution", x => x.Id);
                    table.ForeignKey("FK_RecurringPaymentExecution_RecurringPayment_RecurringPaymentId", x => x.RecurringPaymentId, "RecurringPayment", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    PayeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FrequencyType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTemplate", x => x.Id);
                    table.ForeignKey("FK_PaymentTemplate_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_PaymentTemplate_AspNetUsers_PayeeId", x => x.PayeeId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    BeneficiaryType = table.Column<int>(type: "int", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiary", x => x.Id);
                    table.ForeignKey("FK_Beneficiary_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint("AK_Beneficiary_UserId_AccountNumber_BankCode", x => new { x.UserId, x.AccountNumber, x.BankCode });
                });

            // ==================== STATEMENT TABLES ====================

            migrationBuilder.CreateTable(
                name: "AccountStatement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    StartBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EndBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDebits = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCredits = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatement", x => x.Id);
                    table.ForeignKey("FK_AccountStatement_Accounts_AccountId", x => x.AccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint("AK_AccountStatement_AccountId_Period", x => new { x.AccountId, x.Period });
                });

            migrationBuilder.CreateTable(
                name: "StatementTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    StatementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementTransaction", x => x.Id);
                    table.ForeignKey("FK_StatementTransaction_AccountStatement_StatementId", x => x.StatementId, "AccountStatement", "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_StatementTransaction_Transactions_TransactionId", x => x.TransactionId, "Transactions", "Id", onDelete: ReferentialAction.Restrict);
                });

            // ==================== LOAN TABLES ====================

            migrationBuilder.CreateTable(
                name: "Loan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    LoanTerm = table.Column<int>(type: "int", nullable: false),
                    MonthlyPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaturityDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loan", x => x.Id);
                    table.ForeignKey("FK_Loan_Accounts_AccountId", x => x.AccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanStatusHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanStatusHistory", x => x.Id);
                    table.ForeignKey("FK_LoanStatusHistory_Loan_LoanId", x => x.LoanId, "Loan", "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_LoanStatusHistory_AspNetUsers_ChangedByUserId", x => x.ChangedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanPayment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Principal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Interest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanPayment", x => x.Id);
                    table.ForeignKey("FK_LoanPayment_Loan_LoanId", x => x.LoanId, "Loan", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanDocument",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDocument", x => x.Id);
                    table.ForeignKey("FK_LoanDocument_Loan_LoanId", x => x.LoanId, "Loan", "Id", onDelete: ReferentialAction.Cascade);
                });

            // ==================== CARD TABLES ====================

            migrationBuilder.CreateTable(
                name: "Card",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    CardType = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CVV = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card", x => x.Id);
                    table.ForeignKey("FK_Card_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint("AK_Card_CardNumber", x => x.CardNumber);
                });

            migrationBuilder.CreateTable(
                name: "CardStatusHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardStatusHistory", x => x.Id);
                    table.ForeignKey("FK_CardStatusHistory_Card_CardId", x => x.CardId, "Card", "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_CardStatusHistory_AspNetUsers_ChangedByUserId", x => x.ChangedByUserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CardTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTransaction", x => x.Id);
                    table.ForeignKey("FK_CardTransaction_Card_CardId", x => x.CardId, "Card", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardAuthorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardAuthorization", x => x.Id);
                    table.ForeignKey("FK_CardAuthorization_Card_CardId", x => x.CardId, "Card", "Id", onDelete: ReferentialAction.Cascade);
                    table.UniqueConstraint("AK_CardAuthorization_AuthorizationCode", x => x.AuthorizationCode);
                });

            migrationBuilder.CreateTable(
                name: "CardStatement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatementPeriod = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    TotalCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardStatement", x => x.Id);
                    table.ForeignKey("FK_CardStatement_Card_CardId", x => x.CardId, "Card", "Id", onDelete: ReferentialAction.Cascade);
                    table.UniqueConstraint("AK_CardStatement_CardId_StatementPeriod", x => new { x.CardId, x.StatementPeriod });
                });

            // ==================== BILL PAYMENT TABLES ====================

            migrationBuilder.CreateTable(
                name: "Biller",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    RoutingNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SupportedPaymentMethods = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProcessingDays = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biller", x => x.Id);
                    table.UniqueConstraint("AK_Biller_Name_RoutingNumber", x => new { x.Name, x.RoutingNumber });
                });

            migrationBuilder.CreateTable(
                name: "BillerHealthCheck",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    BillerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsHealthy = table.Column<bool>(type: "bit", nullable: false),
                    StatusMessage = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    ResponseTime = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillerHealthCheck", x => x.Id);
                    table.ForeignKey("FK_BillerHealthCheck_Biller_BillerId", x => x.BillerId, "Biller", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillPayment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillPayment", x => x.Id);
                    table.ForeignKey("FK_BillPayment_AspNetUsers_CustomerId", x => x.CustomerId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_BillPayment_Biller_BillerId", x => x.BillerId, "Biller", "Id", onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint("AK_BillPayment_Reference", x => x.Reference);
                });

            migrationBuilder.CreateTable(
                name: "BillPresentment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    BillerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsScheduledForPayment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillPresentment", x => x.Id);
                    table.ForeignKey("FK_BillPresentment_Biller_BillerId", x => x.BillerId, "Biller", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_BillPresentment_AspNetUsers_CustomerId", x => x.CustomerId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentReceipt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    BillPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfirmationCode = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentReceipt", x => x.Id);
                    table.ForeignKey("FK_PaymentReceipt_BillPayment_BillPaymentId", x => x.BillPaymentId, "BillPayment", "Id", onDelete: ReferentialAction.Cascade);
                    table.UniqueConstraint("AK_PaymentReceipt_ReceiptNumber", x => x.ReceiptNumber);
                    table.UniqueConstraint("AK_PaymentReceipt_ConfirmationCode", x => x.ConfirmationCode);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRetry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    BillPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    LastRetryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextRetryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRetry", x => x.Id);
                    table.ForeignKey("FK_PaymentRetry_BillPayment_BillPaymentId", x => x.BillPaymentId, "BillPayment", "Id", onDelete: ReferentialAction.Cascade);
                });

            // ==================== DEPOSIT TABLES ====================

            migrationBuilder.CreateTable(
                name: "DepositProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    MinimumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TermInMonths = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositProduct", x => x.Id);
                    table.UniqueConstraint("AK_DepositProduct_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "InterestTier",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    DepositProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestTier", x => x.Id);
                    table.ForeignKey("FK_InterestTier_DepositProduct_DepositProductId", x => x.DepositProductId, "DepositProduct", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FixedDeposit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepositProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TermInMonths = table.Column<int>(type: "int", nullable: false),
                    MaturityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedDeposit", x => x.Id);
                    table.ForeignKey("FK_FixedDeposit_Accounts_AccountId", x => x.AccountId, "Accounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_FixedDeposit_DepositProduct_DepositProductId", x => x.DepositProductId, "DepositProduct", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DepositTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    FixedDepositId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositTransaction", x => x.Id);
                    table.ForeignKey("FK_DepositTransaction_FixedDeposit_FixedDepositId", x => x.FixedDepositId, "FixedDeposit", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepositCertificate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    FixedDepositId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaturityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositCertificate", x => x.Id);
                    table.ForeignKey("FK_DepositCertificate_FixedDeposit_FixedDepositId", x => x.FixedDepositId, "FixedDeposit", "Id", onDelete: ReferentialAction.Cascade);
                    table.UniqueConstraint("AK_DepositCertificate_CertificateNumber", x => x.CertificateNumber);
                });

            migrationBuilder.CreateTable(
                name: "MaturityNotice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    FixedDepositId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoticeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaturityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotificationSent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaturityNotice", x => x.Id);
                    table.ForeignKey("FK_MaturityNotice_FixedDeposit_FixedDepositId", x => x.FixedDepositId, "FixedDeposit", "Id", onDelete: ReferentialAction.Cascade);
                });

            // ==================== SYSTEM TABLES ====================

            migrationBuilder.CreateTable(
                name: "BatchJob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    JobName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    JobType = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RecordsProcessed = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchJob", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                    table.ForeignKey("FK_AuditLog_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey("FK_Notification_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationPreference",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DeliveryMethod = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreference", x => x.Id);
                    table.ForeignKey("FK_NotificationPreference_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint("AK_NotificationPreference_UserId_NotificationType", x => new { x.UserId, x.NotificationType });
                });

            // ==================== CREATE INDEXES ====================

            // RecurringPayment indexes
            migrationBuilder.CreateIndex(name: "IX_RecurringPayment_AccountId", table: "RecurringPayment", column: "AccountId");
            migrationBuilder.CreateIndex(name: "IX_RecurringPayment_PayeeId", table: "RecurringPayment", column: "PayeeId");
            migrationBuilder.CreateIndex(name: "IX_RecurringPayment_Status_NextScheduleDate", table: "RecurringPayment", columns: new[] { "Status", "NextScheduleDate" });

            // RecurringPaymentExecution indexes
            migrationBuilder.CreateIndex(name: "IX_RecurringPaymentExecution_RecurringPaymentId", table: "RecurringPaymentExecution", column: "RecurringPaymentId");
            migrationBuilder.CreateIndex(name: "IX_RecurringPaymentExecution_Status_ExecutedDate", table: "RecurringPaymentExecution", columns: new[] { "Status", "ExecutedDate" });

            // PaymentTemplate indexes
            migrationBuilder.CreateIndex(name: "IX_PaymentTemplate_UserId", table: "PaymentTemplate", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_PaymentTemplate_PayeeId", table: "PaymentTemplate", column: "PayeeId");
            migrationBuilder.CreateIndex(name: "IX_PaymentTemplate_IsActive", table: "PaymentTemplate", column: "IsActive");

            // Beneficiary indexes
            migrationBuilder.CreateIndex(name: "IX_Beneficiary_UserId", table: "Beneficiary", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_Beneficiary_IsVerified", table: "Beneficiary", column: "IsVerified");

            // AccountStatement indexes
            migrationBuilder.CreateIndex(name: "IX_AccountStatement_AccountId_Period", table: "AccountStatement", columns: new[] { "AccountId", "Period" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_AccountStatement_Status", table: "AccountStatement", column: "Status");
            migrationBuilder.CreateIndex(name: "IX_AccountStatement_GeneratedAt", table: "AccountStatement", column: "GeneratedAt");

            // StatementTransaction indexes
            migrationBuilder.CreateIndex(name: "IX_StatementTransaction_StatementId", table: "StatementTransaction", column: "StatementId");
            migrationBuilder.CreateIndex(name: "IX_StatementTransaction_TransactionId", table: "StatementTransaction", column: "TransactionId");
            migrationBuilder.CreateIndex(name: "IX_StatementTransaction_TransactionDate", table: "StatementTransaction", column: "TransactionDate");

            // Loan indexes
            migrationBuilder.CreateIndex(name: "IX_Loan_AccountId", table: "Loan", column: "AccountId");
            migrationBuilder.CreateIndex(name: "IX_Loan_Status", table: "Loan", column: "Status");
            migrationBuilder.CreateIndex(name: "IX_Loan_MaturityDate", table: "Loan", column: "MaturityDate");

            // LoanStatusHistory indexes
            migrationBuilder.CreateIndex(name: "IX_LoanStatusHistory_LoanId", table: "LoanStatusHistory", column: "LoanId");
            migrationBuilder.CreateIndex(name: "IX_LoanStatusHistory_ChangedByUserId", table: "LoanStatusHistory", column: "ChangedByUserId");

            // LoanPayment indexes
            migrationBuilder.CreateIndex(name: "IX_LoanPayment_LoanId", table: "LoanPayment", column: "LoanId");
            migrationBuilder.CreateIndex(name: "IX_LoanPayment_Status", table: "LoanPayment", column: "Status");
            migrationBuilder.CreateIndex(name: "IX_LoanPayment_PaymentDate", table: "LoanPayment", column: "PaymentDate");

            // LoanDocument indexes
            migrationBuilder.CreateIndex(name: "IX_LoanDocument_LoanId", table: "LoanDocument", column: "LoanId");
            migrationBuilder.CreateIndex(name: "IX_LoanDocument_DocumentType", table: "LoanDocument", column: "DocumentType");

            // Card indexes
            migrationBuilder.CreateIndex(name: "IX_Card_UserId", table: "Card", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_Card_Status", table: "Card", column: "Status");
            migrationBuilder.CreateIndex(name: "IX_Card_IsActive", table: "Card", column: "IsActive");

            // CardStatusHistory indexes
            migrationBuilder.CreateIndex(name: "IX_CardStatusHistory_CardId", table: "CardStatusHistory", column: "CardId");
            migrationBuilder.CreateIndex(name: "IX_CardStatusHistory_ChangedByUserId", table: "CardStatusHistory", column: "ChangedByUserId");

            // CardTransaction indexes
            migrationBuilder.CreateIndex(name: "IX_CardTransaction_CardId", table: "CardTransaction", column: "CardId");
            migrationBuilder.CreateIndex(name: "IX_CardTransaction_Status", table: "CardTransaction", column: "Status");
            migrationBuilder.CreateIndex(name: "IX_CardTransaction_TransactionDate", table: "CardTransaction", column: "TransactionDate");

            // CardAuthorization indexes
            migrationBuilder.CreateIndex(name: "IX_CardAuthorization_CardId", table: "CardAuthorization", column: "CardId");
            migrationBuilder.CreateIndex(name: "IX_CardAuthorization_Status", table: "CardAuthorization", column: "Status");
            migrationBuilder.CreateIndex(name: "IX_CardAuthorization_ExpiresAt", table: "CardAuthorization", column: "ExpiresAt");

            // CardStatement indexes
            migrationBuilder.CreateIndex(name: "IX_CardStatement_CardId_StatementPeriod", table: "CardStatement", columns: new[] { "CardId", "StatementPeriod" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_CardStatement_Status", table: "CardStatement", column: "Status");
            migrationBuilder.CreateIndex(name: "IX_CardStatement_DueDate", table: "CardStatement", column: "DueDate");

            // Biller indexes
            migrationBuilder.CreateIndex(name: "IX_Biller_Category", table: "Biller", column: "Category");
            migrationBuilder.CreateIndex(name: "IX_Biller_IsActive", table: "Biller", column: "IsActive");

            // BillerHealthCheck indexes
            migrationBuilder.CreateIndex(name: "IX_BillerHealthCheck_BillerId", table: "BillerHealthCheck", column: "BillerId");
            migrationBuilder.CreateIndex(name: "IX_BillerHealthCheck_CheckDate", table: "BillerHealthCheck", column: "CheckDate");

            // BillPayment indexes
            migrationBuilder.CreateIndex(name: "IX_BillPayment_CustomerId", table: "BillPayment", column: "CustomerId");
            migrationBuilder.CreateIndex(name: "IX_BillPayment_BillerId", table: "BillPayment", column: "BillerId");
            migrationBuilder.CreateIndex(name: "IX_BillPayment_Status", table: "BillPayment", column: "Status");
            migrationBuilder.CreateIndex(name: "IX_BillPayment_ScheduledDate", table: "BillPayment", column: "ScheduledDate");

            // BillPresentment indexes
            migrationBuilder.CreateIndex(name: "IX_BillPresentment_BillerId", table: "BillPresentment", column: "BillerId");
            migrationBuilder.CreateIndex(name: "IX_BillPresentment_CustomerId", table: "BillPresentment", column: "CustomerId");
            migrationBuilder.CreateIndex(name: "IX_BillPresentment_DueDate", table: "BillPresentment", column: "DueDate");
            migrationBuilder.CreateIndex(name: "IX_BillPresentment_IsScheduledForPayment", table: "BillPresentment", column: "IsScheduledForPayment");

            // PaymentReceipt indexes
            migrationBuilder.CreateIndex(name: "IX_PaymentReceipt_BillPaymentId", table: "PaymentReceipt", column: "BillPaymentId");

            // PaymentRetry indexes
            migrationBuilder.CreateIndex(name: "IX_PaymentRetry_BillPaymentId", table: "PaymentRetry", column: "BillPaymentId");
            migrationBuilder.CreateIndex(name: "IX_PaymentRetry_Status_NextRetryDate", table: "PaymentRetry", columns: new[] { "Status", "NextRetryDate" });

            // DepositProduct indexes
            migrationBuilder.CreateIndex(name: "IX_DepositProduct_IsActive", table: "DepositProduct", column: "IsActive");

            // InterestTier indexes
            migrationBuilder.CreateIndex(name: "IX_InterestTier_DepositProductId", table: "InterestTier", column: "DepositProductId");

            // FixedDeposit indexes
            migrationBuilder.CreateIndex(name: "IX_FixedDeposit_AccountId", table: "FixedDeposit", column: "AccountId");
            migrationBuilder.CreateIndex(name: "IX_FixedDeposit_DepositProductId", table: "FixedDeposit", column: "DepositProductId");
            migrationBuilder.CreateIndex(name: "IX_FixedDeposit_Status", table: "FixedDeposit", column: "Status");
            migrationBuilder.CreateIndex(name: "IX_FixedDeposit_MaturityDate", table: "FixedDeposit", column: "MaturityDate");

            // DepositTransaction indexes
            migrationBuilder.CreateIndex(name: "IX_DepositTransaction_FixedDepositId", table: "DepositTransaction", column: "FixedDepositId");
            migrationBuilder.CreateIndex(name: "IX_DepositTransaction_TransactionType", table: "DepositTransaction", column: "TransactionType");

            // DepositCertificate indexes
            migrationBuilder.CreateIndex(name: "IX_DepositCertificate_FixedDepositId", table: "DepositCertificate", column: "FixedDepositId");

            // MaturityNotice indexes
            migrationBuilder.CreateIndex(name: "IX_MaturityNotice_FixedDepositId", table: "MaturityNotice", column: "FixedDepositId");
            migrationBuilder.CreateIndex(name: "IX_MaturityNotice_MaturityDate", table: "MaturityNotice", column: "MaturityDate");
            migrationBuilder.CreateIndex(name: "IX_MaturityNotice_NotificationSent", table: "MaturityNotice", column: "NotificationSent");

            // BatchJob indexes
            migrationBuilder.CreateIndex(name: "IX_BatchJob_JobName_StartTime", table: "BatchJob", columns: new[] { "JobName", "StartTime" });
            migrationBuilder.CreateIndex(name: "IX_BatchJob_Status", table: "BatchJob", column: "Status");

            // AuditLog indexes
            migrationBuilder.CreateIndex(name: "IX_AuditLog_UserId", table: "AuditLog", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_AuditLog_EntityName_EntityId", table: "AuditLog", columns: new[] { "EntityName", "EntityId" });
            migrationBuilder.CreateIndex(name: "IX_AuditLog_Action", table: "AuditLog", column: "Action");
            migrationBuilder.CreateIndex(name: "IX_AuditLog_CreatedAt", table: "AuditLog", column: "CreatedAt");

            // Notification indexes
            migrationBuilder.CreateIndex(name: "IX_Notification_UserId", table: "Notification", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_Notification_Type", table: "Notification", column: "Type");
            migrationBuilder.CreateIndex(name: "IX_Notification_IsRead", table: "Notification", column: "IsRead");
            migrationBuilder.CreateIndex(name: "IX_Notification_SendDate", table: "Notification", column: "SendDate");

            // NotificationPreference indexes
            migrationBuilder.CreateIndex(name: "IX_NotificationPreference_UserId", table: "NotificationPreference", column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop all indexes and tables in reverse order

            // System Tables
            migrationBuilder.DropTable(name: "NotificationPreference");
            migrationBuilder.DropTable(name: "Notification");
            migrationBuilder.DropTable(name: "AuditLog");
            migrationBuilder.DropTable(name: "BatchJob");

            // Deposit Tables
            migrationBuilder.DropTable(name: "MaturityNotice");
            migrationBuilder.DropTable(name: "DepositCertificate");
            migrationBuilder.DropTable(name: "DepositTransaction");
            migrationBuilder.DropTable(name: "FixedDeposit");
            migrationBuilder.DropTable(name: "InterestTier");
            migrationBuilder.DropTable(name: "DepositProduct");

            // Bill Payment Tables
            migrationBuilder.DropTable(name: "PaymentRetry");
            migrationBuilder.DropTable(name: "PaymentReceipt");
            migrationBuilder.DropTable(name: "BillPresentment");
            migrationBuilder.DropTable(name: "BillPayment");
            migrationBuilder.DropTable(name: "BillerHealthCheck");
            migrationBuilder.DropTable(name: "Biller");

            // Card Tables
            migrationBuilder.DropTable(name: "CardStatement");
            migrationBuilder.DropTable(name: "CardAuthorization");
            migrationBuilder.DropTable(name: "CardTransaction");
            migrationBuilder.DropTable(name: "CardStatusHistory");
            migrationBuilder.DropTable(name: "Card");

            // Loan Tables
            migrationBuilder.DropTable(name: "LoanDocument");
            migrationBuilder.DropTable(name: "LoanPayment");
            migrationBuilder.DropTable(name: "LoanStatusHistory");
            migrationBuilder.DropTable(name: "Loan");

            // Statement Tables
            migrationBuilder.DropTable(name: "StatementTransaction");
            migrationBuilder.DropTable(name: "AccountStatement");

            // Transaction & Payment Tables
            migrationBuilder.DropTable(name: "RecurringPaymentExecution");
            migrationBuilder.DropTable(name: "PaymentTemplate");
            migrationBuilder.DropTable(name: "Beneficiary");
            migrationBuilder.DropTable(name: "RecurringPayment");
        }
    }
}
