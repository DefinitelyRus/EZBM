using Backend.Core.Models;

namespace Backend.DesktopHost.Services;

/// <summary>
/// Service interface for reading and writing business-wide settings.
/// </summary>
public interface IBusinessSettingsService
{
    /// <summary>
    /// Loads settings from disk or returns default configuration.
    /// </summary>
    BusinessSettings LoadSettings();

    /// <summary>
    /// Saves updated business settings to disk.
    /// </summary>
    void SaveSettings(BusinessSettings settings);

    /// <summary>
    /// Resolves the active database file path.
    /// </summary>
    string GetResolvedDatabasePath();

    /// <summary>
    /// Validates whether a proposed database path is valid and accessible.
    /// </summary>
    bool ValidateDatabasePath(string path, out string errorMessage);
}

