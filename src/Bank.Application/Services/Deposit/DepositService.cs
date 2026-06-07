using AutoMapper;
using Bank.Application.DTOs;
using Bank.Application.Helpers.Shared;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Bank.Application.Services;

/// <summary>
/// Service for managing deposit products and fixed deposits
/// </summary>
public class DepositService : IDepositService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IInterestCalculationService _interestCalculationService;
    private readonly INotificationService _notificationService;
    private readonly IAuditLogService _auditLogService;
    private readonly IDepositWithdrawalService _depositWithdrawalService;
    private readonly ILogger<DepositService> _logger;
    private readonly IMapper _mapper;

    public DepositService(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IInterestCalculationService interestCalculationService,
        INotificationService notificationService,
        IAuditLogService auditLogService,
        IDepositWithdrawalService depositWithdrawalService,
        ILogger<DepositService> logger,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _interestCalculationService = interestCalculationService;
        _notificationService = notificationService;
        _auditLogService = auditLogService;
        _depositWithdrawalService = depositWithdrawalService;
        _logger = logger;
        _mapper = mapper;
    }

    #region Deposit Product Management

    public async Task<DepositProductDto?> GetDepositProductAsync(Guid productId)
    {
        var product = await _unitOfWork.Repository<DepositProduct>().GetByIdAsync(productId);
        return product == null ? null : MapToDepositProductDto(product);
    }

    public async Task<IEnumerable<DepositProductDto>> GetActiveDepositProductsAsync()
    {
        var products = await _unitOfWork.Repository<DepositProduct>()
            .FindAsync(p => p.IsActive);

        return products.Select(MapToDepositProductDto);
    }

    public async Task<IEnumerable<DepositProductDto>> GetDepositProductsByTypeAsync(DepositProductType productType)
    {
        var products = await _unitOfWork.Repository<DepositProduct>()
            .FindAsync(p => p.IsActive && p.ProductType == productType);

        return products.Select(MapToDepositProductDto);
    }
    public async Task<DepositProductDto> CreateDepositProductAsync(CreateDepositProductRequest request, Guid createdByUserId)
    {
        var product = new DepositProduct
        {
            Name = request.Name,
            Description = request.Description,
            ProductType = request.ProductType,
            MinimumTermDays = request.MinimumTermDays,
            MaximumTermDays = request.MaximumTermDays,
            DefaultTermDays = request.DefaultTermDays,
            MinimumBalance = request.MinimumBalance,
            MaximumBalance = request.MaximumBalance,
            MinimumOpeningBalance = request.MinimumOpeningBalance,
            BaseInterestRate = request.BaseInterestRate,
            InterestCalculationMethod = request.InterestCalculationMethod,
            CompoundingFrequency = request.CompoundingFrequency,
            HasTieredRates = request.HasTieredRates,
            AllowPartialWithdrawals = request.AllowPartialWithdrawals,
            PenaltyType = request.PenaltyType,
            PenaltyAmount = request.PenaltyAmount,
            PenaltyPercentage = request.PenaltyPercentage,
            PenaltyFreeDays = request.PenaltyFreeDays,
            DefaultMaturityAction = request.DefaultMaturityAction,
            AllowAutoRenewal = request.AllowAutoRenewal,
            AutoRenewalNoticeDays = request.AutoRenewalNoticeDays,
            PromotionalRateStartDate = request.PromotionalRateStartDate,
            PromotionalRateEndDate = request.PromotionalRateEndDate,
            PromotionalRate = request.PromotionalRate
        };

        await _unitOfWork.Repository<DepositProduct>().AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            createdByUserId,
            "DepositProduct",
            "Create",
            product.Id.ToString(),
            $"Created deposit product: {product.Name}");

        _logger.LogInformation("Created deposit product {ProductId} by user {UserId}", product.Id, createdByUserId);

        return MapToDepositProductDto(product);
    }

    public async Task<DepositProductDto> UpdateDepositProductAsync(Guid productId, UpdateDepositProductRequest request, Guid updatedByUserId)
    {
        var product = await _unitOfWork.Repository<DepositProduct>().GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException($"Deposit product {productId} not found");

        var originalName = product.Name;

        if (!string.IsNullOrEmpty(request.Name))
            product.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Description))
            product.Description = request.Description;
        if (request.IsActive.HasValue)
            product.IsActive = request.IsActive.Value;
        if (request.BaseInterestRate.HasValue)
            product.BaseInterestRate = request.BaseInterestRate.Value;
        if (request.AllowPartialWithdrawals.HasValue)
            product.AllowPartialWithdrawals = request.AllowPartialWithdrawals.Value;
        if (request.PenaltyType.HasValue)
            product.PenaltyType = request.PenaltyType.Value;
        if (request.PenaltyAmount.HasValue)
            product.PenaltyAmount = request.PenaltyAmount.Value;
        if (request.PenaltyPercentage.HasValue)
            product.PenaltyPercentage = request.PenaltyPercentage.Value;
        if (request.DefaultMaturityAction.HasValue)
            product.DefaultMaturityAction = request.DefaultMaturityAction.Value;
        if (request.AllowAutoRenewal.HasValue)
            product.AllowAutoRenewal = request.AllowAutoRenewal.Value;
        if (request.AutoRenewalNoticeDays.HasValue)
            product.AutoRenewalNoticeDays = request.AutoRenewalNoticeDays.Value;
        if (request.PromotionalRateStartDate.HasValue)
            product.PromotionalRateStartDate = request.PromotionalRateStartDate.Value;
        if (request.PromotionalRateEndDate.HasValue)
            product.PromotionalRateEndDate = request.PromotionalRateEndDate.Value;
        if (request.PromotionalRate.HasValue)
            product.PromotionalRate = request.PromotionalRate.Value;

        _unitOfWork.Repository<DepositProduct>().Update(product);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            updatedByUserId,
            "DepositProduct",
            "Update",
            product.Id.ToString(),
            $"Updated deposit product: {originalName} -> {product.Name}");

        _logger.LogInformation("Updated deposit product {ProductId} by user {UserId}", productId, updatedByUserId);

        return MapToDepositProductDto(product);
    }

    public async Task<bool> DeactivateDepositProductAsync(Guid productId, Guid deactivatedByUserId)
    {
        var product = await _unitOfWork.Repository<DepositProduct>().GetByIdAsync(productId);
        if (product == null)
            return false;

        product.IsActive = false;
        _unitOfWork.Repository<DepositProduct>().Update(product);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            deactivatedByUserId,
            "DepositProduct",
            "Deactivate",
            product.Id.ToString(),
            $"Deactivated deposit product: {product.Name}");

        _logger.LogInformation("Deactivated deposit product {ProductId} by user {UserId}", productId, deactivatedByUserId);

        return true;
    }

    #endregion
    #region Interest Tier Management

    public async Task<InterestTierDto> CreateInterestTierAsync(Guid productId, CreateInterestTierRequest request, Guid createdByUserId)
    {
        var product = await _unitOfWork.Repository<DepositProduct>().GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException($"Deposit product {productId} not found");

        var tier = new InterestTier
        {
            DepositProductId = productId,
            TierName = request.TierName,
            MinimumBalance = request.MinimumBalance,
            MaximumBalance = request.MaximumBalance,
            InterestRate = request.InterestRate,
            TierBasis = request.TierBasis,
            DisplayOrder = request.DisplayOrder,
            EffectiveFromDate = request.EffectiveFromDate,
            EffectiveToDate = request.EffectiveToDate,
            IsPromotional = request.IsPromotional
        };

        await _unitOfWork.Repository<InterestTier>().AddAsync(tier);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            createdByUserId,
            "InterestTier",
            "Create",
            tier.Id.ToString(),
            $"Created interest tier: {tier.TierName} for product {product.Name}");

        return MapToInterestTierDto(tier);
    }

    public async Task<InterestTierDto> UpdateInterestTierAsync(Guid tierId, UpdateInterestTierRequest request, Guid updatedByUserId)
    {
        var tier = await _unitOfWork.Repository<InterestTier>().GetByIdAsync(tierId);
        if (tier == null)
            throw new InvalidOperationException($"Interest tier {tierId} not found");

        if (!string.IsNullOrEmpty(request.TierName))
            tier.TierName = request.TierName;
        if (request.InterestRate.HasValue)
            tier.InterestRate = request.InterestRate.Value;
        if (request.IsActive.HasValue)
            tier.IsActive = request.IsActive.Value;
        if (request.DisplayOrder.HasValue)
            tier.DisplayOrder = request.DisplayOrder.Value;
        if (request.EffectiveFromDate.HasValue)
            tier.EffectiveFromDate = request.EffectiveFromDate.Value;
        if (request.EffectiveToDate.HasValue)
            tier.EffectiveToDate = request.EffectiveToDate.Value;

        _unitOfWork.Repository<InterestTier>().Update(tier);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            updatedByUserId,
            "InterestTier",
            "Update",
            tier.Id.ToString(),
            $"Updated interest tier: {tier.TierName}");

        return MapToInterestTierDto(tier);
    }

    public async Task<bool> DeleteInterestTierAsync(Guid tierId, Guid deletedByUserId)
    {
        var tier = await _unitOfWork.Repository<InterestTier>().GetByIdAsync(tierId);
        if (tier == null)
            return false;

        _unitOfWork.Repository<InterestTier>().Remove(tier);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            deletedByUserId,
            "InterestTier",
            "Delete",
            tier.Id.ToString(),
            $"Deleted interest tier: {tier.TierName}");

        return true;
    }

    public async Task<IEnumerable<InterestTierDto>> GetInterestTiersAsync(Guid productId)
    {
        var tiers = await _unitOfWork.Repository<InterestTier>()
            .FindAsync(t => t.DepositProductId == productId);

        return tiers.OrderBy(t => t.DisplayOrder).Select(MapToInterestTierDto);
    }

    #endregion
    #region Fixed Deposit Management

    public async Task<FixedDepositDto> CreateFixedDepositAsync(CreateFixedDepositRequest request, Guid customerId)
    {
        var product = await _unitOfWork.Repository<DepositProduct>()
            .GetByIdAsync(request.DepositProductId);
        
        if (product == null || !product.IsActive)
            throw new InvalidOperationException($"Deposit product {request.DepositProductId} not found or inactive");

        var linkedAccount = await _unitOfWork.Repository<Account>().GetByIdAsync(request.LinkedAccountId);
        if (linkedAccount == null || linkedAccount.CustomerId != customerId)
            throw new InvalidOperationException("Invalid linked account");

        // Validate balance requirements
        if (!product.IsValidBalance(request.PrincipalAmount))
            throw new InvalidOperationException($"Principal amount must be between {product.MinimumBalance} and {product.MaximumBalance}");

        // Validate term
        var termDays = request.TermDays ?? product.DefaultTermDays ?? 365;
        if (!product.IsValidTerm(termDays))
            throw new InvalidOperationException($"Term must be between {product.MinimumTermDays} and {product.MaximumTermDays} days");

        // Check account balance
        if (linkedAccount.Balance < request.PrincipalAmount)
            throw new InvalidOperationException("Insufficient balance in linked account");

        var deposit = new FixedDeposit
        {
            CustomerId = customerId,
            DepositProductId = request.DepositProductId,
            LinkedAccountId = request.LinkedAccountId,
            PrincipalAmount = request.PrincipalAmount,
            InterestRate = product.GetApplicableRate(request.PrincipalAmount, termDays),
            TermDays = termDays,
            StartDate = DateTime.UtcNow,
            MaturityDate = DateTime.UtcNow.AddDays(termDays),
            Status = FixedDepositStatus.Active,
            InterestCalculationMethod = product.InterestCalculationMethod,
            CompoundingFrequency = product.CompoundingFrequency,
            LastInterestCalculationDate = DateTime.UtcNow,
            MaturityAction = request.MaturityAction ?? product.DefaultMaturityAction,
            AutoRenewalEnabled = request.AutoRenewalEnabled ?? product.AllowAutoRenewal,
            RenewalTermDays = request.RenewalTermDays,
            PenaltyType = product.PenaltyType,
            PenaltyAmount = product.PenaltyAmount,
            PenaltyPercentage = product.PenaltyPercentage
        };

        deposit.GenerateDepositNumber();

        // Debit the linked account
        linkedAccount.Balance -= request.PrincipalAmount;
        
        await _unitOfWork.Repository<FixedDeposit>().AddAsync(deposit);
        _unitOfWork.Repository<Account>().Update(linkedAccount);

        // Create deposit transaction record
        var transaction = new DepositTransaction
        {
            FixedDepositId = deposit.Id,
            TransactionType = DepositTransactionType.InterestCredit,
            Amount = request.PrincipalAmount,
            Description = $"Fixed deposit creation - {deposit.DepositNumber}",
            TransactionDate = DateTime.UtcNow,
            Status = TransactionStatus.Completed
        };
        transaction.GenerateTransactionReference();

        await _unitOfWork.Repository<DepositTransaction>().AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            customerId,
            "FixedDeposit",
            "Create",
            deposit.Id.ToString(),
            $"Created fixed deposit: {deposit.DepositNumber} for {request.PrincipalAmount:C}");

        _logger.LogInformation("Created fixed deposit {DepositId} for customer {CustomerId}", deposit.Id, customerId);

        return await MapToFixedDepositDtoAsync(deposit);
    }

    public async Task<FixedDepositDto?> GetFixedDepositAsync(Guid depositId)
    {
        var deposit = await _unitOfWork.Repository<FixedDeposit>()
            .GetByIdAsync(depositId);

        return deposit == null ? null : await MapToFixedDepositDtoAsync(deposit);
    }

    public async Task<FixedDepositDto?> GetFixedDepositByNumberAsync(string depositNumber)
    {
        var deposits = await _unitOfWork.Repository<FixedDeposit>()
            .FindAsync(d => d.DepositNumber == depositNumber);

        var deposit = deposits.FirstOrDefault();
        return deposit == null ? null : await MapToFixedDepositDtoAsync(deposit);
    }

    public async Task<IEnumerable<FixedDepositDto>> GetCustomerFixedDepositsAsync(Guid customerId)
    {
        var deposits = await _unitOfWork.Repository<FixedDeposit>()
            .FindAsync(d => d.CustomerId == customerId);

        var result = new List<FixedDepositDto>();
        foreach (var deposit in deposits.OrderByDescending(d => d.CreatedAt))
        {
            result.Add(await MapToFixedDepositDtoAsync(deposit));
        }
        return result;
    }

    public async Task<IEnumerable<FixedDepositDto>> GetMaturingDepositsAsync(DateTime fromDate, DateTime toDate)
    {
        var deposits = await _unitOfWork.Repository<FixedDeposit>()
            .FindAsync(d => d.Status == FixedDepositStatus.Active && 
                           d.MaturityDate >= fromDate && 
                           d.MaturityDate <= toDate);

        var result = new List<FixedDepositDto>();
        foreach (var deposit in deposits.OrderBy(d => d.MaturityDate))
        {
            result.Add(await MapToFixedDepositDtoAsync(deposit));
        }
        return result;
    }

    #endregion
    #region Interest Calculation and Processing

    public async Task<decimal> CalculateInterestAsync(Guid depositId, DateTime fromDate, DateTime toDate)
    {
        var deposit = await _unitOfWork.Repository<FixedDeposit>().GetByIdAsync(depositId);
        if (deposit == null)
            throw new InvalidOperationException($"Fixed deposit {depositId} not found");

        var principal = deposit.PrincipalAmount;
        var rate = deposit.InterestRate / 100;
        var days = (toDate - fromDate).Days;

        return deposit.InterestCalculationMethod switch
        {
            InterestCalculationMethod.Simple => CalculationHelper.CalculateSimpleInterest(principal, rate, days),
            InterestCalculationMethod.CompoundDaily => CalculationHelper.CalculateCompoundInterest(principal, rate, days, 365),
            InterestCalculationMethod.CompoundMonthly => CalculationHelper.CalculateCompoundInterest(principal, rate, days, 12),
            _ => CalculationHelper.CalculateSimpleInterest(principal, rate, days)
        };
    }



    public async Task<bool> ProcessInterestCreditAsync(Guid depositId, Guid processedByUserId)
    {
        var deposit = await _unitOfWork.Repository<FixedDeposit>().GetByIdAsync(depositId);
        if (deposit == null || deposit.Status != FixedDepositStatus.Active)
            return false;

        var fromDate = deposit.LastInterestCalculationDate;
        var toDate = DateTime.UtcNow;
        
        if ((toDate - fromDate).Days < 1)
            return false; // No interest to calculate

        var interestAmount = await CalculateInterestAsync(depositId, fromDate, toDate);
        if (interestAmount <= 0)
            return false;

        deposit.AccruedInterest += interestAmount;
        deposit.LastInterestCalculationDate = toDate;

        var transaction = new DepositTransaction
        {
            FixedDepositId = depositId,
            TransactionType = DepositTransactionType.InterestCredit,
            Amount = interestAmount,
            Description = $"Interest credit for period {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
            TransactionDate = toDate,
            Status = TransactionStatus.Completed,
            InterestPeriodStart = fromDate,
            InterestPeriodEnd = toDate,
            InterestRate = deposit.InterestRate,
            InterestDays = (toDate - fromDate).Days,
            ProcessedByUserId = processedByUserId,
            ProcessedDate = DateTime.UtcNow
        };
        transaction.GenerateTransactionReference();

        _unitOfWork.Repository<FixedDeposit>().Update(deposit);
        await _unitOfWork.Repository<DepositTransaction>().AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogSystemEventAsync(
            "DepositInterest",
            "Credit",
            depositId.ToString(),
            $"Credited interest {interestAmount:C} to deposit {deposit.DepositNumber}");

        return true;
    }

    public async Task<bool> ProcessDailyInterestAsync()
    {
        var activeDeposits = await _unitOfWork.Repository<FixedDeposit>()
            .FindAsync(d => d.Status == FixedDepositStatus.Active);

        var processedCount = 0;
        foreach (var deposit in activeDeposits)
        {
            try
            {
                if (await ProcessInterestCreditAsync(deposit.Id, Guid.Empty))
                    processedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing daily interest for deposit {DepositId}", deposit.Id);
            }
        }

        _logger.LogInformation("Processed daily interest for {Count} deposits", processedCount);
        return true;
    }

    public async Task<bool> ProcessMonthlyInterestAsync()
    {
        // This would be similar to daily but with different logic for monthly compounding
        return await ProcessDailyInterestAsync();
    }

    #endregion
    #region Maturity and Renewal Management

    public async Task<MaturityDetailsDto> GetMaturityDetailsAsync(Guid depositId)
    {
        var deposit = await _unitOfWork.Repository<FixedDeposit>()
            .GetByIdAsync(depositId);
        
        if (deposit == null)
            throw new InvalidOperationException($"Fixed deposit {depositId} not found");

        var maturityAmount = deposit.CalculateMaturityAmount();
        var interestAtMaturity = deposit.CalculateInterestAtMaturity();

        var availableActions = new List<MaturityActionOption>
        {
            new() { Action = MaturityAction.TransferToPrimary, Description = "Transfer to primary account", RequiresCustomerConsent = false },
            new() { Action = MaturityAction.HoldForInstructions, Description = "Hold pending instructions", RequiresCustomerConsent = true }
        };

        if (deposit.DepositProduct.AllowAutoRenewal)
        {
            availableActions.Add(new MaturityActionOption 
            { 
                Action = MaturityAction.AutoRenew, 
                Description = "Auto-renew for same term", 
                RequiresCustomerConsent = true 
            });
        }

        return new MaturityDetailsDto
        {
            DepositId = depositId,
            MaturityDate = deposit.MaturityDate,
            PrincipalAmount = deposit.PrincipalAmount,
            AccruedInterest = deposit.AccruedInterest,
            MaturityAmount = maturityAmount,
            DefaultAction = deposit.MaturityAction,
            AutoRenewalEnabled = deposit.AutoRenewalEnabled,
            RenewalTermDays = deposit.RenewalTermDays,
            CustomerConsentReceived = deposit.CustomerConsentReceived,
            AvailableActions = availableActions
        };
    }

    public async Task<bool> ProcessMaturityAsync(Guid depositId, MaturityAction action, Guid processedByUserId)
    {
        var deposit = await _unitOfWork.Repository<FixedDeposit>()
            .GetByIdAsync(depositId);
        
        if (deposit == null || deposit.Status != FixedDepositStatus.Active)
            return false;

        var maturityAmount = deposit.CalculateMaturityAmount();

        switch (action)
        {
            case MaturityAction.TransferToPrimary:
                return await ProcessMaturityTransferAsync(deposit, maturityAmount, processedByUserId);
            
            case MaturityAction.AutoRenew:
                return await ProcessAutoRenewalAsync(deposit, processedByUserId);
            
            case MaturityAction.HoldForInstructions:
                return await ProcessMaturityHoldAsync(deposit, processedByUserId);
            
            default:
                return false;
        }
    }

    private async Task<bool> ProcessMaturityTransferAsync(FixedDeposit deposit, decimal maturityAmount, Guid processedByUserId)
    {
        deposit.Status = FixedDepositStatus.Matured;
        deposit.ClosureDate = DateTime.UtcNow;
        deposit.NetAmountPaid = maturityAmount;

        // Credit the linked account
        var linkedAccount = await _unitOfWork.Repository<Account>().GetByIdAsync(deposit.LinkedAccountId);
        if (linkedAccount != null)
        {
            linkedAccount.Balance += maturityAmount;
        }

        var transaction = new DepositTransaction
        {
            FixedDepositId = deposit.Id,
            TransactionType = DepositTransactionType.MaturityPayout,
            Amount = maturityAmount,
            Description = $"Maturity payout for deposit {deposit.DepositNumber}",
            TransactionDate = DateTime.UtcNow,
            Status = TransactionStatus.Completed,
            ProcessedByUserId = processedByUserId,
            ProcessedDate = DateTime.UtcNow
        };
        transaction.GenerateTransactionReference();

        _unitOfWork.Repository<FixedDeposit>().Update(deposit);
        _unitOfWork.Repository<Account>().Update(deposit.LinkedAccount);
        await _unitOfWork.Repository<DepositTransaction>().AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            processedByUserId,
            "FixedDeposit",
            "Maturity",
            deposit.Id.ToString(),
            $"Processed maturity for deposit {deposit.DepositNumber}, paid {maturityAmount:C}");

        return true;
    }

    private async Task<bool> ProcessAutoRenewalAsync(FixedDeposit deposit, Guid processedByUserId)
    {
        if (!deposit.AutoRenewalEnabled || !deposit.CustomerConsentReceived)
            return false;

        var renewalRequest = new RenewDepositRequest
        {
            TermDays = deposit.RenewalTermDays ?? deposit.TermDays,
            InterestRate = deposit.InterestRate,
            MaturityAction = deposit.MaturityAction,
            AutoRenewalEnabled = deposit.AutoRenewalEnabled
        };

        await RenewFixedDepositAsync(deposit.Id, renewalRequest, processedByUserId);
        return true;
    }

    private async Task<bool> ProcessMaturityHoldAsync(FixedDeposit deposit, Guid processedByUserId)
    {
        deposit.Status = FixedDepositStatus.PendingRenewal;
        _unitOfWork.Repository<FixedDeposit>().Update(deposit);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            processedByUserId,
            "FixedDeposit",
            "Hold",
            deposit.Id.ToString(),
            $"Deposit {deposit.DepositNumber} held pending customer instructions");

        return true;
    }

    public async Task<FixedDepositDto> RenewFixedDepositAsync(Guid depositId, RenewDepositRequest request, Guid processedByUserId)
    {
        var originalDeposit = await _unitOfWork.Repository<FixedDeposit>()
            .GetByIdAsync(depositId);
        
        if (originalDeposit == null)
            throw new InvalidOperationException($"Fixed deposit {depositId} not found");

        var maturityAmount = originalDeposit.CalculateMaturityAmount();
        var termDays = request.TermDays ?? originalDeposit.TermDays;
        var interestRate = request.InterestRate ?? originalDeposit.DepositProduct.GetApplicableRate(maturityAmount, termDays);

        // Close original deposit
        originalDeposit.Status = FixedDepositStatus.Renewed;
        originalDeposit.ClosureDate = DateTime.UtcNow;

        // Create new deposit
        var renewedDeposit = new FixedDeposit
        {
            CustomerId = originalDeposit.CustomerId,
            DepositProductId = originalDeposit.DepositProductId,
            LinkedAccountId = originalDeposit.LinkedAccountId,
            PrincipalAmount = maturityAmount,
            InterestRate = interestRate,
            TermDays = termDays,
            StartDate = DateTime.UtcNow,
            MaturityDate = DateTime.UtcNow.AddDays(termDays),
            Status = FixedDepositStatus.Active,
            InterestCalculationMethod = originalDeposit.InterestCalculationMethod,
            CompoundingFrequency = originalDeposit.CompoundingFrequency,
            LastInterestCalculationDate = DateTime.UtcNow,
            MaturityAction = request.MaturityAction ?? originalDeposit.MaturityAction,
            AutoRenewalEnabled = request.AutoRenewalEnabled ?? originalDeposit.AutoRenewalEnabled,
            PenaltyType = originalDeposit.PenaltyType,
            PenaltyAmount = originalDeposit.PenaltyAmount,
            PenaltyPercentage = originalDeposit.PenaltyPercentage,
            RenewedFromDepositId = originalDeposit.Id,
            RenewalCount = originalDeposit.RenewalCount + 1
        };

        renewedDeposit.GenerateDepositNumber();
        originalDeposit.RenewedToDepositId = renewedDeposit.Id;

        _unitOfWork.Repository<FixedDeposit>().Update(originalDeposit);
        await _unitOfWork.Repository<FixedDeposit>().AddAsync(renewedDeposit);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            processedByUserId,
            "FixedDeposit",
            "Renew",
            renewedDeposit.Id.ToString(),
            $"Renewed deposit {originalDeposit.DepositNumber} to {renewedDeposit.DepositNumber}");

        return await MapToFixedDepositDtoAsync(renewedDeposit);
    }

    public async Task<bool> ProcessAutoRenewalsAsync()
    {
        var maturingDeposits = await _unitOfWork.Repository<FixedDeposit>()
            .FindAsync(d => d.Status == FixedDepositStatus.Active &&
                           d.MaturityDate <= DateTime.UtcNow &&
                           d.AutoRenewalEnabled &&
                           d.CustomerConsentReceived);

        var processedCount = 0;
        foreach (var deposit in maturingDeposits)
        {
            try
            {
                if (await ProcessAutoRenewalAsync(deposit, Guid.Empty))
                    processedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing auto-renewal for deposit {DepositId}", deposit.Id);
            }
        }

        _logger.LogInformation("Processed auto-renewals for {Count} deposits", processedCount);
        return true;
    }

    #endregion
    #region Withdrawal Management

    public async Task<WithdrawalDetailsDto> CalculateEarlyWithdrawalAsync(Guid depositId, decimal withdrawalAmount)
    {
        // Delegate to DepositWithdrawalService to avoid duplication
        var calculation = await _depositWithdrawalService.CalculateDetailedWithdrawalAsync(depositId, withdrawalAmount);
        
        return new WithdrawalDetailsDto
        {
            DepositId = depositId,
            RequestedAmount = withdrawalAmount,
            AvailableBalance = calculation.AvailableBalance,
            PenaltyAmount = calculation.PenaltyAmount,
            NetAmount = calculation.NetAmount,
            PenaltyType = calculation.PenaltyType,
            PenaltyDescription = calculation.PenaltyDescription,
            IsEarlyWithdrawal = calculation.IsEarlyWithdrawal,
            DaysBeforeMaturity = calculation.DaysBeforeMaturity
        };
    }

    private static string GetPenaltyDescription(WithdrawalPenaltyType penaltyType, decimal penaltyAmount)
    {
        return penaltyType switch
        {
            WithdrawalPenaltyType.None => "No penalty applicable",
            WithdrawalPenaltyType.FixedAmount => $"Fixed penalty of {penaltyAmount:C}",
            WithdrawalPenaltyType.Percentage => $"Percentage-based penalty of {penaltyAmount:C}",
            WithdrawalPenaltyType.InterestForfeiture => $"Interest forfeiture penalty of {penaltyAmount:C}",
            WithdrawalPenaltyType.Combined => $"Combined penalty of {penaltyAmount:C}",
            _ => "Penalty calculation pending"
        };
    }

    public async Task<bool> ProcessEarlyWithdrawalAsync(Guid depositId, EarlyWithdrawalRequest request, Guid processedByUserId)
    {
        // Delegate to DepositWithdrawalService to avoid duplication
        var result = await _depositWithdrawalService.ProcessEarlyWithdrawalWithDetailsAsync(depositId, request, processedByUserId);
        return result.Success;
    }

    public async Task<bool> ProcessPartialWithdrawalAsync(Guid depositId, PartialWithdrawalRequest request, Guid processedByUserId)
    {
        var deposit = await _unitOfWork.Repository<FixedDeposit>()
            .GetByIdAsync(depositId);
        
        if (deposit == null || deposit.Status != FixedDepositStatus.Active)
            return false;

        if (!deposit.DepositProduct.AllowPartialWithdrawals)
            throw new InvalidOperationException("Partial withdrawals not allowed for this deposit product");

        var availableBalance = deposit.PrincipalAmount + deposit.AccruedInterest;
        if (request.WithdrawalAmount > availableBalance)
            throw new InvalidOperationException("Withdrawal amount exceeds available balance");

        var remainingBalance = availableBalance - request.WithdrawalAmount;
        if (remainingBalance < deposit.DepositProduct.MinimumBalance)
            throw new InvalidOperationException($"Remaining balance would be below minimum of {deposit.DepositProduct.MinimumBalance:C}");

        // Update deposit principal (assuming withdrawal comes from principal first)
        if (request.WithdrawalAmount <= deposit.PrincipalAmount)
        {
            deposit.PrincipalAmount -= request.WithdrawalAmount;
        }
        else
        {
            var interestWithdrawal = request.WithdrawalAmount - deposit.PrincipalAmount;
            deposit.PrincipalAmount = 0;
            deposit.AccruedInterest -= interestWithdrawal;
        }

        // Credit the linked account
        deposit.LinkedAccount.Balance += request.WithdrawalAmount;

        var transaction = new DepositTransaction
        {
            FixedDepositId = depositId,
            TransactionType = DepositTransactionType.PartialWithdrawal,
            Amount = request.WithdrawalAmount,
            Description = $"Partial withdrawal from deposit {deposit.DepositNumber}: {request.Reason}",
            TransactionDate = DateTime.UtcNow,
            Status = TransactionStatus.Completed,
            ProcessedByUserId = processedByUserId,
            ProcessedDate = DateTime.UtcNow
        };
        transaction.GenerateTransactionReference();

        _unitOfWork.Repository<FixedDeposit>().Update(deposit);
        _unitOfWork.Repository<Account>().Update(deposit.LinkedAccount);
        await _unitOfWork.Repository<DepositTransaction>().AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            processedByUserId,
            "FixedDeposit",
            "PartialWithdrawal",
            depositId.ToString(),
            $"Processed partial withdrawal of {request.WithdrawalAmount:C} from deposit {deposit.DepositNumber}");

        return true;
    }

    #endregion
    #region Certificate Management

    public async Task<DepositCertificateDto> GenerateCertificateAsync(Guid depositId, Guid generatedByUserId)
    {
        var deposit = await _unitOfWork.Repository<FixedDeposit>()
            .GetByIdAsync(depositId);
        
        if (deposit == null)
            throw new InvalidOperationException($"Fixed deposit {depositId} not found");

        var certificate = new DepositCertificate
        {
            FixedDepositId = depositId,
            Status = DepositCertificateStatus.Generated,
            IssueDate = DateTime.UtcNow,
            CertificateTemplate = "StandardDepositCertificate",
            CertificateContent = GenerateCertificateContent(deposit),
            GeneratedByUserId = generatedByUserId
        };

        certificate.GenerateCertificateNumber();
        certificate.GenerateSecurityHash();

        await _unitOfWork.Repository<DepositCertificate>().AddAsync(certificate);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            generatedByUserId,
            "DepositCertificate",
            "Generate",
            certificate.Id.ToString(),
            $"Generated certificate {certificate.CertificateNumber} for deposit {deposit.DepositNumber}");

        return MapToDepositCertificateDto(certificate);
    }

    private static string GenerateCertificateContent(FixedDeposit deposit)
    {
        return $@"
DEPOSIT CERTIFICATE

Certificate Number: {deposit.DepositNumber}
Customer: {deposit.Customer.UserName}
Principal Amount: {deposit.PrincipalAmount:C}
Interest Rate: {deposit.InterestRate}%
Term: {deposit.TermDays} days
Start Date: {deposit.StartDate:yyyy-MM-dd}
Maturity Date: {deposit.MaturityDate:yyyy-MM-dd}
Maturity Amount: {deposit.CalculateMaturityAmount():C}

This certificate confirms the deposit details above.
";
    }

    public async Task<DepositCertificateDto?> GetCertificateAsync(Guid certificateId)
    {
        var certificate = await _unitOfWork.Repository<DepositCertificate>().GetByIdAsync(certificateId);
        return certificate == null ? null : MapToDepositCertificateDto(certificate);
    }

    public async Task<byte[]> GetCertificatePdfAsync(Guid certificateId)
    {
        var certificate = await _unitOfWork.Repository<DepositCertificate>().GetByIdAsync(certificateId);
        if (certificate?.CertificatePdf == null)
            throw new InvalidOperationException("Certificate PDF not found");

        return certificate.CertificatePdf;
    }

    public async Task<bool> DeliverCertificateAsync(Guid certificateId, string deliveryMethod, string deliveryAddress, Guid deliveredByUserId)
    {
        var certificate = await _unitOfWork.Repository<DepositCertificate>().GetByIdAsync(certificateId);
        if (certificate == null)
            return false;

        certificate.Status = DepositCertificateStatus.Issued;
        certificate.DeliveryMethod = deliveryMethod;
        certificate.DeliveryAddress = deliveryAddress;
        certificate.IssuedByUserId = deliveredByUserId;

        _unitOfWork.Repository<DepositCertificate>().Update(certificate);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            deliveredByUserId,
            "DepositCertificate",
            "Deliver",
            certificateId.ToString(),
            $"Delivered certificate {certificate.CertificateNumber} via {deliveryMethod}");

        return true;
    }

    #endregion

    #region Notice Management

    public async Task<MaturityNoticeDto> GenerateMaturityNoticeAsync(Guid depositId, MaturityNoticeType noticeType, Guid generatedByUserId)
    {
        var deposit = await _unitOfWork.Repository<FixedDeposit>()
            .GetByIdAsync(depositId);
        
        if (deposit == null)
            throw new InvalidOperationException($"Fixed deposit {depositId} not found");

        var notice = new MaturityNotice
        {
            FixedDepositId = depositId,
            NoticeType = noticeType,
            NoticeDate = DateTime.UtcNow,
            MaturityDate = deposit.MaturityDate,
            Status = NotificationStatus.Pending,
            Subject = GenerateNoticeSubject(noticeType, deposit),
            Content = GenerateNoticeContent(noticeType, deposit),
            DeliveryChannel = NotificationChannel.Email,
            DeliveryAddress = deposit.Customer.Email,
            GeneratedByUserId = generatedByUserId
        };

        notice.GenerateNoticeNumber();

        await _unitOfWork.Repository<MaturityNotice>().AddAsync(notice);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            generatedByUserId,
            "MaturityNotice",
            "Generate",
            notice.Id.ToString(),
            $"Generated {noticeType} notice {notice.NoticeNumber} for deposit {deposit.DepositNumber}");

        return MapToMaturityNoticeDto(notice);
    }

    private static string GenerateNoticeSubject(MaturityNoticeType noticeType, FixedDeposit deposit)
    {
        return noticeType switch
        {
            MaturityNoticeType.Initial => $"Fixed Deposit Maturity Notice - {deposit.DepositNumber}",
            MaturityNoticeType.Reminder => $"Reminder: Fixed Deposit Maturing Soon - {deposit.DepositNumber}",
            MaturityNoticeType.Final => $"Final Notice: Fixed Deposit Maturity - {deposit.DepositNumber}",
            MaturityNoticeType.AutoRenewal => $"Auto-Renewal Confirmation - {deposit.DepositNumber}",
            _ => $"Fixed Deposit Notice - {deposit.DepositNumber}"
        };
    }

    private static string GenerateNoticeContent(MaturityNoticeType noticeType, FixedDeposit deposit)
    {
        var maturityAmount = deposit.CalculateMaturityAmount();
        var daysToMaturity = (deposit.MaturityDate - DateTime.UtcNow).Days;

        return $@"
Dear Customer,

Your fixed deposit {deposit.DepositNumber} will mature on {deposit.MaturityDate:yyyy-MM-dd} ({daysToMaturity} days from now).

Deposit Details:
- Principal Amount: {deposit.PrincipalAmount:C}
- Interest Rate: {deposit.InterestRate}%
- Maturity Amount: {maturityAmount:C}

Please contact us to provide instructions for the maturity proceeds.

Best regards,
Bank Customer Service
";
    }

    public async Task<bool> SendMaturityNoticesAsync()
    {
        // Get deposits maturing in the next 30 days that haven't received notices
        var maturingDeposits = await _unitOfWork.Repository<FixedDeposit>()
            .FindAsync(d => d.Status == FixedDepositStatus.Active &&
                           d.MaturityDate <= DateTime.UtcNow.AddDays(30) &&
                           d.MaturityDate > DateTime.UtcNow);

        var processedCount = 0;
        foreach (var deposit in maturingDeposits)
        {
            try
            {
                // Check if notice already sent
                var existingNotices = await _unitOfWork.Repository<MaturityNotice>()
                    .FindAsync(n => n.FixedDepositId == deposit.Id);

                if (!existingNotices.Any())
                {
                    await GenerateMaturityNoticeAsync(deposit.Id, MaturityNoticeType.Initial, Guid.Empty);
                    processedCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending maturity notice for deposit {DepositId}", deposit.Id);
            }
        }

        _logger.LogInformation("Sent maturity notices for {Count} deposits", processedCount);
        return true;
    }

    public async Task<bool> ProcessCustomerResponseAsync(Guid noticeId, MaturityAction customerChoice, string? instructions, Guid processedByUserId)
    {
        var notice = await _unitOfWork.Repository<MaturityNotice>().GetByIdAsync(noticeId);
        if (notice == null)
            return false;

        notice.RecordCustomerResponse(customerChoice, instructions);

        // Update the deposit with customer choice
        var deposit = await _unitOfWork.Repository<FixedDeposit>().GetByIdAsync(notice.FixedDepositId);
        if (deposit != null)
        {
            deposit.MaturityAction = customerChoice;
            deposit.CustomerConsentReceived = true;
            _unitOfWork.Repository<FixedDeposit>().Update(deposit);
        }

        _unitOfWork.Repository<MaturityNotice>().Update(notice);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogUserActionAsync(
            processedByUserId,
            "MaturityNotice",
            "CustomerResponse",
            noticeId.ToString(),
            $"Recorded customer response: {customerChoice} for notice {notice.NoticeNumber}");

        return true;
    }

    #endregion
    #region Reporting and Analytics

    public async Task<DepositSummaryDto> GetDepositSummaryAsync(Guid customerId)
    {
        var deposits = await _unitOfWork.Repository<FixedDeposit>()
            .FindAsync(d => d.CustomerId == customerId);

        var activeDeposits = deposits.Where(d => d.Status == FixedDepositStatus.Active).ToList();
        var maturingThisMonth = activeDeposits.Where(d => d.MaturityDate <= DateTime.UtcNow.AddDays(30)).Count();

        return new DepositSummaryDto
        {
            CustomerId = customerId,
            TotalDeposits = deposits.Count(),
            TotalPrincipal = activeDeposits.Sum(d => d.PrincipalAmount),
            TotalAccruedInterest = activeDeposits.Sum(d => d.AccruedInterest),
            TotalMaturityValue = activeDeposits.Sum(d => d.CalculateMaturityAmount()),
            ActiveDeposits = activeDeposits.Count,
            MaturingThisMonth = maturingThisMonth,
            AverageInterestRate = activeDeposits.Any() ? activeDeposits.Average(d => d.InterestRate) : 0
        };
    }

    public async Task<IEnumerable<DepositTransactionDto>> GetDepositTransactionsAsync(Guid depositId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var transactions = await _unitOfWork.Repository<DepositTransaction>()
            .FindAsync(t => t.FixedDepositId == depositId &&
                           (!fromDate.HasValue || t.TransactionDate >= fromDate.Value) &&
                           (!toDate.HasValue || t.TransactionDate <= toDate.Value));

        return transactions.OrderByDescending(t => t.TransactionDate).Select(t => _mapper.Map<DepositTransactionDto>(t));
    }

    public async Task<DepositPortfolioDto> GetCustomerDepositPortfolioAsync(Guid customerId)
    {
        var customer = await _userRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new InvalidOperationException($"Customer {customerId} not found");

        var summary = await GetDepositSummaryAsync(customerId);
        var activeDeposits = await GetCustomerFixedDepositsAsync(customerId);
        var maturingDeposits = activeDeposits.Where(d => d.Status == FixedDepositStatus.Active && 
                                                         d.MaturityDate <= DateTime.UtcNow.AddDays(30)).ToList();

        // Get recent transactions across all deposits
        var recentTransactions = new List<DepositTransactionDto>();
        foreach (var deposit in activeDeposits.Take(5)) // Limit to recent deposits
        {
            var transactions = await GetDepositTransactionsAsync(deposit.Id, DateTime.UtcNow.AddDays(-30));
            recentTransactions.AddRange(transactions.Take(10));
        }

        return new DepositPortfolioDto
        {
            CustomerId = customerId,
            CustomerName = customer.UserName ?? string.Empty,
            Summary = summary,
            ActiveDeposits = activeDeposits.Where(d => d.Status == FixedDepositStatus.Active).ToList(),
            MaturingDeposits = maturingDeposits,
            RecentTransactions = recentTransactions.OrderByDescending(t => t.TransactionDate).Take(20).ToList()
        };
    }

    #endregion

    #region Background Processing

    public async Task<bool> ProcessMaturityNoticesAsync()
    {
        return await SendMaturityNoticesAsync();
    }

    public async Task<bool> ProcessPendingMaturityActionsAsync()
    {
        var maturingDeposits = await _unitOfWork.Repository<FixedDeposit>()
            .FindAsync(d => d.Status == FixedDepositStatus.Active &&
                           d.MaturityDate <= DateTime.UtcNow);

        var processedCount = 0;
        foreach (var deposit in maturingDeposits)
        {
            try
            {
                if (await ProcessMaturityAsync(deposit.Id, deposit.MaturityAction, Guid.Empty))
                    processedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing maturity for deposit {DepositId}", deposit.Id);
            }
        }

        _logger.LogInformation("Processed maturity actions for {Count} deposits", processedCount);
        return true;
    }

    public async Task<bool> ProcessInterestAccrualsAsync()
    {
        return await ProcessDailyInterestAsync();
    }

    #endregion
    #region Mapping Methods

    private static DepositProductDto MapToDepositProductDto(DepositProduct product)
    {
        return new DepositProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ProductType = product.ProductType,
            IsActive = product.IsActive,
            MinimumTermDays = product.MinimumTermDays,
            MaximumTermDays = product.MaximumTermDays,
            DefaultTermDays = product.DefaultTermDays,
            MinimumBalance = product.MinimumBalance,
            MaximumBalance = product.MaximumBalance,
            MinimumOpeningBalance = product.MinimumOpeningBalance,
            BaseInterestRate = product.BaseInterestRate,
            InterestCalculationMethod = product.InterestCalculationMethod,
            CompoundingFrequency = product.CompoundingFrequency,
            HasTieredRates = product.HasTieredRates,
            AllowPartialWithdrawals = product.AllowPartialWithdrawals,
            PenaltyType = product.PenaltyType,
            PenaltyAmount = product.PenaltyAmount,
            PenaltyPercentage = product.PenaltyPercentage,
            PenaltyFreeDays = product.PenaltyFreeDays,
            DefaultMaturityAction = product.DefaultMaturityAction,
            AllowAutoRenewal = product.AllowAutoRenewal,
            AutoRenewalNoticeDays = product.AutoRenewalNoticeDays,
            PromotionalRateStartDate = product.PromotionalRateStartDate,
            PromotionalRateEndDate = product.PromotionalRateEndDate,
            PromotionalRate = product.PromotionalRate,
            IsPromotionalRateActive = product.IsPromotionalRateActive(),
            InterestTiers = product.InterestTiers.Select(MapToInterestTierDto).ToList(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    private static InterestTierDto MapToInterestTierDto(InterestTier tier)
    {
        return new InterestTierDto
        {
            Id = tier.Id,
            DepositProductId = tier.DepositProductId,
            TierName = tier.TierName,
            MinimumBalance = tier.MinimumBalance,
            MaximumBalance = tier.MaximumBalance,
            InterestRate = tier.InterestRate,
            TierBasis = tier.TierBasis,
            IsActive = tier.IsActive,
            DisplayOrder = tier.DisplayOrder,
            EffectiveFromDate = tier.EffectiveFromDate,
            EffectiveToDate = tier.EffectiveToDate,
            IsPromotional = tier.IsPromotional,
            IsEffective = tier.IsEffective()
        };
    }

    private async Task<FixedDepositDto> MapToFixedDepositDtoAsync(FixedDeposit deposit)
    {
        // Load related entities if not already loaded
        if (deposit.Customer == null)
        {
            deposit = await _unitOfWork.Repository<FixedDeposit>()
                .GetByIdAsync(deposit.Id) ?? deposit;
        }

        var daysToMaturity = (deposit.MaturityDate - DateTime.UtcNow).Days;

        return new FixedDepositDto
        {
            Id = deposit.Id,
            DepositNumber = deposit.DepositNumber,
            CustomerId = deposit.CustomerId,
            CustomerName = deposit.Customer?.UserName ?? string.Empty,
            DepositProductId = deposit.DepositProductId,
            ProductName = deposit.DepositProduct?.Name ?? string.Empty,
            LinkedAccountId = deposit.LinkedAccountId,
            LinkedAccountNumber = deposit.LinkedAccount?.AccountNumber ?? string.Empty,
            PrincipalAmount = deposit.PrincipalAmount,
            InterestRate = deposit.InterestRate,
            TermDays = deposit.TermDays,
            StartDate = deposit.StartDate,
            MaturityDate = deposit.MaturityDate,
            Status = deposit.Status,
            InterestCalculationMethod = deposit.InterestCalculationMethod,
            CompoundingFrequency = deposit.CompoundingFrequency,
            AccruedInterest = deposit.AccruedInterest,
            LastInterestCalculationDate = deposit.LastInterestCalculationDate,
            MaturityAction = deposit.MaturityAction,
            AutoRenewalEnabled = deposit.AutoRenewalEnabled,
            RenewalTermDays = deposit.RenewalTermDays,
            RenewalNoticeDate = deposit.RenewalNoticeDate,
            CustomerConsentReceived = deposit.CustomerConsentReceived,
            PenaltyType = deposit.PenaltyType,
            PenaltyAmount = deposit.PenaltyAmount,
            PenaltyPercentage = deposit.PenaltyPercentage,
            ClosureDate = deposit.ClosureDate,
            ClosureReason = deposit.ClosureReason,
            PenaltyApplied = deposit.PenaltyApplied,
            NetAmountPaid = deposit.NetAmountPaid,
            RenewalCount = deposit.RenewalCount,
            MaturityAmount = deposit.CalculateMaturityAmount(),
            InterestAtMaturity = deposit.CalculateInterestAtMaturity(),
            DaysToMaturity = Math.Max(0, daysToMaturity),
            HasMatured = deposit.HasMatured(),
            CreatedAt = deposit.CreatedAt,
            UpdatedAt = deposit.UpdatedAt
        };
    }

    private static DepositCertificateDto MapToDepositCertificateDto(DepositCertificate certificate)
    {
        return new DepositCertificateDto
        {
            Id = certificate.Id,
            FixedDepositId = certificate.FixedDepositId,
            CertificateNumber = certificate.CertificateNumber,
            Status = certificate.Status,
            IssueDate = certificate.IssueDate,
            DeliveryDate = certificate.DeliveryDate,
            DeliveryMethod = certificate.DeliveryMethod,
            DeliveryAddress = certificate.DeliveryAddress,
            DeliveryReference = certificate.DeliveryReference,
            PdfFileName = certificate.PdfFileName,
            HasPdf = certificate.CertificatePdf != null
        };
    }

    private static MaturityNoticeDto MapToMaturityNoticeDto(MaturityNotice notice)
    {
        return new MaturityNoticeDto
        {
            Id = notice.Id,
            FixedDepositId = notice.FixedDepositId,
            NoticeNumber = notice.NoticeNumber,
            NoticeType = notice.NoticeType,
            NoticeDate = notice.NoticeDate,
            MaturityDate = notice.MaturityDate,
            Status = notice.Status,
            Subject = notice.Subject,
            DeliveryChannel = notice.DeliveryChannel,
            DeliveryAddress = notice.DeliveryAddress,
            DeliveryDate = notice.DeliveryDate,
            DeliveryAttempts = notice.DeliveryAttempts,
            CustomerResponseDate = notice.CustomerResponseDate,
            CustomerChoice = notice.CustomerChoice,
            CustomerInstructions = notice.CustomerInstructions,
            ConsentReceived = notice.ConsentReceived
        };
    }

    #endregion
}

// Helper class for building predicates
public static class PredicateBuilder
{
    public static ExpressionStarter<T> New<T>(bool defaultExpression = false) => new(defaultExpression);
    public static ExpressionStarter<T> New<T>(Expression<Func<T, bool>> expression) => new(expression);
}

public class ExpressionStarter<T>
{
    private Expression<Func<T, bool>>? _predicate;

    internal ExpressionStarter(bool defaultExpression = false)
    {
        if (defaultExpression)
            _predicate = f => true;
        else
            _predicate = f => false;
    }

    internal ExpressionStarter(Expression<Func<T, bool>> expression)
    {
        _predicate = expression;
    }

    public ExpressionStarter<T> And(Expression<Func<T, bool>> expression)
    {
        if (_predicate == null)
            _predicate = expression;
        else
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(_predicate.Parameters[0], parameter);
            var left = leftVisitor.Visit(_predicate.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expression.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression.Body);
            _predicate = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left!, right!), parameter);
        }
        return this;
    }

    public static implicit operator Expression<Func<T, bool>>(ExpressionStarter<T> starter)
    {
        return starter._predicate ?? (f => false);
    }
}

public class ReplaceExpressionVisitor : ExpressionVisitor
{
    private readonly Expression _oldValue;
    private readonly Expression _newValue;

    public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override Expression? Visit(Expression? node)
    {
        return node == _oldValue ? _newValue : base.Visit(node);
    }
}