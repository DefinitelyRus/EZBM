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
/// Page model for the login screen.
/// </summary>
public class LoginModel : PageModel
{
    #region Properties

    /// <summary>
    /// The username entered by the user.
    /// </summary>
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password entered by the user.
    /// </summary>
    [BindProperty]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Holds validation or operational error messages.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Holds informational success messages.
    /// </summary>
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// The RFID card ID scanned by the user.
    /// </summary>
    [BindProperty]
    public string RfidCardId { get; set; } = string.Empty;

    /// <summary>
    /// Whether RFID login is enabled globally in store settings.
    /// </summary>
    public bool EnableRfidLogin { get; set; }

    #endregion

    #region Handlers

    /// <summary>
    /// Handles GET requests for the Login page and redirects to setup if DB empty.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        using AppDbContext context = new();
        bool hasStaff = await context.Staff.AnyAsync();

        if (!hasStaff)
        {
            return RedirectToPage("/Setup");
        }

        StoreSettings settings = SettingsService.LoadSettings();
        EnableRfidLogin = settings.EnableRfidLogin;

        if (TempData.TryGetValue("SuccessMessage", out object? val) &&
            val is string msg)
        {
            SuccessMessage = msg;
        }

        return Page();
    }

    /// <summary>
    /// Handles login submission and sets user context.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        StoreSettings settings = SettingsService.LoadSettings();
        EnableRfidLogin = settings.EnableRfidLogin;

        bool hasRfid = !string.IsNullOrWhiteSpace(RfidCardId);

        if (EnableRfidLogin && hasRfid)
        {
            Utils.RequestResult<string> loginResult =
                await AuthenticationService.LoginByRfidAsync(RfidCardId);

            if (loginResult.Type != Utils.Result.Success)
            {
                ErrorMessage = loginResult.Message;
                return Page();
            }

            string? staffId = loginResult.Data;
            if (staffId is not null)
            {
                CookieOptions cookieOpts = new()
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Append("ActiveStaffId", staffId, cookieOpts);
            }

            return RedirectToPage("/Index");
        }

        bool hasUsername = !string.IsNullOrWhiteSpace(Username);
        bool hasPassword = !string.IsNullOrWhiteSpace(Password);

        if (!hasUsername || !hasPassword)
        {
            ErrorMessage = "Username and Password are required fields.";
            return Page();
        }

        LoginRequest loginRequest = new(
            Username: Username,
            Password: Password
        );

        Utils.RequestResult<string> loginResultNormal =
            await AuthenticationService.LoginAsync(loginRequest);

        if (loginResultNormal.Type != Utils.Result.Success)
        {
            ErrorMessage = loginResultNormal.Message;
            return Page();
        }

        string? staffIdNormal = loginResultNormal.Data;
        if (staffIdNormal is not null)
        {
            CookieOptions cookieOptsNormal = new()
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append(
                "ActiveStaffId",
                staffIdNormal,
                cookieOptsNormal
            );
        }

        return RedirectToPage("/Index");
    }

    /// <summary>
    /// Handles logout requests by clearing the login cookie.
    /// </summary>
    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete("ActiveStaffId");
        return RedirectToPage("/Login");
    }

    #endregion
}
