using Bank.Domain.Enums;

namespace Bank.Application.DTOs.Deposit.Core;

/// <summary>
/// Deposit transaction data transfer object
/// </summary>
public class DepositTransactionDto : BaseDepositTransactionDto
{
    public Guid DepositId { get; set; }
    public Guid Id { get; set; }
    public Guid FixedDepositId { get; set; }
}



