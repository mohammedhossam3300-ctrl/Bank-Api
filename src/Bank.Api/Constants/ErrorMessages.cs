namespace Bank.Api.Constants;

/// <summary>
/// Error messages used throughout the API layer
/// </summary>
public static class ErrorMessages
{
    // Access and Authorization
    public const string YouDontHaveAccessToThisAccount = "You don't have access to this account";
    public const string UnauthorizedAccess = "Unauthorized access";
    public const string AccessDenied = "Access denied";
    public const string InsufficientPermissions = "Insufficient permissions";
    
    // User-related
    public const string UserNotFound = "User not found";
    public const string UserAlreadyExists = "User already exists";
    public const string UserAccountLocked = "User account is locked";
    public const string UserAccountDisabled = "User account is disabled";
    public const string InvalidUserCredentials = "Invalid user credentials";
    
    // Account-related
    public const string AccountNotFound = "Account not found";
    public const string InvalidAccountNumber = "Invalid account number";
    public const string AccountLocked = "Account is locked";
    public const string InsufficientFunds = "Insufficient funds";
    public const string InvalidAccountStatus = "Invalid account status";
    
    // Operation-related
    public const string OperationFailed = "Operation failed";
    public const string InvalidOperation = "Invalid operation";
    public const string OperationNotAllowed = "Operation not allowed";
    public const string TransactionFailed = "Transaction failed";
    
    // Validation errors
    public const string InvalidInput = "Invalid input";
    public const string RequiredFieldMissing = "Required field is missing";
    public const string InvalidFormat = "Invalid format";
    
    // Service errors
    public const string ServiceUnavailable = "Service is temporarily unavailable";
    public const string InternalServerError = "An internal server error occurred";
}
