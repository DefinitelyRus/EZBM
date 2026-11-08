namespace Backend.DesktopHost.DTOs;

/// <summary>
/// Response payload containing business settings information.
/// </summary>
public record GetSettingsResponse(
    string ActiveDatabasePath,
    string DefaultDatabasePath
);

