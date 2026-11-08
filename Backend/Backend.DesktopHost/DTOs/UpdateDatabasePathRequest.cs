namespace Backend.DesktopHost.DTOs;

/// <summary>
/// Request payload to update the configured database path.
/// </summary>
public record UpdateDatabasePathRequest(
    string DatabasePath
);

