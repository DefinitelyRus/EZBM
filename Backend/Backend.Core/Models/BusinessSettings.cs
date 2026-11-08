namespace Backend.Core.Models;

/// <summary>
/// Configuration settings for the business application.
/// </summary>
public class BusinessSettings
{
    /// <summary>
    /// Gets or sets the custom SQLite database file path.
    /// </summary>
    public string DatabasePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets the default database file path in the user documents directory.
    /// </summary>
    public static string GetDefaultDatabasePath()
    {
        string documentsFolder = Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments
        );
        string businessDataFolder = Path.Combine(
            documentsFolder,
            "EZBM",
            "business_data"
        );
        return Path.Combine(businessDataFolder, "data.db");
    }

    /// <summary>
    /// Resolves the effective database path, falling back to the default path.
    /// </summary>
    public string GetResolvedDatabasePath()
    {
        if (string.IsNullOrWhiteSpace(DatabasePath))
        {
            return GetDefaultDatabasePath();
        }

        return DatabasePath;
    }
}

