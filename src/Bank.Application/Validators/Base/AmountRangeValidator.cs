using FluentValidation;

namespace Bank.Application.Validators.Base;

/// <summary>
/// Base validator for amount range validation (MinAmount/MaxAmount)
/// </summary>
public static class AmountRangeValidator
{
    /// <summary>
    /// Adds amount range validation rules to a validator
    /// </summary>
    public static void AddAmountRangeRules<T>(AbstractValidator<T> validator, 
        Func<T, decimal?> minAmountSelector, 
        Func<T, decimal?> maxAmountSelector) where T : class
    {
        validator.RuleFor(x => minAmountSelector(x))
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum amount cannot be negative")
            .When(x => minAmountSelector(x).HasValue);

        validator.RuleFor(x => maxAmountSelector(x))
            .GreaterThanOrEqualTo(0)
            .WithMessage("Maximum amount cannot be negative")
            .When(x => maxAmountSelector(x).HasValue);

        validator.RuleFor(x => x)
            .Must(x => !minAmountSelector(x).HasValue || !maxAmountSelector(x).HasValue || minAmountSelector(x) <= maxAmountSelector(x))
            .WithMessage("Minimum amount must be less than or equal to maximum amount")
            .WithName("AmountRange");
    }
}
