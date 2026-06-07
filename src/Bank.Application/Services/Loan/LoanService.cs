using AutoMapper;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Application.Helpers.Loan;
using Bank.Application.Helpers.Shared;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bank.Application.Services;

/// <summary>
/// Service for loan management operations
/// </summary>
public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoanService> _logger;
    private readonly ITransactionService _transactionService;
    private readonly ILoanInterestCalculationService _loanInterestCalculationService;
    private readonly IMapper _mapper;

    public LoanService(
        ILoanRepository loanRepository,
        IUserRepository userRepository,
        ILogger<LoanService> logger,
        ITransactionService transactionService,
        ILoanInterestCalculationService loanInterestCalculationService,
        IMapper mapper)
    {
        _loanRepository = loanRepository;
        _userRepository = userRepository;
        _logger = logger;
        _transactionService = transactionService;
        _loanInterestCalculationService = loanInterestCalculationService;
        _mapper = mapper;
    }

    public async Task<LoanApplicationResult> SubmitApplicationAsync(Guid customerId, LoanApplicationRequest request)
    {
        try
        {
            // Validate customer exists
            var customer = await _userRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return new LoanApplicationResult
                {
                    IsSuccess = false,
                    Message = "Customer not found"
                };
            }

            // Generate loan number
            var loanNumber = await _loanRepository.GenerateNextLoanNumberAsync();

            // Get loan type configuration and set appropriate interest rate
            var config = await _loanInterestCalculationService.GetLoanTypeConfigurationAsync(request.Type);
            var interestRate = await _loanInterestCalculationService.GetInterestRateForLoanTypeAsync(
                request.Type, 650, request.RequestedAmount); // Default credit score, will be updated after scoring

            // Create loan entity
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                LoanNumber = loanNumber,
                CustomerId = customerId,
                Type = request.Type,
                RequestedAmount = request.RequestedAmount,
                PrincipalAmount = request.RequestedAmount, // Will be updated after approval
                TermInMonths = request.TermInMonths,
                InterestRate = interestRate,
                Purpose = request.Purpose,
                Status = LoanStatus.UnderReview,
                ApplicationDate = DateTime.UtcNow,
                OutstandingBalance = 0,
                CalculationMethod = config.DefaultCalculationMethod
            };

            // Add status history
            loan.StatusHistory.Add(new LoanStatusHistory
            {
                Id = Guid.NewGuid(),
                LoanId = loan.Id,
                FromStatus = LoanStatus.UnderReview, // Initial status
                ToStatus = LoanStatus.UnderReview,
                StatusChangeDate = DateTime.UtcNow,
                Reason = "Loan application submitted",
                IsSystemGenerated = true
            });

            await _loanRepository.AddAsync(loan);

            _logger.LogInformation("Loan application submitted: {LoanNumber} for customer {CustomerId}", 
                loanNumber, customerId);

            return new LoanApplicationResult
            {
                IsSuccess = true,
                LoanId = loan.Id,
                LoanNumber = loanNumber,
                Status = LoanStatus.UnderReview,
                Message = "Loan application submitted successfully",
                ApplicationDate = loan.ApplicationDate,
                RequiredDocuments = GetRequiredDocuments(request.Type)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting loan application for customer {CustomerId}", customerId);
            return new LoanApplicationResult
            {
                IsSuccess = false,
                Message = "An error occurred while processing your application"
            };
        }
    }

    public async Task<CreditScoreResult> PerformCreditScoringAsync(Guid customerId, Guid loanId)
    {
        try
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null || loan.CustomerId != customerId)
            {
                return new CreditScoreResult
                {
                    IsSuccess = false,
                    RiskAssessment = "Loan not found"
                };
            }

            // Simulate credit scoring (in real implementation, this would call external credit bureau)
            var creditScore = await SimulateCreditScoringAsync(customerId);
            var scoreRange = GetCreditScoreRange(creditScore);
            var interestRate = CalculateInterestRateFromScore(creditScore, loan.Type);
            var maxLoanAmount = CalculateMaxLoanAmount(creditScore, loan.RequestedAmount);

            // Update loan with credit score
            loan.CreditScore = creditScore;
            loan.CreditScoreRange = scoreRange;
            loan.CreditScoringDate = DateTime.UtcNow;
            loan.InterestRate = interestRate;

            await _loanRepository.UpdateAsync(loan);

            _logger.LogInformation("Credit scoring completed for loan {LoanId}: range {ScoreRange}", 
                loanId, scoreRange);

            return new CreditScoreResult
            {
                IsSuccess = true,
                CreditScore = creditScore,
                ScoreRange = scoreRange,
                RiskAssessment = GetRiskAssessment(scoreRange),
                RecommendedInterestRate = interestRate,
                MaxLoanAmount = maxLoanAmount,
                ScoringDate = DateTime.UtcNow,
                RiskFactors = GetRiskFactors(creditScore)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing credit scoring for loan {LoanId}", loanId);
            return new CreditScoreResult
            {
                IsSuccess = false,
                RiskAssessment = "Error occurred during credit scoring"
            };
        }
    }

    public async Task<LoanApprovalResult> ProcessApprovalAsync(Guid loanId, ApprovalDecision decision, Guid approvedBy)
    {
        try
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);

            if (loan == null)
            {
                return new LoanApprovalResult
                {
                    IsSuccess = false,
                    Message = "Loan not found"
                };
            }

            if (loan.Status != LoanStatus.UnderReview)
            {
                return new LoanApprovalResult
                {
                    IsSuccess = false,
                    Message = "Loan is not in a state that can be approved or rejected"
                };
            }

            var oldStatus = loan.Status;
            
            if (decision.IsApproved)
            {
                loan.Status = LoanStatus.Approved;
                loan.ApprovalDate = DateTime.UtcNow;
                loan.ApprovedBy = approvedBy;
                
                if (decision.ApprovedAmount.HasValue)
                    loan.PrincipalAmount = decision.ApprovedAmount.Value;
                if (decision.InterestRate.HasValue)
                    loan.InterestRate = decision.InterestRate.Value;
                if (decision.ApprovedTermInMonths.HasValue)
                    loan.TermInMonths = decision.ApprovedTermInMonths.Value;

                // Use enhanced interest calculation service for monthly payment
                loan.MonthlyPaymentAmount = await _loanInterestCalculationService.CalculateMonthlyPaymentAsync(
                    loan.PrincipalAmount, loan.InterestRate, loan.TermInMonths, loan.CalculationMethod);

                loan.GenerateRepaymentSchedule();
            }
            else
            {
                loan.Status = LoanStatus.Rejected;
                loan.RejectionReason = decision.RejectionReason;
            }

            // Add status history
            loan.StatusHistory.Add(new LoanStatusHistory
            {
                Id = Guid.NewGuid(),
                LoanId = loanId,
                FromStatus = oldStatus,
                ToStatus = loan.Status,
                StatusChangeDate = DateTime.UtcNow,
                ChangedBy = approvedBy,
                Reason = decision.IsApproved ? "Loan approved" : decision.RejectionReason,
                IsSystemGenerated = false
            });

            await _loanRepository.UpdateAsync(loan);

            _logger.LogInformation("Loan {LoanId} {Status} by user {ApprovedBy}", 
                loanId, decision.IsApproved ? "approved" : "rejected", approvedBy);

            return new LoanApprovalResult
            {
                IsSuccess = true,
                Message = decision.IsApproved ? "Loan approved successfully" : "Loan application rejected",
                NewStatus = loan.Status,
                ApprovedAmount = decision.IsApproved ? loan.PrincipalAmount : null,
                InterestRate = decision.IsApproved ? loan.InterestRate : null,
                MonthlyPayment = decision.IsApproved ? loan.MonthlyPaymentAmount : null,
                ApprovalDate = loan.ApprovalDate,
                NextSteps = decision.IsApproved ? new List<string> { "Complete documentation", "Schedule disbursement" } : new List<string>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing loan approval for loan {LoanId}", loanId);
            return new LoanApprovalResult
            {
                IsSuccess = false,
                Message = "An error occurred while processing the approval"
            };
        }
    }

    public async Task<DisbursementResult> DisburseLoanAsync(Guid loanId, Guid disbursedBy)
    {
        try
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);

            if (loan == null)
            {
                return new DisbursementResult
                {
                    IsSuccess = false,
                    Message = "Loan not found"
                };
            }

            if (!loan.IsEligibleForDisbursement())
            {
                return new DisbursementResult
                {
                    IsSuccess = false,
                    Message = "Loan is not eligible for disbursement"
                };
            }

            // For now, we'll simulate the account lookup and transaction creation
            // In a real implementation, this would use the account service
            
            // Create disbursement transaction
            var transactionDto = new CreateTransactionRequest
            {
                FromAccountId = Guid.Empty, // System account - use Empty instead of null
                ToAccountId = Guid.NewGuid(), // Placeholder - would be customer's account
                Amount = loan.PrincipalAmount,
                Description = $"Loan disbursement - {loan.LoanNumber}",
                Type = TransactionType.ACH
            };

            var transaction = await _transactionService.CreateTransactionAsync(transactionDto);
            
            if (transaction == null)
            {
                return new DisbursementResult
                {
                    IsSuccess = false,
                    Message = "Failed to process disbursement transaction"
                };
            }

            // Update loan status
            var oldStatus = loan.Status;
            loan.Status = LoanStatus.Disbursed;
            loan.DisbursementDate = DateTime.UtcNow;
            loan.OutstandingBalance = loan.PrincipalAmount;
            loan.GenerateRepaymentSchedule();

            // Add status history
            loan.StatusHistory.Add(new LoanStatusHistory
            {
                Id = Guid.NewGuid(),
                LoanId = loanId,
                FromStatus = oldStatus,
                ToStatus = loan.Status,
                StatusChangeDate = DateTime.UtcNow,
                ChangedBy = disbursedBy,
                Reason = "Loan disbursed",
                SystemReference = transaction.Id.ToString(),
                IsSystemGenerated = false
            });

            await _loanRepository.UpdateAsync(loan);

            _logger.LogInformation("Loan {LoanId} disbursed successfully. Amount: {Amount}", 
                loanId, loan.PrincipalAmount);

            return new DisbursementResult
            {
                IsSuccess = true,
                Message = "Loan disbursed successfully",
                DisbursedAmount = loan.PrincipalAmount,
                TransactionReference = transaction.Id.ToString(),
                DisbursementDate = loan.DisbursementDate.Value,
                FirstPaymentDueDate = loan.NextPaymentDueDate ?? DateTime.UtcNow.AddMonths(1),
                MonthlyPaymentAmount = loan.MonthlyPaymentAmount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disbursing loan {LoanId}", loanId);
            return new DisbursementResult
            {
                IsSuccess = false,
                Message = "An error occurred during loan disbursement"
            };
        }
    }

    public async Task<PaymentResult> ProcessPaymentAsync(LoanPaymentRequest request, Guid processedBy)
    {
        try
        {
            var loan = await _loanRepository.GetByIdAsync(request.LoanId);
            if (loan == null)
            {
                return new PaymentResult
                {
                    IsSuccess = false,
                    Message = "Loan not found"
                };
            }

            if (!loan.IsActive())
            {
                return new PaymentResult
                {
                    IsSuccess = false,
                    Message = "Loan is not active"
                };
            }

            // Calculate payment allocation using enhanced interest calculation service
            var interestCalculationResult = await _loanInterestCalculationService.CalculateLoanInterestAsync(loan, request.PaymentAmount);
            var principalAmount = interestCalculationResult.PrincipalAmount;
            var interestAmount = interestCalculationResult.InterestAmount;

            // Create payment record
            var payment = new LoanPayment
            {
                Id = Guid.NewGuid(),
                LoanId = request.LoanId,
                PaymentAmount = request.PaymentAmount,
                PrincipalAmount = principalAmount,
                InterestAmount = interestAmount,
                PaymentDate = DateTime.UtcNow,
                DueDate = loan.NextPaymentDueDate ?? DateTime.UtcNow,
                Status = LoanPaymentStatus.Paid,
                PaymentMethod = request.PaymentMethod,
                Notes = request.Notes,
                ProcessedBy = processedBy,
                ProcessedDate = DateTime.UtcNow,
                OutstandingBalanceAfterPayment = loan.OutstandingBalance - principalAmount
            };

            loan.Payments.Add(payment);

            // Update loan
            loan.UpdateOutstandingBalance(request.PaymentAmount, principalAmount, interestAmount);
            
            // Update next payment due date
            if (loan.Status == LoanStatus.Active || loan.Status == LoanStatus.Disbursed)
            {
                loan.NextPaymentDueDate = loan.NextPaymentDueDate?.AddMonths(1) ?? DateTime.UtcNow.AddMonths(1);
                loan.DaysOverdue = 0; // Reset overdue days
                
                if (loan.Status != LoanStatus.PaidOff)
                {
                    loan.Status = LoanStatus.Active;
                }
            }

            await _loanRepository.UpdateAsync(loan);

            _logger.LogInformation("Loan payment processed for loan {LoanId}. Amount: {Amount}", 
                request.LoanId, request.PaymentAmount);

            return new PaymentResult
            {
                IsSuccess = true,
                Message = loan.Status == LoanStatus.PaidOff ? "Loan paid off successfully" : "Payment processed successfully",
                PaymentAmount = request.PaymentAmount,
                PrincipalAmount = principalAmount,
                InterestAmount = interestAmount,
                RemainingBalance = loan.OutstandingBalance,
                PaymentDate = DateTime.UtcNow,
                NextPaymentDueDate = loan.Status == LoanStatus.PaidOff ? null : loan.NextPaymentDueDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing loan payment for loan {LoanId}", request.LoanId);
            return new PaymentResult
            {
                IsSuccess = false,
                Message = "An error occurred while processing the payment"
            };
        }
    }

    public async Task<List<LoanDto>> GetCustomerLoansAsync(Guid customerId)
    {
        var loans = await _loanRepository.GetByCustomerIdAsync(customerId);
        return loans.Select(MapToLoanDto).ToList();
    }

    public async Task<LoanDto?> GetLoanByIdAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        return loan != null ? MapToLoanDto(loan) : null;
    }

    public async Task<RepaymentSchedule> GenerateRepaymentScheduleAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);

        if (loan == null)
        {
            return new RepaymentSchedule();
        }

        var schedule = new RepaymentSchedule
        {
            LoanId = loanId,
            LoanNumber = loan.LoanNumber,
            MonthlyPayment = loan.MonthlyPaymentAmount,
            TotalPayments = loan.TermInMonths,
            Schedule = new List<RepaymentScheduleEntry>()
        };

        var balance = loan.PrincipalAmount;
        var monthlyRate = loan.InterestRate / 100 / 12;
        var startDate = loan.DisbursementDate ?? loan.ApprovalDate ?? DateTime.UtcNow;

        for (int i = 1; i <= loan.TermInMonths; i++)
        {
            var interestAmount = balance * monthlyRate;
            var principalAmount = loan.MonthlyPaymentAmount - interestAmount;
            balance -= principalAmount;

            var dueDate = startDate.AddMonths(i);
            var payment = loan.Payments.FirstOrDefault(p => p.DueDate.Date == dueDate.Date);

            schedule.Schedule.Add(new RepaymentScheduleEntry
            {
                PaymentNumber = i,
                DueDate = dueDate,
                PaymentAmount = loan.MonthlyPaymentAmount,
                PrincipalAmount = Math.Round(principalAmount, 2),
                InterestAmount = Math.Round(interestAmount, 2),
                RemainingBalance = Math.Max(0, Math.Round(balance, 2)),
                IsPaid = payment?.Status == LoanPaymentStatus.Paid,
                PaidDate = payment?.PaymentDate
            });
        }

        schedule.TotalAmount = schedule.Schedule.Sum(s => s.PaymentAmount);
        schedule.TotalInterest = schedule.Schedule.Sum(s => s.InterestAmount);

        return schedule;
    }

    public async Task<(List<LoanDto> Loans, int TotalCount)> SearchLoansAsync(LoanSearchRequest request)
    {
        var loans = await _loanRepository.SearchAsync(
            request.CustomerId,
            request.Type,
            request.Status,
            request.MinAmount,
            request.MaxAmount,
            request.ApplicationDateFrom,
            request.ApplicationDateTo,
            request.IsOverdue,
            (request.PageNumber - 1) * request.PageSize,
            request.PageSize,
            request.SortBy,
            request.SortDescending);

        var totalCount = await _loanRepository.GetSearchCountAsync(
            request.CustomerId,
            request.Type,
            request.Status,
            request.MinAmount,
            request.MaxAmount,
            request.ApplicationDateFrom,
            request.ApplicationDateTo,
            request.IsOverdue);

        return (loans.Select(MapToLoanDto).ToList(), totalCount);
    }

    public async Task<List<LoanPayment>> GetLoanPaymentHistoryAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        return loan?.Payments.OrderByDescending(p => p.PaymentDate).ToList() ?? new List<LoanPayment>();
    }

    public async Task<int> ProcessDelinquentLoansAsync()
    {
        var overdueLoans = await _loanRepository.GetOverdueLoansAsync();

        int processedCount = 0;

        foreach (var loan in overdueLoans)
        {
            var daysOverdue = (DateTime.UtcNow - loan.NextPaymentDueDate!.Value).Days;
            loan.DaysOverdue = daysOverdue;

            if (daysOverdue > 30)
            {
                loan.MarkAsDelinquent();
                processedCount++;
            }

            await _loanRepository.UpdateAsync(loan);
        }

        if (processedCount > 0)
        {
            _logger.LogInformation("Processed {Count} delinquent loans", processedCount);
        }

        return processedCount;
    }

    public async Task<RepaymentScheduleEntry?> GetNextPaymentDetailsAsync(Guid loanId)
    {
        var schedule = await GenerateRepaymentScheduleAsync(loanId);
        return schedule.Schedule.FirstOrDefault(s => !s.IsPaid);
    }

    public async Task UpdateLoanStatusAsync(Guid loanId, LoanStatus newStatus, Guid? changedBy = null, string? reason = null)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan == null) return;

        var oldStatus = loan.Status;
        loan.Status = newStatus;

        loan.StatusHistory.Add(new LoanStatusHistory
        {
            Id = Guid.NewGuid(),
            LoanId = loanId,
            FromStatus = oldStatus,
            ToStatus = newStatus,
            StatusChangeDate = DateTime.UtcNow,
            ChangedBy = changedBy,
            Reason = reason,
            IsSystemGenerated = changedBy == null
        });

        await _loanRepository.UpdateAsync(loan);
    }

    #region Private Helper Methods

    private async Task<int> SimulateCreditScoringAsync(Guid customerId)
    {
        // Simulate credit scoring based on customer data
        // In real implementation, this would call external credit bureau APIs
        
        var customer = await _userRepository.GetByIdAsync(customerId);
        
        // Simple scoring algorithm (replace with actual credit bureau integration)
        var baseScore = 650;
        
        // Use a deterministic approach based on customer ID for consistency
        var hash = customerId.GetHashCode();
        var randomFactor = (hash % 101) - 50; // Range: -50 to 50

        return Math.Max(300, Math.Min(850, baseScore + randomFactor));
    }

    private static CreditScoreRange GetCreditScoreRange(int score)
    {
        return score switch
        {
            >= 800 => CreditScoreRange.Excellent,
            >= 740 => CreditScoreRange.VeryGood,
            >= 670 => CreditScoreRange.Good,
            >= 580 => CreditScoreRange.Fair,
            _ => CreditScoreRange.Poor
        };
    }

    private static decimal CalculateInterestRateFromScore(int creditScore, LoanType loanType)
    {
        var baseRate = loanType switch
        {
            LoanType.Personal => 12.0m,
            LoanType.Auto => 8.0m,
            LoanType.Mortgage => 6.0m,
            LoanType.Business => 10.0m,
            LoanType.Education => 7.0m,
            LoanType.HomeEquity => 7.5m,
            _ => 10.0m
        };

        return InterestCalculationHelper.CalculateInterestRateFromScore(creditScore, baseRate / 100, 0.25m) * 100;
    }

    private static decimal CalculateMaxLoanAmount(int creditScore, decimal requestedAmount)
    {
        var multiplier = creditScore switch
        {
            >= 800 => 1.2m,
            >= 740 => 1.1m,
            >= 670 => 1.0m,
            >= 580 => 0.8m,
            _ => 0.6m
        };

        return requestedAmount * multiplier;
    }

    private static string GetRiskAssessment(CreditScoreRange scoreRange)
    {
        return scoreRange switch
        {
            CreditScoreRange.Excellent => "Very Low Risk",
            CreditScoreRange.VeryGood => "Low Risk",
            CreditScoreRange.Good => "Medium Risk",
            CreditScoreRange.Fair => "High Risk",
            CreditScoreRange.Poor => "Very High Risk",
            _ => "Unknown Risk"
        };
    }

    private static List<string> GetRiskFactors(int creditScore)
    {
        var factors = new List<string>();

        if (creditScore < 580)
        {
            factors.Add("Low credit score");
            factors.Add("High default risk");
        }
        else if (creditScore < 670)
        {
            factors.Add("Fair credit score");
            factors.Add("Moderate default risk");
        }

        return factors;
    }

    private static List<string> GetRequiredDocuments(LoanType loanType)
    {
        var documents = new List<string>
        {
            "Identity proof",
            "Address proof",
            "Income verification"
        };

        switch (loanType)
        {
            case LoanType.Auto:
                documents.Add("Vehicle registration");
                documents.Add("Insurance documents");
                break;
            case LoanType.Mortgage:
                documents.Add("Property documents");
                documents.Add("Property valuation");
                break;
            case LoanType.Business:
                documents.Add("Business registration");
                documents.Add("Financial statements");
                break;
        }

        return documents;
    }

    private static (decimal principalAmount, decimal interestAmount) CalculatePaymentAllocation(Loan loan, decimal paymentAmount)
    {
        var monthlyInterestRate = loan.InterestRate / 100 / 12;
        return CalculationHelper.CalculatePaymentAllocation(loan.OutstandingBalance, paymentAmount, monthlyInterestRate);
    }

    private LoanDto MapToLoanDto(Loan loan)
    {
        return _mapper.Map<LoanDto>(loan);
    }

    #endregion
}