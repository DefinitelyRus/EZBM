namespace Backend.DesktopHost.DTOs;

/// <summary>
/// Data transfer object for system audit event logs.
/// </summary>
public record AuditEventLogDto(
    long Id,
    string EventType,
    string Message,
    string Severity,
    DateTime Timestamp,
    string? Source,
    long? UserId
);

/// <summary>
/// Request record for logging a new audit event.
/// </summary>
public record CreateAuditLogRequest(
    string EventType,
    string Message,
    string Severity,
    string? Source,
    long? UserId
);
