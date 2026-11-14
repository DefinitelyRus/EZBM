namespace Backend.Core.Models;

/// <summary>
/// Record representing an audit or security system event.
/// </summary>
public class AuditEventLog : Entity
{
    /// <summary>
    /// Category or type of event (e.g. AuthFailed, StockAdjusted).
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Informational message describing the event.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Event severity level (e.g. Info, Warning, Error).
    /// </summary>
    public string Severity { get; set; } = "Info";

    /// <summary>
    /// Timestamp when the event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Source subsystem or component generating the event.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Optional user identifier associated with the event.
    /// </summary>
    public long? UserId { get; set; }
}
