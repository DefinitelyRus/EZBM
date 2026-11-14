using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.DesktopHost.Services;

/// <summary>
/// Service managing system audit logging and historical log retrieval.
/// </summary>
public class AuditLogService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    #region Audit Methods

    /// <summary>
    /// Records an audit log event asynchronously.
    /// </summary>
    public async Task<AuditEventLogDto> LogEventAsync(
        string eventType,
        string message,
        string severity = "Info",
        string? source = null,
        long? userId = null
    )
    {
        AuditEventLog log = new()
        {
            EventType = eventType,
            Message = message,
            Severity = severity,
            Timestamp = DateTime.UtcNow,
            Source = source,
            UserId = userId
        };

        await _context.AuditEventLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        AuditEventLogDto dto = new(
            log.Id,
            log.EventType,
            log.Message,
            log.Severity,
            log.Timestamp,
            log.Source,
            log.UserId
        );

        return dto;
    }

    /// <summary>
    /// Retrieves historical audit logs with filtering support.
    /// </summary>
    public async Task<List<AuditEventLogDto>> GetLogsAsync(
        string? eventType,
        string? severity,
        DateTime? from,
        DateTime? to
    )
    {
        IQueryable<AuditEventLog> query = _context.AuditEventLogs
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(eventType))
        {
            query = query.Where(l => l.EventType == eventType);
        }

        if (!string.IsNullOrWhiteSpace(severity))
        {
            query = query.Where(l => l.Severity == severity);
        }

        if (from.HasValue)
        {
            query = query.Where(l => l.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(l => l.Timestamp <= to.Value);
        }

        List<AuditEventLogDto> results = await query
            .OrderByDescending(l => l.Timestamp)
            .Select(l => new AuditEventLogDto(
                l.Id,
                l.EventType,
                l.Message,
                l.Severity,
                l.Timestamp,
                l.Source,
                l.UserId
            ))
            .ToListAsync();

        return results;
    }

    #endregion
}
