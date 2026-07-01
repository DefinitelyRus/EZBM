using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EZBM.Core.Services;

/// <summary>
/// Holds configuration variables for the storefront and hardware policies.
/// </summary>
public class StoreSettings
{
    /// <summary>
    /// The display name of the storefront.
    /// </summary>
    public string StoreName { get; set; } = "EZBM Store";

    /// <summary>
    /// The default currency code used for formatting.
    /// </summary>
    public string Currency { get; set; } = "PHP";

    /// <summary>
    /// The threshold below which item stock quantity flags a warning.
    /// </summary>
    public float LowStockThreshold { get; set; } = 5f;

    /// <summary>
    /// The amount above which checkout requires a physical written receipt.
    /// </summary>
    public float WrittenReceiptThreshold { get; set; } = 1000f;

    /// <summary>
    /// The official grand opening date of the storefront.
    /// </summary>
    public DateTime StoreOpeningDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The default validity period in days for access card profiles.
    /// </summary>
    public Dictionary<string, int> CardExpirationOffsets { get; set; } = new()
    {
        { "Staff", 365 },
        { "OneTime", 1 },
        { "Member", 30 }
    };

    /// <summary>
    /// The standard commission rates earned for membership registration or upgrades.
    /// </summary>
    public Dictionary<string, float> MembershipCommissions { get; set; } = new()
    {
        { "Silver Upgrade", 10f },
        { "Gold Upgrade", 25f },
        { "Platinum Upgrade", 50f }
    };
}

/// <summary>
/// Provides access to read/write storefront configurations from settings.json.
/// </summary>
public static class SettingsService
{
    private static string GetSettingsFilePath()
    {
        string? currentDir = AppDomain.CurrentDomain.BaseDirectory;
        while (currentDir is not null)
        {
            if (File.Exists(Path.Combine(currentDir, "EZBM.slnx")))
            {
                return Path.Combine(currentDir, "settings.json");
            }
            currentDir = Directory.GetParent(currentDir)?.FullName;
        }
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
    }

    /// <summary>
    /// Loads the active storefront configuration parameters.
    /// </summary>
    public static StoreSettings LoadSettings()
    {
        try
        {
            string path = GetSettingsFilePath();
            if (!File.Exists(path))
            {
                var defaults = new StoreSettings();
                SaveSettings(defaults);
                return defaults;
            }
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<StoreSettings>(json) ?? new StoreSettings();
        }
        catch
        {
            return new StoreSettings();
        }
    }

    /// <summary>
    /// Persists new storefront configuration parameters.
    /// </summary>
    public static void SaveSettings(StoreSettings settings)
    {
        try
        {
            string path = GetSettingsFilePath();
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Tools.Log.Err($"Failed to save settings: {ex.Message}");
        }
    }
}
