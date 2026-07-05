using System.IO;
using System.Threading.Tasks;
using EZBM.Core.Services;
using Microsoft.AspNetCore.Http;
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
        if (settings is null)
        {
            return Results.BadRequest("Settings payload cannot be null.");
        }
        SettingsService.SaveSettings(settings);
        return Results.Ok(new { success = true });
    }

    /// <summary>
    /// Retrieves configurations for a specific user.
    /// </summary>
    public static IResult GetUserSettings(ulong id)
    {
        string json = SettingsService.LoadUserSettings(id);
        return Results.Content(json, "application/json");
    }

    /// <summary>
    /// Saves configurations for a specific user.
    /// </summary>
    public static async Task<IResult> SaveUserSettings(ulong id, HttpRequest request)
    {
        using var reader = new StreamReader(request.Body);
        string json = await reader.ReadToEndAsync();
        SettingsService.SaveUserSettings(id, json);
        return Results.Ok(new { success = true });
    }
}
