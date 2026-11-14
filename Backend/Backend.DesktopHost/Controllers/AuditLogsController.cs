using Backend.DesktopHost.DTOs;
using Backend.DesktopHost.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.DesktopHost.Controllers;

/// <summary>
/// Controller for querying and recording system audit logs.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuditLogsController(
    AuditLogService auditLogService
) : ControllerBase
{
    private readonly AuditLogService _auditLogService = auditLogService;

    #region Audit Log Endpoints

    /// <summary>
    /// Retrieves system audit events filtered by criteria.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AuditEventLogDto>>> GetLogs(
        [FromQuery] string? eventType,
        [FromQuery] string? severity,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        try
        {
            List<AuditEventLogDto> logs =
                await _auditLogService.GetLogsAsync(
                    eventType,
                    severity,
                    from,
                    to
                );
            return Ok(logs);
        }

        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Records a system audit log entry.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AuditEventLogDto>> CreateLog(
        [FromBody] CreateAuditLogRequest request
    )
    {
        try
        {
            AuditEventLogDto log = await _auditLogService.LogEventAsync(
                request.EventType,
                request.Message,
                request.Severity,
                request.Source,
                request.UserId
            );
            return StatusCode(StatusCodes.Status201Created, log);
        }

        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    #endregion
}
