using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Loan;

/// <summary>
/// Controller for loan analytics and reporting
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoanAnalyticsController : ControllerBase
{
    private readonly ILoanAnalyticsService _loanAnalyticsService;

    public LoanAnalyticsController(
        ILoanAnalyticsService loanAnalyticsService)
    {
        _loanAnalyticsService = loanAnalyticsService;
    }

    /// <summary>
    /// Get comprehensive loan analytics
    /// </summary>
    [HttpGet("overview")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<LoanAnalyticsDto>> GetLoanAnalytics([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var analytics = await _loanAnalyticsService.GetLoanAnalyticsAsync(fromDate, toDate);
        return Ok(analytics);
    }

    /// <summary>
    /// Get loan performance metrics for a specific loan
    /// </summary>
    [HttpGet("performance/{loanId}")]
    public async Task<ActionResult<LoanPerformanceMetrics>> GetLoanPerformance(Guid loanId)
    {
        var performance = await _loanAnalyticsService.GetLoanPerformanceAsync(loanId);
        return Ok(performance);
    }

    /// <summary>
    /// Get loan portfolio summary by type
    /// </summary>
    [HttpGet("portfolio-by-type")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<Dictionary<LoanType, LoanAnalyticsDto>>> GetPortfolioByType()
    {
        var portfolio = await _loanAnalyticsService.GetPortfolioByTypeAsync();
        return Ok(portfolio);
    }

    /// <summary>
    /// Get delinquency report
    /// </summary>
    [HttpGet("delinquency-report")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<LoanDto>>> GetDelinquencyReport()
    {
        var report = await _loanAnalyticsService.GetDelinquencyReportAsync();
        return Ok(report);
    }

    /// <summary>
    /// Get loans approaching maturity
    /// </summary>
    [HttpGet("approaching-maturity")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<LoanDto>>> GetLoansApproachingMaturity([FromQuery] int daysAhead = 30)
    {
        var loans = await _loanAnalyticsService.GetLoansApproachingMaturityAsync(daysAhead);
        return Ok(loans);
    }

    /// <summary>
    /// Calculate portfolio risk metrics
    /// </summary>
    [HttpGet("portfolio-risk")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<PortfolioRiskMetrics>> GetPortfolioRisk()
    {
        var risk = await _loanAnalyticsService.CalculatePortfolioRiskAsync();
        return Ok(risk);
    }

    /// <summary>
    /// Get loan origination trends
    /// </summary>
    [HttpGet("origination-trends")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<LoanOriginationTrend>>> GetOriginationTrends([FromQuery] int months = 12)
    {
        var trends = await _loanAnalyticsService.GetOriginationTrendsAsync(months);
        return Ok(trends);
    }

    /// <summary>
    /// Get customer loan summary
    /// </summary>
    [HttpGet("customer-summary/{customerId}")]
    public async Task<ActionResult<CustomerLoanSummary>> GetCustomerLoanSummary(Guid customerId)
    {
        // Ensure user can only access their own data or is admin/manager
        var currentUserId = GetCurrentUserId();
        if (currentUserId != customerId && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
        {
            return Forbid("You can only access your own loan summary");
        }

        var summary = await _loanAnalyticsService.GetCustomerLoanSummaryAsync(customerId);
        return Ok(summary);
    }

    /// <summary>
    /// Get my loan summary (current user)
    /// </summary>
    [HttpGet("my-summary")]
    public async Task<ActionResult<CustomerLoanSummary>> GetMyLoanSummary()
    {
        var customerId = GetCurrentUserId();
        var summary = await _loanAnalyticsService.GetCustomerLoanSummaryAsync(customerId);
        return Ok(summary);
    }

    #region Private Helper Methods

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
    }

    #endregion
}