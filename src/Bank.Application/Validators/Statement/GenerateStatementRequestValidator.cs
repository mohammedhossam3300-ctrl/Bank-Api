using Bank.Application.DTOs.Statement.Core;
using Bank.Application.Validators.Base;
using Bank.Domain.Enums;
using FluentValidation;

namespace Bank.Application.Validators.Statement;

/// <summary>
/// Validator for generate statement requests
/// </summary>
public class GenerateStatementRequestValidator : AbstractValidator<GenerateStatementRequest>
{
    public GenerateStatementRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required")
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before end date");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("End date cannot be in the future");

        RuleFor(x => x.Format)
            .IsInEnum()
            .WithMessage("Invalid statement format");

        RuleFor(x => x.DeliveryMethod)
            .IsInEnum()
            .WithMessage("Invalid delivery method");

        // Email validation when delivery method is Email
        When(x => x.DeliveryMethod == StatementDeliveryMethod.Email, () =>
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage("Email address is required for email delivery")
                .EmailAddress()
                .WithMessage("Invalid email address format");
        });

        // Validate amount filters if provided
        When(x => x.MinAmount.HasValue || x.MaxAmount.HasValue, () =>
        {
            AmountRangeValidator.AddAmountRangeRules(this, x => x.MinAmount, x => x.MaxAmount);
        });

        // Validate custom title if provided
        RuleFor(x => x.CustomTitle)
            .MaximumLength(200)
            .WithMessage("Custom title cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.CustomTitle));

        // Validate request reason if provided
        RuleFor(x => x.RequestReason)
            .MaximumLength(500)
            .WithMessage("Request reason cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.RequestReason));

        // Validate filter by description if provided
        RuleFor(x => x.FilterByDescription)
            .MaximumLength(200)
            .WithMessage("Filter description cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.FilterByDescription));

        // Validate date range is not too large (max 5 years)
        RuleFor(x => x)
            .Must(x => (x.EndDate - x.StartDate).TotalDays <= 1825)
            .WithMessage("Date range cannot exceed 5 years")
            .WithName("DateRange");
    }
}
