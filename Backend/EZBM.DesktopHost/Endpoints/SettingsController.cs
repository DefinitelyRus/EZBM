using EZBM.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints to read and modify storefront configurations.
/// </summary>
public static class SettingsController
{
    /// <summary>
    /// Retrieves the current storefront configuration.
    /// </summary>
    public static IResult GetSettings()
    {
        return Results.Ok(SettingsService.LoadSettings());
    }

    /// <summary>
    /// Updates the storefront configurations.
    /// </summary>
    public static IResult SaveSettings([FromBody] StoreSettings settings)
    {
        if (settings == null)
        {
            return Results.BadRequest("Settings payload cannot be null.");
        }
        SettingsService.SaveSettings(settings);
        return Results.Ok(new { success = true });
    }
}
