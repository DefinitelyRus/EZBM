using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.DesktopClient.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace EZBM.DesktopClient.Pages;

public class SettingsModel : PageModel
{
    #region Nested Classes
    public class UserPreferences
    {
        public string Theme { get; set; } = "System";
        public string DateFormat { get; set; } = "yyyy-MM-dd";
        public bool SoundAlertsEnabled { get; set; } = true;
        public string BarcodeScannerDevice { get; set; } = "Default Keyboard HID";
        public string RfidScannerDevice { get; set; } = "Default Keyboard HID";
        public string CashRegisterTriggerDevice { get; set; } = "Default USB Relay Trigger";
    }
    #endregion

    #region Admin Settings Properties
    [BindProperty]
    public string StoreName { get; set; } = "EZBM Store";

    [BindProperty]
    public string Currency { get; set; } = "PHP";

    [BindProperty]
    public float LowStockThreshold { get; set; } = 20f; // in percent warning

    [BindProperty]
    public float WrittenReceiptThreshold { get; set; } = 1000f;

    [BindProperty]
    public DateTime StoreOpeningDate { get; set; } = DateTime.UtcNow;

    [BindProperty]
    public int CardExpiryStaff { get; set; } = 365;

    [BindProperty]
    public int CardExpiryOneTime { get; set; } = 1;

    [BindProperty]
    public int CardExpiryMember { get; set; } = 30;

    [BindProperty]
    public string BackupTargetPath { get; set; } = "";

    [BindProperty]
    public string BackupAutosaveInterval { get; set; } = "Daily";

    [BindProperty]
    public int BackupRetentionCount { get; set; } = 10;

    [BindProperty]
    public bool BackupRotationEnabled { get; set; } = true;

    [BindProperty]
    public bool EnableRfidLogin { get; set; } = false;
    #endregion

    #region User Settings Properties
    [BindProperty]
    public string UserTheme { get; set; } = "System";

    [BindProperty]
    public string DateFormat { get; set; } = "yyyy-MM-dd";

    [BindProperty]
    public bool SoundAlerts { get; set; } = true;

    [BindProperty]
    public string BarcodeDevice { get; set; } = "Default Keyboard HID";

    [BindProperty]
    public string RfidDevice { get; set; } = "Default Keyboard HID";

    [BindProperty]
    public string CashRegisterDevice { get; set; } = "Default USB Relay Trigger";

    [BindProperty]
    public string? CurrentPassword { get; set; }

    [BindProperty]
    public string? NewPassword { get; set; }

    [BindProperty]
    public string? ConfirmPassword { get; set; }
    #endregion

    #region Backup Restore Properties
    [BindProperty]
    public string? BackupFileToRestore { get; set; }

    public List<string> AvailableBackups { get; set; } = new();
    #endregion

    #region General Page Properties
    public Staff? ActiveStaff { get; set; }
    public bool IsAdmin { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }
    
    public Dictionary<string, StoreSettings.PromoCodeInfo> PromoCodesList { get; set; } = new();
    #endregion

    #region Handlers

    /// <summary>
    /// Restricts page access to authenticated staff members.
    /// </summary>
    public override async Task OnPageHandlerExecutionAsync(
        Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context,
        Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutionDelegate next
    )
    {
        Staff? activeStaff = await StateHelper.GetActiveStaffAsync(HttpContext);

        if (activeStaff is null)
        {
            context.Result = RedirectToPage("/Login");
            return;
        }

        await next();
    }

