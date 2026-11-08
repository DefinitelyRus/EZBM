using Backend.Core.Models;
using Backend.DesktopHost.DTOs;
using Backend.DesktopHost.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.DesktopHost.Controllers;

/// <summary>
/// API controller exposing operations to inspect and update business configuration.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SettingsController(IBusinessSettingsService settingsService) : ControllerBase
{
    private readonly IBusinessSettingsService _settingsService = settingsService;

    /// <summary>
    /// Gets active and default business settings.
    /// </summary>
    [HttpGet]
    public ActionResult<GetSettingsResponse> GetSettings()
    {
        BusinessSettings settings = _settingsService.LoadSettings();
        string activePath = settings.GetResolvedDatabasePath();
        string defaultPath = BusinessSettings.GetDefaultDatabasePath();

        GetSettingsResponse response = new(activePath, defaultPath);
        return Ok(response);
    }

    /// <summary>
    /// Updates the configured database file path.
    /// </summary>
    [HttpPut("database-path")]
    public ActionResult UpdateDatabasePath([FromBody] UpdateDatabasePathRequest request)
    {
        if (!_settingsService.ValidateDatabasePath(request.DatabasePath, out string errorMessage))
        {
            return BadRequest(errorMessage);
        }

        BusinessSettings settings = _settingsService.LoadSettings();
        settings.DatabasePath = request.DatabasePath.Trim();
        _settingsService.SaveSettings(settings);

        string message = "Database path updated. Please restart the host to connect to the new database.";
        return Ok(new { Message = message, settings.DatabasePath });
    }

    /// <summary>
    /// Resets the database file path back to default.
    /// </summary>
    [HttpPost("reset-database-path")]
    public ActionResult ResetDatabasePath()
    {
        BusinessSettings settings = _settingsService.LoadSettings();
        settings.DatabasePath = string.Empty;
        _settingsService.SaveSettings(settings);

        string defaultPath = BusinessSettings.GetDefaultDatabasePath();
        string message = "Database path reset to default. Please restart the host to connect to the new database.";
        return Ok(new { Message = message, DatabasePath = defaultPath });
    }
}

