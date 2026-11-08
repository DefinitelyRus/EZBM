using System.Text.Json;
using Backend.Core.Models;

namespace Backend.DesktopHost.Services;

/// <summary>
/// Service managing local business configuration stored in JSON format.
/// </summary>
public class BusinessSettingsService : IBusinessSettingsService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Gets the path to the settings JSON file in the user documents folder.
    /// </summary>
    public static string GetSettingsFilePath()
    {
        string documentsFolder = Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments
        );
        return Path.Combine(documentsFolder, "EZBM", "settings.json");
    }

    /// <inheritdoc />
    public BusinessSettings LoadSettings()
    {
        string filePath = GetSettingsFilePath();
        if (!File.Exists(filePath))
        {
            return new BusinessSettings();
        }

        try
        {
            string rawJson = File.ReadAllText(filePath);
            BusinessSettings? settings = JsonSerializer.Deserialize<BusinessSettings>(
                rawJson,
                _jsonOptions
            );

            return settings ?? new BusinessSettings();
        }

        catch
        {
            return new BusinessSettings();
        }
    }

    /// <inheritdoc />
    public void SaveSettings(BusinessSettings settings)
    {
        string filePath = GetSettingsFilePath();
        string? directory = Path.GetDirectoryName(filePath);
        if (directory is not null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string rawJson = JsonSerializer.Serialize(settings, _jsonOptions);
        File.WriteAllText(filePath, rawJson);
    }

    /// <inheritdoc />
    public string GetResolvedDatabasePath()
    {
        BusinessSettings settings = LoadSettings();
        string resolvedPath = settings.GetResolvedDatabasePath();

        string? parentDir = Path.GetDirectoryName(resolvedPath);
        if (parentDir is not null && !Directory.Exists(parentDir))
        {
            Directory.CreateDirectory(parentDir);
        }

        return resolvedPath;
    }

    /// <inheritdoc />
    public bool ValidateDatabasePath(string path, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            errorMessage = "Database path cannot be empty.";
            return false;
        }

        string extension = Path.GetExtension(path);
        if (!extension.Equals(".db", StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "Database file must have a .db extension.";
            return false;
        }

        try
        {
            string fullPath = Path.GetFullPath(path);
            string? dir = Path.GetDirectoryName(fullPath);
            if (dir is not null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            errorMessage = string.Empty;
            return true;
        }

        catch (Exception ex)
        {
            errorMessage = $"Invalid path format: {ex.Message}";
            return false;
        }
    }
}

