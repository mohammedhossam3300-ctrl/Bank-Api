using System.Text.RegularExpressions;

namespace Bank.Application.Validators.Shared;

/// <summary>
/// Validator for phone number format
/// </summary>
public static class PhoneNumberValidator
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Validates phone number format
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>True if phone number format is valid</returns>
    public static bool ValidateFormat(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Remove common formatting characters
        var cleaned = Regex.Replace(phoneNumber, @"[\s\-\(\)\+\.]", "", RegexOptions.None, RegexTimeout);
        
        // Check if it's all digits and reasonable length
        return cleaned.All(char.IsDigit) && cleaned.Length >= 10 && cleaned.Length <= 15;
    }
}
