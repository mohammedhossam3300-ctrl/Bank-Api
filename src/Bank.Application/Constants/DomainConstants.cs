namespace Bank.Application.Constants;

/// <summary>
/// Domain-wide constants for error messages, role names, and common strings
/// </summary>
public static class DomainConstants
{
    // Authentication & Authorization
    public const string ADMIN_ROLE = "Admin";
    public const string USER_ROLE = "User";
    public const string MANAGER_ROLE = "Manager";
    public const string AUDITOR_ROLE = "Auditor";

    // Access & Permissions
    public const string ACCESS_DENIED = "You don't have access to this account";
    public const string USER_NOT_AUTHENTICATED = "User not authenticated";
    public const string USER_NOT_FOUND = "User not found";

    // Account & Deposit
    public const string ACCOUNT_PREFIX = "Account";
    public const string FIXED_DEPOSIT_PREFIX = "FixedDeposit";
    public const string ACCESS_OWN_DEPOSITS = "You can only access your own deposits";

    // Card Operations
    public const string CARD_NOT_FOUND = "Card not found";
    public const string INVALID_CARD_ID = "Invalid card ID";
    public const string CARD_NOT_FOUND_OR_DENIED = "Card not found or access denied";

    // Payment & Bills
    public const string RECEIPT_NOT_FOUND = "Receipt not found";
    public const string BILL_PRESENTMENT_NOT_FOUND = "Bill presentment not found";
    public const string BILLER_NOT_FOUND = "Biller not found";

    // Audit & Logging
    public const string AUDIT_ERROR_MESSAGE = "An error occurred while retrieving audit logs";

    // IP Whitelist
    public const string IP_WHITELIST_PREFIX = "IpWhitelist";

    // Generic/Utility
    public const string UNKNOWN = "Unknown";
}
