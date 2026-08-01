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
    #region Properties

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

    #endregion

    #region Handlers

    /// <summary>
    /// Handles GET requests to check if setup is already done.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        using AppDbContext context = new();
        bool hasStaff = await context.Staff.AnyAsync();

        if (hasStaff)
        {
            return RedirectToPage("/Login");
        }

        return Page();
    }

    /// <summary>
    /// Handles POST requests to save onboarding info and create admin profile.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        bool hasUsername = !string.IsNullOrWhiteSpace(AdminUsername);
        bool hasPassword = !string.IsNullOrWhiteSpace(AdminPassword);

        if (!hasUsername || !hasPassword)
        {
            ErrorMessage = "Admin Username and Password are required.";
            return Page();
        }

        using AppDbContext context = new();
        bool alreadySetup = await context.Staff.AnyAsync();

        if (alreadySetup)
        {
            ErrorMessage = "Setup has already been completed.";
            return RedirectToPage("/Login");
        }

        try
        {
            Role? adminRole = await context.Role
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole is null)
            {
                string adminPermsJson = "{\"Checkout\":1,\"ApplyDiscounts\":1," +
                    "\"Refunds\":1,\"ViewInventory\":1,\"ModifyInventory\":1," +
                    "\"ViewSensitiveInventoryCost\":1,\"ViewStaffInfo\":1," +
                    "\"ManageStaff\":1,\"ManageAccess\":1,\"ManagePayroll\":1," +
                    "\"ViewLogs\":1,\"DeleteLogs\":1,\"ManageSettings\":1}";
                adminRole = new Role(
                    id: Utils.GenerateEntityId(),
                    name: "Admin",
                    permissionsJson: adminPermsJson
                );
                context.Role.Add(adminRole);
                await context.SaveChangesAsync();
            }

            Role? cashierRole = await context.Role
                .FirstOrDefaultAsync(r => r.Name == "Cashier");

            if (cashierRole is null)
            {
                string cashierPermsJson = "{\"Checkout\":1,\"ApplyDiscounts\":0," +
                    "\"Refunds\":0,\"ViewInventory\":1,\"ModifyInventory\":0," +
                    "\"ViewSensitiveInventoryCost\":0,\"ViewStaffInfo\":0," +
                    "\"ManageStaff\":0,\"ManageAccess\":0,\"ManagePayroll\":0," +
                    "\"ViewLogs\":0,\"DeleteLogs\":0,\"ManageSettings\":0}";
                cashierRole = new Role(
                    id: Utils.GenerateEntityId(),
                    name: "Cashier",
                    permissionsJson: cashierPermsJson
                );
                context.Role.Add(cashierRole);
                await context.SaveChangesAsync();
            }

            string hashedPassword = AuthenticationService.HashPassword(
                AdminPassword
            );

            Staff admin = new(
                id: Utils.GenerateEntityId(),
                username: AdminUsername,
                payFrequency: Staff.Frequency.Monthly,
                payRate: 0f,
                password: hashedPassword,
                firstName: AdminFirstName,
                lastName: AdminLastName,
                email: AdminEmail,
                position: "Administrator"
            );
            admin.Roles.Add(adminRole);
            admin.Permissions = new List<string>
            {
                "Checkout",
                "ApplyDiscounts",
                "Refunds",
                "ViewInventory",
                "ModifyInventory",
                "ViewSensitiveInventoryCost",
                "ViewStaffInfo",
                "ManageStaff",
                "ManageAccess",
                "ManagePayroll",
                "ViewLogs",
                "DeleteLogs",
                "ManageSettings",
                "AccessAdminSettings",
                "AccessPOS",
                "AccessInventory"
            };

            context.Staff.Add(admin);
            await context.SaveChangesAsync();

            StoreSettings settings = new()
            {
                StoreName = StoreName,
                Currency = Currency,
                StoreOpeningDate = DateTime.UtcNow
            };
            SettingsService.SaveSettings(settings);

            string message = "System setup completed successfully.";
            Log.Me(message);

            string successMsg = "Onboarding completed successfully! " +
                                "Please log in with your credentials.";
            TempData["SuccessMessage"] = successMsg;

            return RedirectToPage("/Login");
        }

        catch (Exception ex)
        {
            string errMessage = $"Error during setup: {ex.Message}";
            Log.Err(errMessage);
            ErrorMessage = errMessage;
            return Page();
        }
    }

    #endregion
}
