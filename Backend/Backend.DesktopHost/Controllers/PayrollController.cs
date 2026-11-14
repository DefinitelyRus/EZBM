using Backend.Core.Common;
using Backend.DesktopHost.DTOs;
using Backend.DesktopHost.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.DesktopHost.Controllers;

/// <summary>
/// Controller for calculating and reviewing employee payroll summaries.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PayrollController(PayrollService payrollService) : ControllerBase
{
    private readonly PayrollService _payrollService = payrollService;

    #region Payroll Endpoints

    /// <summary>
    /// Calculates hours worked and gross pay for an employee.
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<PayrollSummaryResponse>> GetSummary(
        [FromQuery] long userId,
        [FromQuery] decimal hourlyRate,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate
    )
    {
        try
        {
            PayrollSummaryResponse response =
                await _payrollService.CalculatePayrollAsync(
                    userId,
                    hourlyRate,
                    startDate,
                    endDate
                );
            return Ok(response);
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

    #endregion
}
