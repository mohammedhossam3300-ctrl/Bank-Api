using System.Text.RegularExpressions;

namespace Bank.Application.Validators.Shared;

/// <summary>
/// Validator for email format
/// </summary>
public static class EmailValidator
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Validates email format
    /// </summary>
    /// <param name="email">Email to validate</param>
    /// <returns>True if email format is valid</returns>
    public static bool ValidateFormat(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, RegexTimeout);
            return emailRegex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }
}
