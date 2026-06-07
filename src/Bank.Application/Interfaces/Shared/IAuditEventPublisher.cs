using Bank.Domain.Entities;
using Bank.Domain.Enums;

namespace Bank.Application.Interfaces;

/// <summary>
/// Service interface for publishing audit events using domain events.
/// Provides methods for publishing different types of audit events.
/// </summary>
public interface IAuditEventPublisher
{
    /// <summary>
    /// Publishes an entity created event
    /// </summary>
    Task PublishEntityCreatedAsync(
        Guid? userId,
        string entityType,
        string entityId,
        string newValues,
        string? ipAddress = null,
        string? userAgent = null,
        string? sessionId = null,
        string? requestId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes an entity updated event
    /// </summary>
    Task PublishEntityUpdatedAsync(
        Guid? userId,
        string entityType,
        string entityId,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? sessionId = null,
        string? requestId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes an entity deleted event
    /// </summary>
    Task PublishEntityDeletedAsync(
        Guid? userId,
        string entityType,
        string entityId,
        string? oldValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? sessionId = null,
        string? requestId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a security event
    /// </summary>
    Task PublishSecurityEventAsync(
        Guid? userId,
        string action,
        string entityType,
        string entityId,
        string? ipAddress = null,
        string? userAgent = null,
        string? additionalData = null,
        string? sessionId = null,
        string? requestId = null,
        CancellationToken cancellationToken = default);
}