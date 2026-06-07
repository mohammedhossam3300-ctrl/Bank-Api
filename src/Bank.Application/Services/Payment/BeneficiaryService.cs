using AutoMapper;
using Bank.Application.DTOs;
using Bank.Application.DTOs.Payment.Beneficiary;
using Bank.Application.Helpers.Shared;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bank.Application.Services
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IRepository<Beneficiary> _beneficiaryRepository;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public BeneficiaryService(
            IRepository<Beneficiary> beneficiaryRepository,
            IMapper mapper,
            IAuditLogService auditLogService)
        {
            _beneficiaryRepository = beneficiaryRepository;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<BeneficiaryResult> AddBeneficiaryAsync(Guid customerId, AddBeneficiaryRequest request)
        {
            var beneficiary = new Beneficiary
            {
                CustomerId = customerId,
                Name = request.Name,
                AccountNumber = request.AccountNumber,
                BankCode = request.BankCode,
                Nickname = request.Nickname,
                Status = BeneficiaryStatus.Pending,
                IsVerified = false
            };

            await _beneficiaryRepository.AddAsync(beneficiary);
            
            await _auditLogService.LogActivityAsync(customerId, "AddBeneficiary", "Beneficiary", beneficiary.Id.ToString(), "Added new beneficiary");

            return new BeneficiaryResult
            {
                Success = true,
                Beneficiary = _mapper.Map<BeneficiaryDto>(beneficiary),
                Message = "Beneficiary added successfully"
            };
        }

        public async Task<BeneficiaryResult> UpdateBeneficiaryAsync(Guid beneficiaryId, UpdateBeneficiaryRequest request, Guid updatedByUserId)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
            if (beneficiary == null)
                return new BeneficiaryResult { Success = false, Message = "Beneficiary not found" };

            beneficiary.Name = request.Name ?? beneficiary.Name;
            beneficiary.Nickname = request.Nickname ?? beneficiary.Nickname;
            
            _beneficiaryRepository.Update(beneficiary);

            await _auditLogService.LogActivityAsync(updatedByUserId, "UpdateBeneficiary", "Beneficiary", beneficiary.Id.ToString(), "Updated beneficiary details");

            return new BeneficiaryResult
            {
                Success = true,
                Beneficiary = _mapper.Map<BeneficiaryDto>(beneficiary),
                Message = "Beneficiary updated successfully"
            };
        }

        public async Task<BeneficiaryDto?> GetBeneficiaryByIdAsync(Guid beneficiaryId)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
            return beneficiary == null ? null : _mapper.Map<BeneficiaryDto>(beneficiary);
        }

        public async Task<List<BeneficiaryDto>> GetCustomerBeneficiariesAsync(Guid customerId, bool activeOnly = true)
        {
            Expression<Func<Beneficiary, bool>> predicate = b => b.CustomerId == customerId;
            if (activeOnly)
            {
                predicate = b => b.CustomerId == customerId && b.Status == BeneficiaryStatus.Active;
            }

            var beneficiaries = await _beneficiaryRepository.FindAsync(predicate);
            return _mapper.Map<List<BeneficiaryDto>>(beneficiaries.ToList());
        }

        public async Task<BeneficiarySearchResult> SearchBeneficiariesAsync(BeneficiarySearchCriteria criteria)
        {
            var (items, count) = await _beneficiaryRepository.ListAsync(b => b.CustomerId == criteria.CustomerId && 
                                                                             (string.IsNullOrEmpty(criteria.SearchTerm) || b.Name.Contains(criteria.SearchTerm) || b.AccountNumber.Contains(criteria.SearchTerm)), 
                                                                             criteria.PageNumber, criteria.PageSize);

            return new BeneficiarySearchResult
            {
                Beneficiaries = _mapper.Map<List<BeneficiaryDto>>(items.ToList()),
                TotalCount = count,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize
            };
        }

        public async Task<BeneficiaryVerificationResult> VerifyBeneficiaryAsync(Guid beneficiaryId, Guid verifiedByUserId)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
            if (beneficiary == null)
                return new BeneficiaryVerificationResult { Success = false };

            beneficiary.IsVerified = true;
            beneficiary.VerifiedAt = DateTime.UtcNow;
            beneficiary.VerifiedBy = verifiedByUserId;
            beneficiary.Status = BeneficiaryStatus.Active;

            _beneficiaryRepository.Update(beneficiary);
            
            return new BeneficiaryVerificationResult
            {
                Success = true,
                IsAccountValid = true,
                AccountHolderName = beneficiary.Name
            };
        }

        public async Task<bool> ArchiveBeneficiaryAsync(Guid beneficiaryId, string reason, Guid archivedByUserId)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
            if (beneficiary == null) return false;

            beneficiary.Status = BeneficiaryStatus.Archived;
            _beneficiaryRepository.Update(beneficiary);
            
            await _auditLogService.LogActivityAsync(archivedByUserId, "ArchiveBeneficiary", "Beneficiary", beneficiaryId.ToString(), reason);
            
            return true;
        }

        public async Task<bool> ReactivateBeneficiaryAsync(Guid beneficiaryId, Guid reactivatedByUserId)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
            if (beneficiary == null) return false;

            beneficiary.Status = BeneficiaryStatus.Active;
            _beneficiaryRepository.Update(beneficiary);
            
            await _auditLogService.LogActivityAsync(reactivatedByUserId, "ReactivateBeneficiary", "Beneficiary", beneficiaryId.ToString(), "Reactivated beneficiary");
            
            return true;
        }

        public async Task<bool> CanReceiveTransfersAsync(Guid beneficiaryId)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
            return beneficiary != null && beneficiary.Status == BeneficiaryStatus.Active;
        }

        public async Task<bool> ValidateTransferLimitsAsync(Guid beneficiaryId, decimal amount)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
            if (beneficiary == null) return false;

            if (beneficiary.DailyTransferLimit.HasValue && amount > beneficiary.DailyTransferLimit.Value) return false;
            if (beneficiary.MonthlyTransferLimit.HasValue && (beneficiary.TotalTransferAmount + amount) > beneficiary.MonthlyTransferLimit.Value) return false;

            return true;
        }

        public async Task RecordTransferAsync(Guid beneficiaryId, decimal amount)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
            if (beneficiary != null)
            {
                beneficiary.TotalTransferAmount += amount;
                beneficiary.TotalTransferCount += 1;
                beneficiary.LastTransferAmount = amount;
                beneficiary.LastTransferDate = DateTime.UtcNow;
                _beneficiaryRepository.Update(beneficiary);
            }
        }

        public async Task<BeneficiaryTransferHistory> GetTransferHistoryAsync(Guid beneficiaryId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            // Placeholder: In a real app, this would query a transactions table linked to the beneficiary
            return new BeneficiaryTransferHistory
            {
                BeneficiaryId = beneficiaryId,
                Transfers = new List<TransferHistoryItem>()
            };
        }

        public async Task<BeneficiaryStatistics> GetBeneficiaryStatisticsAsync(Guid customerId)
        {
            var beneficiaries = await _beneficiaryRepository.FindAsync(b => b.CustomerId == customerId);
            var list = beneficiaries.ToList();
            
            return new BeneficiaryStatistics
            {
                TotalBeneficiaries = list.Count,
                VerifiedBeneficiaries = list.Count(b => b.IsVerified),
                TotalTransferAmount = list.Sum(b => b.TotalTransferAmount)
            };
        }

        public async Task<BeneficiaryVerificationResult> ValidateAccountDetailsAsync(AddBeneficiaryRequest request)
        {
            // Implementation for account validation
            return new BeneficiaryVerificationResult
            {
                Success = true,
                IsAccountValid = true,
                AccountHolderName = "Validation Placeholder"
            };
        }

        public async Task<bool> UpdateTransferLimitsAsync(Guid beneficiaryId, decimal? dailyLimit, decimal? monthlyLimit, decimal? singleLimit, Guid updatedByUserId)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
            if (beneficiary == null) return false;

            beneficiary.DailyTransferLimit = dailyLimit;
            beneficiary.MonthlyTransferLimit = monthlyLimit;
            beneficiary.SingleTransferLimit = singleLimit;
            
            _beneficiaryRepository.Update(beneficiary);
            return true;
        }
    }
}