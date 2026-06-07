using Bank.Application.DTOs;
using Bank.Application.DTOs.Statement.Core;
using Bank.Application.DTOs.Statement.Search;
using Bank.Application.DTOs.Statement.Analytics;
using Bank.Application.DTOs.Statement.Delivery;
using Bank.Application.DTOs.Statement.Summary;
using Bank.Application.DTOs.Statement.Transaction;
using Bank.Application.Interfaces;
using Bank.Application.Services.Shared;
using Bank.Domain.Entities;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Bank.Application.Services;

/// <summary>
/// Service for generating and managing account statements
/// </summary>
public class StatementService : IStatementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStatementGenerator _statementGenerator;
    private readonly IAuditLogService _auditLogService;
    private readonly IEmailService _emailService;
    private readonly ILogger<StatementService> _logger;
    private readonly IConfiguration _configuration;

    public StatementService(
        IUnitOfWork unitOfWork,
        IStatementGenerator statementGenerator,
        IAuditLogService auditLogService,
        IEmailService emailService,
        ILogger<StatementService> logger,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _statementGenerator = statementGenerator;
        _auditLogService = auditLogService;
        _emailService = emailService;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<StatementGenerationResult> GenerateStatementAsync(GenerateStatementRequest request, Guid requestedByUserId)
    {
        try
        {
            // Validate request
            var (isValid, errors) = await ValidateStatementRequestAsync(request);
            if (!isValid)
            {
                return new StatementGenerationResult
                {
                    Success = false,
                    Message = "Invalid statement request",
                    Errors = errors
                };
            }

            // Get account
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(request.AccountId);
            if (account == null)
            {
                return new StatementGenerationResult
                {
                    Success = false,
                    Message = "Account not found"
                };
            }

            // Create statement entity
            var statement = await CreateStatementEntityAsync(request, account, requestedByUserId);
            
            // Get transactions for the period
            var transactions = await GetTransactionsForPeriodAsync(request.AccountId, request.StartDate, request.EndDate, request);
            
            // Add transactions to statement
            await AddTransactionsToStatementAsync(statement, transactions);
            
            // Calculate statement statistics
            statement.CalculateStatistics();
            
            // Generate statement number
            statement.GenerateStatementNumber();
            
            // Save statement to database
            await _unitOfWork.Repository<AccountStatement>().AddAsync(statement);
            await _unitOfWork.SaveChangesAsync();

            // Generate statement file
            var (content, fileName) = await GenerateStatementFileAsync(statement, request.Format);
            
            // Save file and update statement
            var filePath = await SaveStatementFileAsync(content, fileName, statement.Id);
            statement.FilePath = filePath;
            statement.FileName = fileName;
            statement.FileSizeBytes = content.Length;
            statement.FileHash = CalculateFileHash(content);
            statement.Status = StatementStatus.Generated;
            
            _unitOfWork.Repository<AccountStatement>().Update(statement);
            await _unitOfWork.SaveChangesAsync();

            // Deliver statement if requested
            if (request.DeliveryMethod != StatementDeliveryMethod.Download)
            {
                await DeliverStatementAsync(statement.Id, request.DeliveryMethod, request.EmailAddress ?? account.User.Email);
            }

            await _auditLogService.LogAsync("Statement Generated", 
                $"Statement {statement.StatementNumber} generated for account {account.AccountNumber}", requestedByUserId);

            return new StatementGenerationResult
            {
                Success = true,
                Message = "Statement generated successfully",
                StatementId = statement.Id,
                StatementNumber = statement.StatementNumber,
                FileName = fileName,
                FilePath = filePath,
                FileSizeBytes = content.Length,
                GeneratedDate = statement.CreatedAt
            };
        }
        catch (Exception ex)
        {
            SecureLoggingService.LogErrorSecurely(_logger, ex, "statement generation");
            return new StatementGenerationResult
            {
                Success = false,
                Message = "An error occurred while generating the statement"
            };
        }
    }

    public async Task<StatementGenerationResult> GenerateConsolidatedStatementAsync(ConsolidatedStatementRequest request, Guid requestedByUserId)
    {
        try
        {
            var statements = new List<AccountStatement>();
            
            // Generate individual statements for each account
            foreach (var accountId in request.AccountIds)
            {
                var individualRequest = new GenerateStatementRequest
                {
                    AccountId = accountId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Format = request.Format,
                    DeliveryMethod = StatementDeliveryMethod.Download, // Don't deliver individual statements
                    IncludeTransactionDetails = request.IncludeTransactionDetails
                };

                var result = await GenerateStatementAsync(individualRequest, requestedByUserId);
                if (result.Success && result.StatementId.HasValue)
                {
                    var statement = await _unitOfWork.Repository<AccountStatement>().GetByIdAsync(result.StatementId.Value);
                    if (statement != null)
                    {
                        statements.Add(statement);
                    }
                }
            }

            if (!statements.Any())
            {
                return new StatementGenerationResult
                {
                    Success = false,
                    Message = "No statements could be generated for the specified accounts"
                };
            }

            // Generate consolidated statement file
            var (content, fileName) = await _statementGenerator.GenerateConsolidatedStatementAsync(statements, request);
            
            // Create consolidated statement record
            var consolidatedStatement = new AccountStatement
            {
                AccountId = statements.First().AccountId, // Use first account as primary
                StatementDate = DateTime.UtcNow,
                PeriodStartDate = request.StartDate,
                PeriodEndDate = request.EndDate,
                Format = request.Format,
                DeliveryMethod = request.DeliveryMethod,
                RequestedByUserId = requestedByUserId,
                RequestedDate = DateTime.UtcNow,
                Status = StatementStatus.Generated,
                FileName = fileName,
                FileSizeBytes = content.Length,
                FileHash = CalculateFileHash(content)
            };

            consolidatedStatement.GenerateStatementNumber();
            
            await _unitOfWork.Repository<AccountStatement>().AddAsync(consolidatedStatement);
            await _unitOfWork.SaveChangesAsync();

            // Save consolidated file
            var filePath = await SaveStatementFileAsync(content, fileName, consolidatedStatement.Id);
            consolidatedStatement.FilePath = filePath;
            
            _unitOfWork.Repository<AccountStatement>().Update(consolidatedStatement);
            await _unitOfWork.SaveChangesAsync();

            // Deliver consolidated statement
            if (request.DeliveryMethod != StatementDeliveryMethod.Download)
            {
                await DeliverStatementAsync(consolidatedStatement.Id, request.DeliveryMethod, request.EmailAddress ?? "");
            }

            await _auditLogService.LogAsync("Consolidated Statement Generated", 
                $"Consolidated statement generated for {request.AccountIds.Count} accounts", requestedByUserId);

            return new StatementGenerationResult
            {
                Success = true,
                Message = "Consolidated statement generated successfully",
                StatementId = consolidatedStatement.Id,
                StatementNumber = consolidatedStatement.StatementNumber,
                FileName = fileName,
                FilePath = filePath,
                FileSizeBytes = content.Length,
                GeneratedDate = consolidatedStatement.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating consolidated statement");
            return new StatementGenerationResult
            {
                Success = false,
                Message = "An error occurred while generating the consolidated statement"
            };
        }
    }

    public async Task<StatementDto?> GetStatementByIdAsync(Guid statementId)
    {
        try
        {
            var statement = await _unitOfWork.Repository<AccountStatement>()
                .Query()
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.Id == statementId);

            return statement != null ? MapToDto(statement) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statement {StatementId}", statementId);
            return null;
        }
    }

    public async Task<StatementSearchResult> SearchStatementsAsync(StatementSearchCriteria criteria)
    {
        try
        {
            var baseQuery = _unitOfWork.Repository<AccountStatement>()
                .Query()
                .Include(s => s.Account);

            IQueryable<AccountStatement> query = baseQuery;

            // Apply filters
            if (criteria.AccountId.HasValue)
                query = query.Where(s => s.AccountId == criteria.AccountId.Value);

            if (criteria.FromDate.HasValue)
                query = query.Where(s => s.StatementDate >= criteria.FromDate.Value);

            if (criteria.ToDate.HasValue)
                query = query.Where(s => s.StatementDate <= criteria.ToDate.Value);

            if (criteria.Status.HasValue)
                query = query.Where(s => s.Status == criteria.Status.Value);

            if (criteria.Format.HasValue)
                query = query.Where(s => s.Format == criteria.Format.Value);

            if (criteria.IsDelivered.HasValue)
                query = query.Where(s => s.IsDelivered == criteria.IsDelivered.Value);

            var totalCount = await query.CountAsync();
            var statements = await query
                .OrderByDescending(s => s.StatementDate)
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync();

            return new StatementSearchResult
            {
                Statements = statements.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / criteria.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching statements");
            return new StatementSearchResult();
        }
    }

    public async Task<List<StatementDto>> GetAccountStatementsAsync(Guid accountId, int? limit = null)
    {
        try
        {
            var baseQuery = _unitOfWork.Repository<AccountStatement>()
                .Query()
                .Include(s => s.Account);

            IQueryable<AccountStatement> query = baseQuery
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.StatementDate);

            if (limit.HasValue)
                query = query.Take(limit.Value);

            var statements = await query.ToListAsync();
            return statements.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            SecureLoggingService.LogErrorSecurely(_logger, ex, "retrieving statements");
            return new List<StatementDto>();
        }
    }

    public async Task<StatementSummary> GetStatementSummaryAsync(Guid accountId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                return new StatementSummary { AccountId = accountId };
            }

            var transactions = await GetTransactionsForPeriodAsync(accountId, startDate, endDate, new GenerateStatementRequest());
            
            var summary = new StatementSummary
            {
                AccountId = accountId,
                AccountNumber = account.AccountNumber,
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TransactionCount = transactions.Count,
                TotalIncome = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount),
                TotalExpenses = Math.Abs(transactions.Where(t => t.Amount < 0).Sum(t => t.Amount))
            };

            // Calculate opening and closing balances
            summary.OpeningBalance = await GetBalanceAtDate(accountId, startDate);
            summary.ClosingBalance = await GetBalanceAtDate(accountId, endDate);
            summary.NetChange = summary.ClosingBalance - summary.OpeningBalance;

            // Generate category breakdown
            summary.CategoryBreakdown = transactions
                .GroupBy(t => GetTransactionCategory(t))
                .Select(g => new TransactionCategorySummary
                {
                    Category = g.Key,
                    TransactionCount = g.Count(),
                    TotalAmount = g.Sum(t => Math.Abs(t.Amount)),
                    Percentage = summary.TotalExpenses > 0 ? (g.Sum(t => Math.Abs(t.Amount)) / summary.TotalExpenses) * 100 : 0
                })
                .OrderByDescending(c => c.TotalAmount)
                .ToList();

            // Generate monthly breakdown
            summary.MonthlyBreakdown = transactions
                .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                .Select(g => new MonthlyTransactionSummary
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    TransactionCount = g.Count(),
                    TotalDebits = Math.Abs(g.Where(t => t.Amount < 0).Sum(t => t.Amount)),
                    TotalCredits = g.Where(t => t.Amount > 0).Sum(t => t.Amount),
                    NetAmount = g.Sum(t => t.Amount)
                })
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToList();

            return summary;
        }
        catch (Exception ex)
        {
            SecureLoggingService.LogErrorSecurely(_logger, ex, "generating statement summary");
            return new StatementSummary { AccountId = accountId };
        }
    }

    public async Task<(byte[] Content, string FileName, string ContentType)> DownloadStatementAsync(Guid statementId)
    {
        try
        {
            var statement = await _unitOfWork.Repository<AccountStatement>().GetByIdAsync(statementId);
            if (statement == null || string.IsNullOrEmpty(statement.FilePath))
            {
                throw new FileNotFoundException("Statement file not found");
            }

            var content = await File.ReadAllBytesAsync(statement.FilePath);
            var contentType = _statementGenerator.GetContentType(statement.Format);

            return (content, statement.FileName ?? "statement", contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading statement {StatementId}", statementId);
            throw;
        }
    }

    public async Task<bool> DeliverStatementAsync(Guid statementId, StatementDeliveryMethod deliveryMethod, string deliveryAddress)
    {
        try
        {
            var statement = await _unitOfWork.Repository<AccountStatement>()
                .Query()
                .Include(s => s.Account)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(s => s.Id == statementId);

            if (statement == null)
                return false;

            switch (deliveryMethod)
            {
                case StatementDeliveryMethod.Email:
                    return await DeliverViaEmailAsync(statement, deliveryAddress);
                
                case StatementDeliveryMethod.SMS:
                    return await DeliverViaSmsAsync(statement, deliveryAddress);
                
                default:
                    return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delivering statement {StatementId}", statementId);
            return false;
        }
    }

    public async Task<StatementDeliveryStatus> GetDeliveryStatusAsync(Guid statementId)
    {
        try
        {
            var statement = await _unitOfWork.Repository<AccountStatement>().GetByIdAsync(statementId);
            if (statement == null)
            {
                return new StatementDeliveryStatus { StatementId = statementId };
            }

            return new StatementDeliveryStatus
            {
                StatementId = statementId,
                IsDelivered = statement.IsDelivered,
                DeliveredDate = statement.DeliveredDate,
                DeliveryReference = statement.DeliveryReference,
                DeliveryMethod = statement.DeliveryMethod
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting delivery status for statement {StatementId}", statementId);
            return new StatementDeliveryStatus { StatementId = statementId };
        }
    }

    public async Task<StatementGenerationResult> RegenerateStatementAsync(Guid statementId, Guid requestedByUserId)
    {
        try
        {
            var existingStatement = await _unitOfWork.Repository<AccountStatement>().GetByIdAsync(statementId);
            if (existingStatement == null)
            {
                return new StatementGenerationResult
                {
                    Success = false,
                    Message = "Statement not found"
                };
            }

            var request = new GenerateStatementRequest
            {
                AccountId = existingStatement.AccountId,
                StartDate = existingStatement.PeriodStartDate,
                EndDate = existingStatement.PeriodEndDate,
                Format = existingStatement.Format,
                DeliveryMethod = existingStatement.DeliveryMethod
            };

            return await GenerateStatementAsync(request, requestedByUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating statement {StatementId}", statementId);
            return new StatementGenerationResult
            {
                Success = false,
                Message = "An error occurred while regenerating the statement"
            };
        }
    }

    public async Task<bool> CancelStatementGenerationAsync(Guid statementId, Guid cancelledByUserId)
    {
        try
        {
            var statement = await _unitOfWork.Repository<AccountStatement>().GetByIdAsync(statementId);
            if (statement == null)
                return false;

            if (statement.Status == StatementStatus.Generated || statement.Status == StatementStatus.Delivered)
                return false; // Cannot cancel completed statements

            statement.Status = StatementStatus.Cancelled;
            _unitOfWork.Repository<AccountStatement>().Update(statement);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Statement Cancelled", 
                $"Statement {statement.StatementNumber} cancelled", cancelledByUserId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling statement {StatementId}", statementId);
            return false;
        }
    }

    public async Task<List<StatementTemplate>> GetAvailableTemplatesAsync()
    {
        // Feature not yet implemented - requires template management system
        throw new NotImplementedException("Statement template retrieval is not yet implemented. Requires template management system implementation.");
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateStatementRequestAsync(GenerateStatementRequest request)
    {
        var errors = new List<string>();

        if (request.AccountId == Guid.Empty)
            errors.Add("Account ID is required");

        if (request.StartDate >= request.EndDate)
            errors.Add("Start date must be before end date");

        if (request.EndDate > DateTime.UtcNow)
            errors.Add("End date cannot be in the future");

        if ((request.EndDate - request.StartDate).TotalDays > 365)
            errors.Add("Statement period cannot exceed 365 days");

        // Check if account exists
        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(request.AccountId);
        if (account == null)
            errors.Add("Account not found");

        return (errors.Count == 0, errors);
    }

    #region Private Helper Methods

    private async Task<AccountStatement> CreateStatementEntityAsync(GenerateStatementRequest request, Account account, Guid requestedByUserId)
    {
        var statementSequence = await GetNextStatementSequenceAsync(request.AccountId, request.EndDate);
        
        return new AccountStatement
        {
            AccountId = request.AccountId,
            Account = account,
            StatementDate = DateTime.UtcNow,
            PeriodStartDate = request.StartDate,
            PeriodEndDate = request.EndDate,
            StatementSequence = statementSequence,
            Format = request.Format,
            DeliveryMethod = request.DeliveryMethod,
            RequestedByUserId = requestedByUserId,
            RequestedDate = DateTime.UtcNow,
            RequestReason = request.RequestReason,
            Status = StatementStatus.Generating,
            OpeningBalance = await GetBalanceAtDate(request.AccountId, request.StartDate),
            ClosingBalance = await GetBalanceAtDate(request.AccountId, request.EndDate)
        };
    }

    private async Task<List<Transaction>> GetTransactionsForPeriodAsync(Guid accountId, DateTime startDate, DateTime endDate, GenerateStatementRequest request)
    {
        var query = _unitOfWork.Repository<Transaction>()
            .Query()
            .Where(t => (t.FromAccountId == accountId || t.ToAccountId == accountId) &&
                       t.CreatedAt >= startDate &&
                       t.CreatedAt <= endDate &&
                       t.Status == TransactionStatus.Completed);

        // Apply filters
        if (request.FilterByTransactionTypes?.Any() == true)
            query = query.Where(t => request.FilterByTransactionTypes.Contains(t.Type));

        if (request.MinAmount.HasValue)
            query = query.Where(t => Math.Abs(t.Amount) >= request.MinAmount.Value);

        if (request.MaxAmount.HasValue)
            query = query.Where(t => Math.Abs(t.Amount) <= request.MaxAmount.Value);

        // Enhanced filtering for description (Requirement 3.7)
        if (!string.IsNullOrWhiteSpace(request.FilterByDescription))
            query = query.Where(t => t.Description.Contains(request.FilterByDescription));

        // Enhanced filtering for categories (Requirement 3.7)
        if (request.FilterByCategories?.Any() == true)
        {
            query = query.Where(t => request.FilterByCategories.Any(category => 
                GetTransactionCategoryFromDescription(t.Description).Equals(category, StringComparison.OrdinalIgnoreCase)));
        }

        return await query.OrderBy(t => t.CreatedAt).ToListAsync();
    }

    private async Task AddTransactionsToStatementAsync(AccountStatement statement, List<Transaction> transactions)
    {
        var runningBalance = statement.OpeningBalance;
        
        foreach (var transaction in transactions)
        {
            var amount = transaction.FromAccountId == statement.AccountId ? -transaction.Amount : transaction.Amount;
            runningBalance += amount;

            var statementTransaction = new StatementTransaction
            {
                StatementId = statement.Id,
                TransactionId = transaction.Id,
                Transaction = transaction,
                TransactionDate = transaction.CreatedAt,
                Description = transaction.Description,
                Reference = transaction.Reference,
                Amount = amount,
                RunningBalance = runningBalance,
                Type = transaction.Type,
                Status = transaction.Status,
                Category = GetTransactionCategory(transaction)
            };

            statement.Transactions.Add(statementTransaction);
        }
    }

    private async Task<(byte[] Content, string FileName)> GenerateStatementFileAsync(AccountStatement statement, StatementFormat format)
    {
        return format switch
        {
            StatementFormat.PDF => await _statementGenerator.GeneratePdfStatementAsync(statement),
            StatementFormat.CSV => await _statementGenerator.GenerateCsvStatementAsync(statement),
            StatementFormat.Excel => await _statementGenerator.GenerateExcelStatementAsync(statement),
            StatementFormat.HTML => await _statementGenerator.GenerateHtmlStatementAsync(statement),
            StatementFormat.JSON => await _statementGenerator.GenerateJsonStatementAsync(statement),
            _ => throw new NotSupportedException($"Statement format {format} is not supported")
        };
    }

    private async Task<string> SaveStatementFileAsync(byte[] content, string fileName, Guid statementId)
    {
        var basePath = _configuration["FileStorage:BasePath"] ?? Path.Combine(AppContext.BaseDirectory, "storage");
        var directory = Path.Combine(basePath, "statements", DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"));
        Directory.CreateDirectory(directory);
        
        var filePath = Path.Combine(directory, $"{statementId}_{fileName}");
        await File.WriteAllBytesAsync(filePath, content);
        
        return filePath;
    }

    private string CalculateFileHash(byte[] content)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(content);
        return Convert.ToBase64String(hash);
    }

    private async Task<decimal> GetBalanceAtDate(Guid accountId, DateTime date)
    {
        // This is a simplified calculation - in a real system, you'd need to calculate based on transaction history
        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
        return account?.Balance ?? 0;
    }

    private async Task<int> GetNextStatementSequenceAsync(Guid accountId, DateTime statementDate)
    {
        var year = statementDate.Year;
        var month = statementDate.Month;
        
        var lastSequence = await _unitOfWork.Repository<AccountStatement>()
            .Query()
            .Where(s => s.AccountId == accountId && 
                       s.StatementDate.Year == year && 
                       s.StatementDate.Month == month)
            .MaxAsync(s => (int?)s.StatementSequence) ?? 0;

        return lastSequence + 1;
    }

    private string GetTransactionCategory(Transaction transaction)
    {
        return GetTransactionCategoryFromDescription(transaction.Description);
    }

    private string GetTransactionCategoryFromDescription(string description)
    {
        // Simple categorization based on description - in a real system, this would be more sophisticated
        var desc = description.ToLower();
        
        if (desc.Contains("salary") || desc.Contains("payroll"))
            return "Income";
        if (desc.Contains("grocery") || desc.Contains("food"))
            return "Food & Dining";
        if (desc.Contains("gas") || desc.Contains("fuel"))
            return "Transportation";
        if (desc.Contains("utility") || desc.Contains("electric") || desc.Contains("water"))
            return "Utilities";
        if (desc.Contains("rent") || desc.Contains("mortgage"))
            return "Housing";
        if (desc.Contains("medical") || desc.Contains("pharmacy"))
            return "Healthcare";
        if (desc.Contains("fee") || desc.Contains("charge"))
            return "Fees & Charges";
        if (desc.Contains("interest"))
            return "Interest";
        if (desc.Contains("transfer"))
            return "Transfers";
        if (desc.Contains("atm") || desc.Contains("withdrawal"))
            return "ATM & Cash";
        if (desc.Contains("shopping") || desc.Contains("retail"))
            return "Shopping";
        if (desc.Contains("entertainment") || desc.Contains("movie"))
            return "Entertainment";
        
        return "Other";
    }

    private async Task<bool> DeliverViaEmailAsync(AccountStatement statement, string emailAddress)
    {
        try
        {
            if (string.IsNullOrEmpty(statement.FilePath))
                return false;

            var content = await File.ReadAllBytesAsync(statement.FilePath);
            var subject = $"Account Statement - {statement.StatementNumber}";
            var body = $"Please find attached your account statement for the period {statement.PeriodStartDate:yyyy-MM-dd} to {statement.PeriodEndDate:yyyy-MM-dd}.";

            await _emailService.SendEmailWithAttachmentAsync(emailAddress, subject, body, content, statement.FileName ?? "statement.pdf");
            
            statement.MarkAsDelivered($"EMAIL_{DateTime.UtcNow:yyyyMMddHHmmss}");
            _unitOfWork.Repository<AccountStatement>().Update(statement);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delivering statement via email");
            return false;
        }
    }

    private async Task<bool> DeliverViaSmsAsync(AccountStatement statement, string phoneNumber)
    {
        try
        {
            // For SMS delivery, we'd typically send a download link rather than the file itself
            var downloadLink = $"https://bankapp.com/statements/download/{statement.Id}";
            var message = $"Your account statement {statement.StatementNumber} is ready. Download: {downloadLink}";

            // This would integrate with SMS service
            await Task.Delay(100); // Simulate SMS sending
            
            statement.MarkAsDelivered($"SMS_{DateTime.UtcNow:yyyyMMddHHmmss}");
            _unitOfWork.Repository<AccountStatement>().Update(statement);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delivering statement via SMS");
            return false;
        }
    }

    private static StatementDto MapToDto(AccountStatement statement)
    {
        return new StatementDto
        {
            Id = statement.Id,
            AccountId = statement.AccountId,
            AccountNumber = statement.Account?.AccountNumber ?? "",
            AccountHolderName = statement.Account?.AccountHolderName ?? "",
            StatementDate = statement.StatementDate,
            PeriodStartDate = statement.PeriodStartDate,
            PeriodEndDate = statement.PeriodEndDate,
            StatementNumber = statement.StatementNumber,
            OpeningBalance = statement.OpeningBalance,
            ClosingBalance = statement.ClosingBalance,
            AverageBalance = statement.AverageBalance,
            TotalTransactions = statement.TotalTransactions,
            TotalDebits = statement.TotalDebits,
            TotalCredits = statement.TotalCredits,
            TotalFees = statement.TotalFees,
            InterestEarned = statement.InterestEarned,
            Status = statement.Status,
            Format = statement.Format,
            FileName = statement.FileName,
            FileSizeBytes = statement.FileSizeBytes,
            IsDelivered = statement.IsDelivered,
            DeliveredDate = statement.DeliveredDate,
            CreatedAt = statement.CreatedAt
        };
    }

    #endregion
}
