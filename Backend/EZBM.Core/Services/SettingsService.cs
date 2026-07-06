using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EZBM.Core.Tools;

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
    /// The default global low-stock threshold percentage (e.g. 0.20 for 20%).
    /// </summary>
    public float LowStockThreshold { get; set; } = 0.20f;

    /// <summary>
    /// The amount above which checkout requires a physical written receipt.
    /// </summary>
    public float WrittenReceiptThreshold { get; set; } = 1000f;

    /// <summary>
    /// The official grand opening date of the storefront.
    /// </summary>
    public DateTime StoreOpeningDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The destination path where SQLite database backups will be saved.
    /// </summary>
    public string BackupTargetPath { get; set; } = Path.Combine(Utils.UserSavePath, "ezbm-backups");

    /// <summary>
    /// The backup autosave interval (e.g. Daily, Weekly, None).
    /// </summary>
    public string BackupAutosaveInterval { get; set; } = "Daily";

    /// <summary>
    /// The number of recent backups to keep.
    /// </summary>
    public int BackupRetentionCount { get; set; } = 10;

    /// <summary>
    /// Whether historical grandfather-father-son backup rotation is enabled.
    /// </summary>
    public bool BackupRotationEnabled { get; set; } = true;

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
        { "Platinum Upgrade", 50f },
        { "Dog Grooming - Small", 20f },
        { "Dog Grooming - Medium", 30f },
        { "Dog Grooming - Large", 40f },
        { "Cat Grooming", 25f }
    };

    /// <summary>
    /// Represents info about a promotional discount code.
    /// </summary>
    public class PromoCodeInfo
    {
        public float DiscountPercentage { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    /// <summary>
    /// Custom promotion discount codes and their settings.
    /// </summary>
    public Dictionary<string, PromoCodeInfo> PromoCodes { get; set; } = new()
    {
        { "FREEWEEK", new PromoCodeInfo { DiscountPercentage = 100f, ExpirationDate = new DateTime(2026, 12, 31) } },
        { "EZBM10", new PromoCodeInfo { DiscountPercentage = 10f, ExpirationDate = new DateTime(2026, 12, 31) } }
    };
}


/// <summary>
/// Provides access to read/write storefront configurations from settings.json.
/// </summary>
public static class SettingsService
{
    private static string GetAdminSettingsFilePath()
    {
        string dir = Path.Combine(Utils.UserSavePath, "ezbm");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        return Path.Combine(dir, "admin-settings");
    }

    /// <summary>
    /// Resolves the storage path for individual user configurations.
    /// </summary>
    public static string GetUserSettingsFilePath(ulong userId)
    {
        string dir = Path.Combine(Utils.UserSavePath, "ezbm", "user-settings");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        return Path.Combine(dir, $"{userId}-settings.json");
    }

    /// <summary>
    /// Loads the active storefront configuration parameters.
    /// </summary>
    public static StoreSettings LoadSettings()
    {
        try
        {
            string path = GetAdminSettingsFilePath();
            if (!File.Exists(path))
            {
                StoreSettings defaults = new();
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
            string path = GetAdminSettingsFilePath();
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        catch (Exception ex)
        {
            Tools.Log.Err($"Failed to save settings: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads individual user preference JSON string.
    /// </summary>
    public static string LoadUserSettings(ulong userId)
    {
        try
        {
            string path = GetUserSettingsFilePath(userId);
            if (!File.Exists(path)) return "{}";
            return File.ReadAllText(path);
        }
        catch
        {
            return "{}";
        }
    }

    /// <summary>
    /// Saves individual user preference JSON string.
    /// </summary>
    public static void SaveUserSettings(ulong userId, string jsonContent)
    {
        try
        {
            string path = GetUserSettingsFilePath(userId);
            File.WriteAllText(path, jsonContent);
        }
        catch (Exception ex)
        {
            Tools.Log.Err($"Failed to save user settings: {ex.Message}");
        }
    }
}
