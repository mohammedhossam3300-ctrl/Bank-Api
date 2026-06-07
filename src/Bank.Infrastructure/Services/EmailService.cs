using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Bank.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Services;

/// <summary>
/// Email service implementation using SMTP with mandatory SSL/TLS encryption.
/// 
/// SECURITY: EnableSsl is ALWAYS set to true. SMTP connections MUST use encrypted channels.
/// This protects email credentials and message content from interception (OWASP A2, CWE-319, STIG V-222596).
/// </summary>
public class EmailService : IEmailService, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpClient _smtpClient;
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(500);

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Configure SMTP client with MANDATORY SSL/TLS encryption
        var smtpHost = _configuration["Email:SmtpHost"] ?? "localhost";
        var smtpPortStr = _configuration["Email:SmtpPort"] ?? "587";
        var smtpPort = int.TryParse(smtpPortStr, out var parsedPort) ? parsedPort : 587;
        var username = _configuration["Email:Username"] ?? "noreply@bankapp.com";
        var password = _configuration["Email:Password"] ?? "password";

        _smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            // SECURITY: EnableSsl is ALWAYS true - no exceptions
            // Port 587 (submission) and 465 (SMTPS) both require encrypted connections
            EnableSsl = true,
            UseDefaultCredentials = string.IsNullOrEmpty(username),
            Credentials = !string.IsNullOrEmpty(username) ? new NetworkCredential(username, password) : null,
            // Ensure credentials are not exposed
            Timeout = 10000
        };

        _logger.LogInformation("SMTP client configured with SSL/TLS encryption enabled (secure channel)");
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            if (!IsValidEmail(to))
            {
                _logger.LogWarning("Invalid email address: {Email}", to);
                return false;
            }

            var fromEmail = _configuration["Email:FromAddress"] ?? "noreply@bankapp.com";
            var fromName = _configuration["Email:FromName"] ?? "Bank Management System";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            await _smtpClient.SendMailAsync(mailMessage);
            
            _logger.LogInformation("Email sent successfully to {Email}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            return false;
        }
    }

    public async Task<bool> SendEmailAsync(List<string> to, string subject, string body, bool isHtml = false)
    {
        try
        {
            var validEmails = to.Where(IsValidEmail).ToList();
            if (!validEmails.Any())
            {
                _logger.LogWarning("No valid email addresses provided");
                return false;
            }

            var fromEmail = _configuration["Email:FromAddress"] ?? "noreply@bankapp.com";
            var fromName = _configuration["Email:FromName"] ?? "Bank Management System";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            foreach (var email in validEmails)
            {
                mailMessage.To.Add(email);
            }

            await _smtpClient.SendMailAsync(mailMessage);
            
            _logger.LogInformation("Email sent successfully to {Count} recipients", validEmails.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to multiple recipients");
            return false;
        }
    }

    public async Task<bool> SendEmailAsync(string to, string templateId, Dictionary<string, string> parameters)
    {
        try
        {
            // For now, use a simple template system
            // In production, you might use a more sophisticated template engine
            var template = GetEmailTemplate(templateId);
            if (string.IsNullOrEmpty(template))
            {
                _logger.LogWarning("Email template not found: {TemplateId}", templateId);
                return false;
            }

            var subject = GetTemplateSubject(templateId);
            var body = ReplaceTemplateParameters(template, parameters);

            return await SendEmailAsync(to, subject, body, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send templated email to {Email} with template {TemplateId}", to, templateId);
            return false;
        }
    }

    public async Task<bool> SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName, bool isHtml = false)
    {
        try
        {
            if (!IsValidEmail(to))
            {
                _logger.LogWarning("Invalid email address: {Email}", to);
                return false;
            }

            var fromEmail = _configuration["Email:FromAddress"] ?? "noreply@bankapp.com";
            var fromName = _configuration["Email:FromName"] ?? "Bank Management System";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            // Add attachment
            if (attachment != null && attachment.Length > 0)
            {
                var attachmentStream = new MemoryStream(attachment);
                var mailAttachment = new Attachment(attachmentStream, attachmentName);
                mailMessage.Attachments.Add(mailAttachment);
            }

            await _smtpClient.SendMailAsync(mailMessage);
            
            _logger.LogInformation("Email with attachment sent successfully to {Email}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachment to {Email}", to);
            return false;
        }
    }

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Use regex for basic email validation
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, RegexTimeout);
            return emailRegex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }

    private string GetEmailTemplate(string templateId)
    {
        // Simple template system - in production, load from database or files
        return templateId switch
        {
            "2fa_token" => "<h2>Your Verification Code</h2><p>Hello {UserName},</p><p>Your verification code is: <strong>{Token}</strong></p><p>This code will expire in {ExpiryMinutes} minutes.</p><p>If you didn't request this code, please contact support immediately.</p>",
            "welcome" => "<h2>Welcome to Bank Management System</h2><p>Hello {UserName},</p><p>Welcome to our banking platform. Your account has been successfully created.</p>",
            _ => string.Empty
        };
    }

    private string GetTemplateSubject(string templateId)
    {
        return templateId switch
        {
            "2fa_token" => "Bank Verification Code",
            "welcome" => "Welcome to Bank Management System",
            _ => "Bank Notification"
        };
    }

    private static string ReplaceTemplateParameters(string template, Dictionary<string, string> parameters)
    {
        var result = template;
        foreach (var parameter in parameters)
        {
            result = result.Replace($"{{{parameter.Key}}}", parameter.Value);
        }
        return result;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _smtpClient?.Dispose();
        }
    }

    ~EmailService()
    {
        Dispose(false);
    }
}