    public async Task OnGetAsync()
    {
        ActiveStaff = await StateHelper.GetActiveStaffAsync(HttpContext);
        IsAdmin = ActiveStaff != null && (
            ActiveStaff.Position?.Contains("Manager", StringComparison.OrdinalIgnoreCase) == true ||
            ActiveStaff.Position?.Contains("CEO", StringComparison.OrdinalIgnoreCase) == true ||
            ActiveStaff.Position?.Contains("Admin", StringComparison.OrdinalIgnoreCase) == true ||
            ActiveStaff.Position?.Contains("Specialist", StringComparison.OrdinalIgnoreCase) == true
        );

        // Load Business Settings
        StoreSettings settings = SettingsService.LoadSettings();
        StoreName = settings.StoreName;
        Currency = settings.Currency;
        LowStockThreshold = settings.LowStockThreshold * 100f; // display as percentage
        WrittenReceiptThreshold = settings.WrittenReceiptThreshold;
        StoreOpeningDate = settings.StoreOpeningDate;

        CardExpiryStaff = settings.CardExpirationOffsets.GetValueOrDefault("Staff", 365);
        CardExpiryOneTime = settings.CardExpirationOffsets.GetValueOrDefault("OneTime", 1);
        CardExpiryMember = settings.CardExpirationOffsets.GetValueOrDefault("Member", 30);
        BackupTargetPath = settings.BackupTargetPath;
        BackupAutosaveInterval = settings.BackupAutosaveInterval;
        BackupRetentionCount = settings.BackupRetentionCount;
        BackupRotationEnabled = settings.BackupRotationEnabled;
        EnableRfidLogin = settings.EnableRfidLogin;
        PromoCodesList = settings.PromoCodes ?? new();

        // Load available backup files
        if (Directory.Exists(BackupTargetPath))
        {
            try
            {
                AvailableBackups = new List<string>(Directory.GetFiles(BackupTargetPath, "*.db"));
            }
            catch
            {
                // ignore
            }
        }

        // Load User Preferences
        if (ActiveStaff != null)
        {
            string userSettingsJson = SettingsService.LoadUserSettings(ActiveStaff.Id);
            try
            {
                var userPrefs = JsonSerializer.Deserialize<UserPreferences>(userSettingsJson) ?? new UserPreferences();
                UserTheme = userPrefs.Theme;
                DateFormat = userPrefs.DateFormat;
                SoundAlerts = userPrefs.SoundAlertsEnabled;
                BarcodeDevice = userPrefs.BarcodeScannerDevice;
                RfidDevice = userPrefs.RfidScannerDevice ?? "Default Keyboard HID";
                CashRegisterDevice = userPrefs.CashRegisterTriggerDevice ?? "Default USB Relay Trigger";
            }
            catch
            {
                // defaults
            }
        }
    }

    public async Task<IActionResult> OnPostSaveUserPreferencesAsync()
    {
        ActiveStaff = await StateHelper.GetActiveStaffAsync(HttpContext);
        if (ActiveStaff == null)
        {
            ErrorMessage = "Must be logged in to save preferences.";
            return RedirectToPage();
        }

        var prefs = new UserPreferences
        {
            Theme = UserTheme,
            DateFormat = DateFormat,
            SoundAlertsEnabled = SoundAlerts,
            BarcodeScannerDevice = BarcodeDevice,
            RfidScannerDevice = RfidDevice,
            CashRegisterTriggerDevice = CashRegisterDevice
        };

        string json = JsonSerializer.Serialize(prefs);
        SettingsService.SaveUserSettings(ActiveStaff.Id, json);
        SuccessMessage = "User preferences saved successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        ActiveStaff = await StateHelper.GetActiveStaffAsync(HttpContext);
        if (ActiveStaff == null)
        {
            ErrorMessage = "Must be logged in to change password.";
            return RedirectToPage();
        }

        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match or are invalid.";
            return RedirectToPage();
        }

        using AppDbContext context = new();
        var staff = await context.Staff.FindAsync(ActiveStaff.Id);
        if (staff == null)
        {
            ErrorMessage = "Staff profile not found.";
            return RedirectToPage();
        }

        if (!AuthenticationService.VerifyPassword(CurrentPassword, staff.Password ?? ""))
        {
            ErrorMessage = "Incorrect current password.";
            return RedirectToPage();
        }

        staff.Password = AuthenticationService.HashPassword(NewPassword);
        await context.SaveChangesAsync();

