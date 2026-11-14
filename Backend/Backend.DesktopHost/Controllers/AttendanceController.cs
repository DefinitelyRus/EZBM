using Backend.Core.Common;
using Backend.DesktopHost.DTOs;
using Backend.DesktopHost.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.DesktopHost.Controllers;

/// <summary>
/// Controller for logging and querying employee attendance records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AttendanceController(
    AttendanceService attendanceService
) : ControllerBase
{
    private readonly AttendanceService _attendanceService = attendanceService;

    #region Attendance Endpoints

    /// <summary>
    /// Records an employee clock-in or clock-out event.
    /// </summary>
    [HttpPost("clock")]
    public async Task<ActionResult<AttendanceResponse>> Clock(
        [FromBody] ClockActionRequest request
    )
    {
        try
        {
            AttendanceResponse response =
                await _attendanceService.ClockActionAsync(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        catch (ArgumentException ex)
        {
            return BadRequest(Esc.ExportMessage(ex));
        }

        catch (Exception ex)
        {
            return Problem(Esc.ExportMessage(ex));
        }
    }

    /// <summary>
    /// Retrieves historical attendance logs.
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<List<AttendanceResponse>>> GetHistory(
        [FromQuery] long? userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate
    )
    {
        try
        {
            List<AttendanceResponse> response =
                await _attendanceService.GetAttendanceHistoryAsync(
                    userId,
                    startDate,
                    endDate
                );
            return Ok(response);
        }

        catch (Exception ex)
        {
            return Problem(Esc.ExportMessage(ex));
        }
    }

    #endregion
}
