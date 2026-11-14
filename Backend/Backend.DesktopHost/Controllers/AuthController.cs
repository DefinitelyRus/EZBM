using Backend.Core.Common;
using Backend.DesktopHost.DTOs;
using Backend.DesktopHost.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.DesktopHost.Controllers;

/// <summary>
/// Controller for authentication and session management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    #region Authentication Endpoints

    /// <summary>
    /// Authenticates user and returns access token.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request
    )
    {
        try
        {
            LoginResponse response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(Esc.ExportMessage(ex));
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
    /// Verifies session token validity.
    /// </summary>
    [HttpGet("verify")]
    public ActionResult Verify([FromQuery] string token)
    {
        bool isValid = _authService.VerifySession(token);
        if (!isValid) return Unauthorized("Session is invalid or expired.");

        return Ok(new { valid = true });
    }

    /// <summary>
    /// Updates user password credentials.
    /// </summary>
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request
    )
    {
        try
        {
            await _authService.ChangePasswordAsync(request);
            return Ok("Password updated successfully.");
        }

        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(Esc.ExportMessage(ex));
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
