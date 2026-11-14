using Backend.Core.Common;
using Backend.DesktopHost.DTOs;
using Backend.DesktopHost.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.DesktopHost.Controllers;

/// <summary>
/// Controller for processing point-of-sale customer checkout and sales orders.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PosController(PosService posService) : ControllerBase
{
    private readonly PosService _posService = posService;

    #region POS Endpoints

    /// <summary>
    /// Processes a POS cart checkout and registers sale records.
    /// </summary>
    [HttpPost("checkout")]
    public async Task<ActionResult<CheckoutResponse>> Checkout(
        [FromBody] CheckoutRequest request
    )
    {
        try
        {
            CheckoutResponse response =
                await _posService.CheckoutAsync(request);
            return Ok(response);
        }

        catch (ArgumentException ex)
        {
            return BadRequest(Esc.ExportMessage(ex));
        }

        catch (InvalidOperationException ex)
        {
            return Conflict(Esc.ExportMessage(ex));
        }

        catch (Exception ex)
        {
            return Problem(Esc.ExportMessage(ex));
        }
    }

    #endregion
}
