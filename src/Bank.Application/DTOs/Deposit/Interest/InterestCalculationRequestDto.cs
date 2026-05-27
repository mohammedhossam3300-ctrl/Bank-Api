namespace Bank.Application.DTOs.Deposit.Interest;

/// <summary>
/// Request to calculate interest for a period
/// </summary>
public class InterestCalculationRequest
{
    public Guid AccountId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal InterestRate { get; set; }
    public decimal Principal { get; set; }
    public int? CompoundingFrequency { get; set; }

    // Aliases used by controller
    public DateTime FromDate { get => StartDate; set => StartDate = value; }
    public DateTime ToDate { get => EndDate; set => EndDate = value; }
}
