using Bank.Domain.Enums;

namespace Bank.Application.DTOs;

/// <summary>
/// Request DTO for creating a beneficiary
/// </summary>
public class CreateBeneficiaryRequest { }

/// <summary>
/// Request DTO for creating a bill payment
/// </summary>
public class CreateBillPaymentRequest { }

public class CreateJointAccountRequest 
{ 
    public Guid AccountId { get; set; }
    public Guid SecondaryOwnerId { get; set; }
}

public class JointAccountDto 
{ 
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid PrimaryOwnerId { get; set; }
    public Guid SecondaryOwnerId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request DTO for updating card information
/// </summary>
public class UpdateCardRequest { }

/// <summary>
/// Request DTO for creating a new card
/// </summary>
public class CreateCardRequest { }

/// <summary>
/// Request DTO for creating a new deposit
/// </summary>
public class CreateDepositRequest { }

public class DepositDto 
{ 
    public Guid Id { get; set; }
    public string DepositNumber { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public DateTime MaturityDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for updating loan information
/// </summary>
public class UpdateLoanRequest { }

/// <summary>
/// Request DTO for creating a new loan
/// </summary>
public class CreateLoanRequest { }

/// <summary>
/// Request DTO for creating a loan payment
/// </summary>
public class CreateLoanPaymentRequest { }

public class LoanPaymentDto 
{ 
    public Guid Id { get; set; }
    public Guid LoanId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = string.Empty; 
}

public class TwoFactorTokenDto 
{ 
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class CreateTwoFactorTokenRequest 
{ 
    public Guid UserId { get; set; }
    public string Purpose { get; set; } = string.Empty;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for creating a new user account
/// </summary>
public class CreateUserRequest { }

/// <summary>
/// Request DTO for updating user information
/// </summary>
public class UpdateUserRequest { }

/// <summary>
/// Request DTO for creating a statement
/// </summary>
public class CreateStatementRequest { }

public class JointAccountHolderDetailsDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public JointAccountRole Role { get; set; }
    public JointAccountAccessLevel AccessLevel { get; set; }
    public decimal? TransactionLimit { get; set; }
    public bool IsActive { get; set; }
    public DateTime JoinedDate { get; set; }
}
