using EZBM.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace EZBM.DesktopClient.Pages;

public class SettingsModel : PageModel
{
    [BindProperty]
    public string StoreName { get; set; } = "EZBM Store";

    [BindProperty]
    public string Currency { get; set; } = "PHP";

    [BindProperty]
    public float LowStockThreshold { get; set; } = 5f;

    [BindProperty]
    public float WrittenReceiptThreshold { get; set; } = 1000f;

    [BindProperty]
    public DateTime StoreOpeningDate { get; set; } = DateTime.UtcNow;

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        var settings = SettingsService.LoadSettings();
        StoreName = settings.StoreName;
        Currency = settings.Currency;
        LowStockThreshold = settings.LowStockThreshold;
        WrittenReceiptThreshold = settings.WrittenReceiptThreshold;
        StoreOpeningDate = settings.StoreOpeningDate;
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Invalid model state. Please correct the fields.";
            return Page();
        }

        try
        {
            var settings = SettingsService.LoadSettings();
            settings.StoreName = StoreName;
            settings.Currency = Currency;
            settings.LowStockThreshold = LowStockThreshold;
            settings.WrittenReceiptThreshold = WrittenReceiptThreshold;
            settings.StoreOpeningDate = StoreOpeningDate;

            SettingsService.SaveSettings(settings);
            SuccessMessage = "Settings saved successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save settings: {ex.Message}";
        }

        return RedirectToPage();
    }
}
