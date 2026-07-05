using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EZBM.DesktopClient.Pages;

/// <summary>
/// Page model for the first-time system onboarding and setup.
/// </summary>
public class SetupModel : PageModel
{
    /// <summary>
    /// Username for the initial admin account.
    /// </summary>
    [BindProperty]
    public string AdminUsername { get; set; } = string.Empty;

    /// <summary>
    /// Password for the initial admin account.
    /// </summary>
    [BindProperty]
    public string AdminPassword { get; set; } = string.Empty;

    /// <summary>
    /// First name of the admin user.
    /// </summary>
    [BindProperty]
    public string AdminFirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the admin user.
    /// </summary>
    [BindProperty]
    public string AdminLastName { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the admin user.
    /// </summary>
    [BindProperty]
    public string AdminEmail { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the store.
    /// </summary>
    [BindProperty]
    public string StoreName { get; set; } = "EZBM Store";

    /// <summary>
    /// Local currency used in the store.
    /// </summary>
    [BindProperty]
    public string Currency { get; set; } = "PHP";

    /// <summary>
    /// Holds validation error messages.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Handles GET requests to check if setup is already done.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        using AppDbContext context = new();
        if (await context.Staff.AnyAsync())
        {
            return RedirectToPage("/Login");
        }
        return Page();
    }

    /// <summary>
    /// Handles POST requests to save onboarding info and create the admin profile.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(AdminUsername) || string.IsNullOrWhiteSpace(AdminPassword))
        {
            ErrorMessage = "Admin Username and Password are required.";
            return Page();
        }

        using AppDbContext context = new();
        if (await context.Staff.AnyAsync())
        {
            ErrorMessage = "Setup has already been completed.";
            return RedirectToPage("/Login");
        }

        try
        {
            // 1. Create Initial Administrator Profile
            Staff admin = new(
                id: Utils.GenerateEntityId(),
                username: AdminUsername,
                payFrequency: Staff.Frequency.Monthly,
                payRate: 0f,
                password: AuthenticationService.HashPassword(AdminPassword),
                firstName: AdminFirstName,
                lastName: AdminLastName,
                email: AdminEmail,
                position: "Administrator"
            );

            context.Staff.Add(admin);
            await context.SaveChangesAsync();

            // 2. Save Initial Business & Store Configuration
            StoreSettings settings = new()
            {
                StoreName = StoreName,
                Currency = Currency,
                StoreOpeningDate = DateTime.UtcNow
            };
            SettingsService.SaveSettings(settings);

            TempData["SuccessMessage"] = "Onboarding completed successfully! Please log in with your credentials.";
            return RedirectToPage("/Login");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error during setup: {ex.Message}";
            return Page();
        }
    }
}
