using Backend.Core.Common;
using Backend.DesktopHost.DTOs;
using Backend.DesktopHost.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.DesktopHost.Controllers;

/// <summary>
/// Controller for processing and recording customer payments.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentsController(PaymentService paymentService) : ControllerBase
{
    private readonly PaymentService _paymentService = paymentService;

    #region Payment Endpoints

    /// <summary>
    /// Processes and saves a payment transaction.
    /// </summary>
    [HttpPost("process")]
    public async Task<ActionResult<ProcessPaymentResponse>> ProcessPayment(
        [FromBody] ProcessPaymentRequest request
    )
    {
        try
        {
            ProcessPaymentResponse response =
                await _paymentService.ProcessPaymentAsync(request);
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