        SuccessMessage = "Password updated successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveBusinessSettingsAsync()
    {
        ActiveStaff = await StateHelper.GetActiveStaffAsync(HttpContext);
        IsAdmin = ActiveStaff != null && (
            ActiveStaff.Position?.Contains("Manager", StringComparison.OrdinalIgnoreCase) == true ||
            ActiveStaff.Position?.Contains("CEO", StringComparison.OrdinalIgnoreCase) == true ||
            ActiveStaff.Position?.Contains("Admin", StringComparison.OrdinalIgnoreCase) == true ||
            ActiveStaff.Position?.Contains("Specialist", StringComparison.OrdinalIgnoreCase) == true
        );

        if (!IsAdmin)
        {
            ErrorMessage = "Access Denied: You do not have permission to manage store settings.";
            return RedirectToPage();
        }

        try
        {
            StoreSettings settings = SettingsService.LoadSettings();
            settings.StoreName = StoreName;
            settings.Currency = Currency;
            settings.LowStockThreshold = LowStockThreshold / 100f; // save as decimal fraction
            settings.WrittenReceiptThreshold = WrittenReceiptThreshold;
            settings.StoreOpeningDate = StoreOpeningDate;
            settings.EnableRfidLogin = EnableRfidLogin;

            settings.CardExpirationOffsets["Staff"] = CardExpiryStaff;
            settings.CardExpirationOffsets["OneTime"] = CardExpiryOneTime;
            settings.CardExpirationOffsets["Member"] = CardExpiryMember;
            settings.BackupTargetPath = BackupTargetPath;
            settings.BackupAutosaveInterval = BackupAutosaveInterval;
            settings.BackupRetentionCount = BackupRetentionCount;
            settings.BackupRotationEnabled = BackupRotationEnabled;

            var promoCodes = new Dictionary<string, StoreSettings.PromoCodeInfo>();
            var codes = Request.Form["promo_code[]"];
            var discounts = Request.Form["promo_discount[]"];
            var expiries = Request.Form["promo_expiry[]"];
            for (int i = 0; i < codes.Count; i++)
            {
                string code = codes[i].ToString().Trim().ToUpper();
                if (!string.IsNullOrEmpty(code) && float.TryParse(discounts[i], out float discount) && DateTime.TryParse(expiries[i], out DateTime expiry))
                {
                    promoCodes[code] = new StoreSettings.PromoCodeInfo { DiscountPercentage = discount, ExpirationDate = expiry };
                }
            }
            settings.PromoCodes = promoCodes;

            SettingsService.SaveSettings(settings);
            SuccessMessage = "Business settings saved successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save configurations: {ex.Message}";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTriggerBackupAsync()
    {
        try
        {
            StoreSettings settings = SettingsService.LoadSettings();
            if (!Directory.Exists(settings.BackupTargetPath))
            {
                Directory.CreateDirectory(settings.BackupTargetPath);
            }
            string destFile = Path.Combine(settings.BackupTargetPath, $"ezbm_backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.db");
            if (System.IO.File.Exists(DbManager.DbFilePath))
            {
                System.IO.File.Copy(DbManager.DbFilePath, destFile, true);
                SuccessMessage = $"Database backup created successfully at: {destFile}";
            }
            else
            {
                ErrorMessage = "Could not find active database file to backup.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Backup failed: {ex.Message}";
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTriggerRestoreAsync()
    {
        if (string.IsNullOrEmpty(BackupFileToRestore))
        {
            ErrorMessage = "Please select a backup file to restore.";
            return RedirectToPage();
        }

        try
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (System.IO.File.Exists(BackupFileToRestore))
            {
                System.IO.File.Copy(BackupFileToRestore, DbManager.DbFilePath, true);
                SuccessMessage = "Database successfully restored from backup! Refreshing session.";
            }
            else
            {
                ErrorMessage = $"Backup file not found at: {BackupFileToRestore}";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Restore failed: {ex.Message}";
        }
        return RedirectToPage();
    }
    #endregion
}
