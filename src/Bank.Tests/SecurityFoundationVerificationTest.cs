using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Bank.Tests;

/// <summary>
/// Simple verification test for security foundation components without database dependencies
/// </summary>
public class SecurityFoundationVerificationTest
{
    [Fact]
    public void AuditLog_CanCreateAllEventTypes()
    {
        // Arrange & Act
        var userAction = AuditLog.CreateUserAction(
            Guid.NewGuid(),
            "LOGIN",
            "USER",
            "test-user-id");

        var securityEvent = AuditLog.CreateSecurityEvent(
            Guid.NewGuid(),
            "FAILED_LOGIN",
            "SECURITY",
            "login-attempt");

        var systemEvent = AuditLog.CreateSystemEvent(
            "SYSTEM_STARTUP",
            "SYSTEM",
            "application");

        // Assert
        Assert.Equal(AuditEventType.UserAction, userAction.EventType);
        Assert.Equal(AuditEventType.SecurityEvent, securityEvent.EventType);
        Assert.Equal(AuditEventType.SystemEvent, systemEvent.EventType);
        
        Assert.True(userAction.CreatedAt <= DateTime.UtcNow);
        Assert.True(securityEvent.CreatedAt <= DateTime.UtcNow);
        Assert.True(systemEvent.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void PasswordPolicy_CanCreateAllPolicyTypes()
    {
        // Arrange & Act
        var basicPolicy = PasswordPolicy.CreateBasicPolicy();
        var standardPolicy = PasswordPolicy.CreateStandardPolicy();
        var strongPolicy = PasswordPolicy.CreateStrongPolicy();
        var enterprisePolicy = PasswordPolicy.CreateEnterprisePolicy();

        // Assert
        Assert.Equal("Basic", basicPolicy.Name);
        Assert.Equal("Standard", standardPolicy.Name);
        Assert.Equal("Strong", strongPolicy.Name);
        Assert.Equal("Enterprise", enterprisePolicy.Name);

        Assert.Equal(PasswordComplexityLevel.Basic, basicPolicy.ComplexityLevel);
        Assert.Equal(PasswordComplexityLevel.Standard, standardPolicy.ComplexityLevel);
        Assert.Equal(PasswordComplexityLevel.Strong, strongPolicy.ComplexityLevel);
        Assert.Equal(PasswordComplexityLevel.Enterprise, enterprisePolicy.ComplexityLevel);

        // Verify TimeSpan values are reasonable (< 24 hours)
        Assert.True(basicPolicy.MaxPasswordAge < TimeSpan.FromDays(1));
        Assert.True(standardPolicy.MaxPasswordAge < TimeSpan.FromDays(1));
        Assert.True(strongPolicy.MaxPasswordAge < TimeSpan.FromDays(1));
        Assert.True(enterprisePolicy.MaxPasswordAge < TimeSpan.FromDays(1));
    }

    [Fact]
    public void SecurityFoundation_CoreEntitiesAreValid()
    {
        // Test that core security entities can be created without exceptions
        
        // Test TwoFactorToken
        var token = new TwoFactorToken
        {
            UserId = Guid.NewGuid(),
            Token = "123456",
            Method = TwoFactorMethod.SMS,
            Destination = "+1234567890",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };
        
        Assert.NotNull(token);
        Assert.Equal("123456", token.Token);
        Assert.Equal(TwoFactorMethod.SMS, token.Method);
        Assert.True(token.ExpiresAt > DateTime.UtcNow);

        // Test Session
        var session = new Session(
            Guid.NewGuid(),
            "session-token",
            "refresh-token",
            TimeSpan.FromMinutes(30),
            TimeSpan.FromDays(7),
            "192.168.1.1",
            "Test Browser");
        
        Assert.NotNull(session);
        Assert.Equal("session-token", session.SessionToken);
        Assert.Equal(SessionStatus.Active, session.Status);
        Assert.True(session.ExpiresAt > DateTime.UtcNow);
    }
}