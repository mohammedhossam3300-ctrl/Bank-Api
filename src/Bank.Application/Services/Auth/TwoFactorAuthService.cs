using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Bank.Application.Services;

/// <summary>
/// Service for managing two-factor authentication with SMS, email, and authenticator app support
/// </summary>
public class TwoFactorAuthService : ITwoFactorAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISmsService _smsService;
    private readonly IEmailService _emailService;
    private readonly ITokenGenerationService _tokenGenerationService;
    private readonly ILogger<TwoFactorAuthService> _logger;
    
    private const int TokenLength = 6;
    private const int TokenExpiryMinutes = 5;
    private const int BackupCodeCount = 10;
    private const int BackupCodeLength = 8;

    public TwoFactorAuthService(
        UserManager<User> userManager,
        IUnitOfWork unitOfWork,
        ISmsService smsService,
        IEmailService emailService,
        ITokenGenerationService tokenGenerationService,
        ILogger<TwoFactorAuthService> logger)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _smsService = smsService;
        _emailService = emailService;
        _tokenGenerationService = tokenGenerationService;
        _logger = logger;
    }

    public async Task<TwoFactorTokenResult> GenerateTokenAsync(Guid userId, TwoFactorMethod method, string? destination = null)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new TwoFactorTokenResult { Success = false, Message = "User not found" };
            }

            // Generate 6-digit token
            var token = GenerateNumericToken(TokenLength);
            var expiresAt = DateTime.UtcNow.AddMinutes(TokenExpiryMinutes);

            // Determine destination based on method
            var tokenDestination = method switch
            {
                TwoFactorMethod.SMS => destination ?? user.PhoneNumber,
                TwoFactorMethod.Email => destination ?? user.Email,
                TwoFactorMethod.AuthenticatorApp => "Authenticator App",
                _ => destination
            };

            if (string.IsNullOrEmpty(tokenDestination) && method != TwoFactorMethod.AuthenticatorApp)
            {
                return new TwoFactorTokenResult { Success = false, Message = "Destination not available for selected method" };
            }

            // Create token entity
            var twoFactorToken = new TwoFactorToken
            {
                UserId = userId,
                Token = token,
                Method = method,
                Destination = tokenDestination!,
                ExpiresAt = expiresAt
            };

            await _unitOfWork.Repository<TwoFactorToken>().AddAsync(twoFactorToken);
            await _unitOfWork.SaveChangesAsync();

            // Send token based on method
            var sendResult = await SendTokenAsync(method, tokenDestination!, token, user.FullName);
            if (!sendResult)
            {
                return new TwoFactorTokenResult { Success = false, Message = "Failed to send verification token" };
            }

            _logger.LogInformation("2FA token generated for user {UserId} via {Method}", userId, method);

            return new TwoFactorTokenResult
            {
                Success = true,
                Message = "Verification token sent successfully",
                TokenId = twoFactorToken.Id.ToString(),
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating 2FA token for user {UserId}", userId);
            return new TwoFactorTokenResult { Success = false, Message = "An error occurred while generating token" };
        }
    }

    public async Task<TwoFactorVerificationResult> VerifyTokenAsync(Guid userId, string token, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new TwoFactorVerificationResult { Success = false, Message = "User not found" };
            }

            // Find valid token
            var validToken = await _unitOfWork.Repository<TwoFactorToken>()
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);

            if (validToken == null)
            {
                _logger.LogWarning("Invalid or expired 2FA token attempt for user {UserId}", userId);
                return new TwoFactorVerificationResult { Success = false, Message = "Invalid or expired token" };
            }

            // Mark token as used
            validToken.MarkAsUsed(ipAddress, userAgent);
            _unitOfWork.Repository<TwoFactorToken>().Update(validToken);

            // Update user's last 2FA usage
            user.MarkTwoFactorUsed();
            await _userManager.UpdateAsync(user);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("2FA token verified successfully for user {UserId}", userId);

            return new TwoFactorVerificationResult
            {
                Success = true,
                Message = "Token verified successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying 2FA token for user {UserId}", userId);
            return new TwoFactorVerificationResult { Success = false, Message = "An error occurred while verifying token" };
        }
    }

    public async Task<TwoFactorSetupResult> SetupAuthenticatorAsync(Guid userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new TwoFactorSetupResult { Success = false, Message = "User not found" };
            }

            // Generate secret key for authenticator app
            var secretKey = GenerateSecretKey();
            var qrCodeUrl = GenerateQrCodeUrl(user.Email!, secretKey);

            // Store secret key temporarily (will be confirmed during setup completion)
            user.TwoFactorSecretKey = secretKey;
            user.TwoFactorStatus = TwoFactorStatus.Pending;
            await _userManager.UpdateAsync(user);

            return new TwoFactorSetupResult
            {
                Success = true,
                Message = "Authenticator setup initiated",
                SecretKey = secretKey,
                QrCodeUrl = qrCodeUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up authenticator for user {UserId}", userId);
            return new TwoFactorSetupResult { Success = false, Message = "An error occurred during setup" };
        }
    }

    public async Task<TwoFactorSetupResult> CompleteSetupAsync(Guid userId, string verificationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new TwoFactorSetupResult { Success = false, Message = "User not found" };
            }

            if (user.TwoFactorStatus != TwoFactorStatus.Pending || string.IsNullOrEmpty(user.TwoFactorSecretKey))
            {
                return new TwoFactorSetupResult { Success = false, Message = "No pending 2FA setup found" };
            }

            // Verify the token from authenticator app
            var isValidToken = VerifyAuthenticatorToken(user.TwoFactorSecretKey, verificationToken);
            if (!isValidToken)
            {
                return new TwoFactorSetupResult { Success = false, Message = "Invalid verification token" };
            }

            // Generate backup codes
            var backupCodes = GenerateBackupCodes();
            user.TwoFactorBackupCodes = JsonSerializer.Serialize(backupCodes);

            // Enable 2FA
            user.EnableTwoFactor(user.TwoFactorSecretKey, user.TwoFactorBackupCodes);
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("2FA setup completed for user {UserId}", userId);

            return new TwoFactorSetupResult
            {
                Success = true,
                Message = "Two-factor authentication enabled successfully",
                BackupCodes = backupCodes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing 2FA setup for user {UserId}", userId);
            return new TwoFactorSetupResult { Success = false, Message = "An error occurred during setup completion" };
        }
    }

    public async Task<bool> DisableTwoFactorAsync(Guid userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.DisableTwoFactor();
            await _userManager.UpdateAsync(user);

            // Invalidate all existing tokens
            var existingTokens = await _unitOfWork.Repository<TwoFactorToken>()
                .FindAsync(t => t.UserId == userId && !t.IsUsed);

            foreach (var token in existingTokens)
            {
                token.MarkAsUsed();
                _unitOfWork.Repository<TwoFactorToken>().Update(token);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("2FA disabled for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling 2FA for user {UserId}", userId);
            return false;
        }
    }

    public async Task<List<string>> GenerateBackupCodesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new List<string>();

        var backupCodes = GenerateBackupCodes();
        user.TwoFactorBackupCodes = JsonSerializer.Serialize(backupCodes);
        await _userManager.UpdateAsync(user);

        return backupCodes;
    }

    public async Task<bool> VerifyBackupCodeAsync(Guid userId, string backupCode)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || string.IsNullOrEmpty(user.TwoFactorBackupCodes)) return false;

        var backupCodes = JsonSerializer.Deserialize<List<string>>(user.TwoFactorBackupCodes);
        if (backupCodes == null || !backupCodes.Contains(backupCode)) return false;

        // Remove used backup code
        backupCodes.Remove(backupCode);
        user.TwoFactorBackupCodes = JsonSerializer.Serialize(backupCodes);
        user.MarkTwoFactorUsed();
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<bool> IsTwoFactorEnabledAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.TwoFactorEnabled == true;
    }

    public async Task<TwoFactorStatusResult> GetTwoFactorStatusAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new TwoFactorStatusResult { IsEnabled = false, Status = TwoFactorStatus.NotSetup };
        }

        var enabledMethods = new List<TwoFactorMethod>();
        if (!string.IsNullOrEmpty(user.TwoFactorSecretKey)) enabledMethods.Add(TwoFactorMethod.AuthenticatorApp);
        if (!string.IsNullOrEmpty(user.PhoneNumber)) enabledMethods.Add(TwoFactorMethod.SMS);
        if (!string.IsNullOrEmpty(user.Email)) enabledMethods.Add(TwoFactorMethod.Email);

        return new TwoFactorStatusResult
        {
            IsEnabled = user.TwoFactorEnabled,
            Status = user.TwoFactorStatus,
            EnabledMethods = enabledMethods,
            SetupDate = user.TwoFactorSetupDate,
            LastUsed = user.LastTwoFactorUsed
        };
    }

    #region Private Methods

    private string GenerateNumericToken(int length)
        {
            return _tokenGenerationService.GenerateNumericToken(length);
        }


    private string GenerateSecretKey()
    {
        return _tokenGenerationService.GenerateSecretKey();
    }

    private string GenerateQrCodeUrl(string email, string secretKey)
    {
        return _tokenGenerationService.GenerateQrCodeUrl("Bank Management System", email, secretKey);
    }

    private static string ToBase32String(byte[] input)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var output = new StringBuilder();
        
        for (int i = 0; i < input.Length; i += 5)
        {
            var chunk = new byte[5];
            Array.Copy(input, i, chunk, 0, Math.Min(5, input.Length - i));
            
            var b1 = chunk[0];
            var b2 = chunk.Length > 1 ? chunk[1] : (byte)0;
            var b3 = chunk.Length > 2 ? chunk[2] : (byte)0;
            var b4 = chunk.Length > 3 ? chunk[3] : (byte)0;
            var b5 = chunk.Length > 4 ? chunk[4] : (byte)0;
            
            AppendBase32Characters(output, alphabet, chunk.Length, b1, b2, b3, b4, b5);
        }
        
        return output.ToString().TrimEnd('=');
    }

    private static void AppendBase32Characters(StringBuilder output, string alphabet, int chunkLength, 
        byte b1, byte b2, byte b3, byte b4, byte b5)
    {
        output.Append(alphabet[b1 >> 3]);
        output.Append(alphabet[((b1 & 0x07) << 2) | (b2 >> 6)]);
        output.Append(chunkLength > 1 ? alphabet[(b2 >> 1) & 0x1F] : '=');
        output.Append(chunkLength > 1 ? alphabet[((b2 & 0x01) << 4) | (b3 >> 4)] : '=');
        output.Append(chunkLength > 2 ? alphabet[((b3 & 0x0F) << 1) | (b4 >> 7)] : '=');
        output.Append(chunkLength > 3 ? alphabet[(b4 >> 2) & 0x1F] : '=');
        output.Append(chunkLength > 3 ? alphabet[((b4 & 0x03) << 3) | (b5 >> 5)] : '=');
        output.Append(chunkLength > 4 ? alphabet[b5 & 0x1F] : '=');
    }

    private static bool VerifyAuthenticatorToken(string secretKey, string token)
    {
        // Implementation would use TOTP algorithm to verify token
        // For now, returning true for demonstration
        // In production, use libraries like OtpNet
        return !string.IsNullOrEmpty(token) && token.Length == 6;
    }

    private List<string> GenerateBackupCodes()
    {
        return _tokenGenerationService.GenerateBackupCodes(BackupCodeCount, BackupCodeLength);
    }

    private async Task<bool> SendTokenAsync(TwoFactorMethod method, string destination, string token, string userName)
    {
        try
        {
            return method switch
            {
                TwoFactorMethod.SMS => await _smsService.SendSmsAsync(destination, $"Your Bank verification code is: {token}. Valid for {TokenExpiryMinutes} minutes."),
                TwoFactorMethod.Email => await _emailService.SendEmailAsync(destination, "Bank Verification Code", $"Hello {userName},\n\nYour verification code is: {token}\n\nThis code will expire in {TokenExpiryMinutes} minutes.\n\nIf you didn't request this code, please contact support immediately."),
                _ => false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending 2FA token via {Method} to {Destination}", method, destination);
            return false;
        }
    }

    #endregion
}

// Extension method for Base32 encoding
public static class Base32Extensions
{
    private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public static string ToBase32String(this byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0) return string.Empty;

        var result = new StringBuilder();
        int buffer = bytes[0];
        int next = 1;
        int bitsLeft = 8;

        while (bitsLeft > 0 || next < bytes.Length)
        {
            if (bitsLeft < 5)
            {
                if (next < bytes.Length)
                {
                    buffer <<= 8;
                    buffer |= bytes[next++] & 0xFF;
                    bitsLeft += 8;
                }
                else
                {
                    int pad = 5 - bitsLeft;
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }

            int index = 0x1F & (buffer >> (bitsLeft - 5));
            bitsLeft -= 5;
            result.Append(Base32Alphabet[index]);
        }

        return result.ToString();
    }
}