namespace Bank.Api.Constants;

/// <summary>
/// API-wide constants for response messages, error messages, and common strings.
/// Centralizes string literals used across controllers to improve maintainability.
/// </summary>
public static class ApiConstants
{
    // ── Authentication & Authorization ──
    public const string UserNotAuthenticatedMessage = "User not authenticated";
    public const string UnauthorizedAccessMessage = "You are not authorized to access this resource";
    public const string TokenExpiredMessage = "Your session has expired. Please log in again";
    public const string InvalidCredentialsMessage = "Invalid email or password";
    
    // ── PIN Management ──
    public const string PinSetErrorMessage = "An error occurred while setting PIN";
    public const string PinChangeErrorMessage = "An error occurred while changing PIN";
    public const string PinResetErrorMessage = "An error occurred while resetting PIN";
    public const string PinVerifyErrorMessage = "An error occurred while verifying PIN";
    public const string PinGenerationErrorMessage = "An error occurred while generating verification code";
    
    // ── Card Operations ──
    public const string CardNotFoundMessage = "Card not found";
    public const string CardBlockedMessage = "Card is blocked";
    public const string CardExpiredMessage = "Card has expired";
    public const string CardUnblockErrorMessage = "An error occurred while unblocking the card";
    public const string CardBlockErrorMessage = "An error occurred while blocking the card";
    
    // ── Account Operations ──
    public const string AccountNotFoundMessage = "Account not found";
    public const string InsufficientFundsMessage = "Insufficient funds for this transaction";
    public const string AccountNotAccessibleMessage = "You can only access your own account";
    
    // ── Deposit Operations ──
    public const string DepositNotFoundMessage = "Deposit not found";
    public const string YouCanOnlyAccessYourOwnDepositsMessage = "You can only access your own deposits";
    
    // ── Admin Messages ──
    public const string AdminRoleRequired = "Admin";
    
    // ── Payment Operations ──
    public const string BillPaymentNotFoundMessage = "Bill payment not found";
    public const string ReceiptNotFoundMessage = "Receipt not found";
    public const string BillerNotFoundMessage = "Biller not found";
    public const string BillPresentmentNotFoundMessage = "Bill presentment not found";
    
    // ── Audit Operations ──
    public const string AuditLogRetrievalErrorMessage = "An error occurred while retrieving audit logs";
    
    // ── Joint Account Operations ──
    public const string UnknownErrorMessage = "Unknown";
    
    // ── Success Messages ──
    public const string OperationSuccessfulMessage = "Operation completed successfully";
    public const string ResourceCreatedMessage = "Resource created successfully";
    public const string ResourceUpdatedMessage = "Resource updated successfully";
    public const string ResourceDeletedMessage = "Resource deleted successfully";
}
