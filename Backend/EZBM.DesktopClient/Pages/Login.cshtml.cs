using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
    /// Whether RFID login is enabled globally in the store settings.
    /// </summary>
    public bool EnableRfidLogin { get; set; }

    #endregion

    #region Handlers

    /// <summary>
    /// Handles GET requests for the Login page and redirects to onboarding if database is empty.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        using EZBM.Core.Data.AppDbContext context = new();
        if (!await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(context.Staff))
        {
            return RedirectToPage("/Setup");
        }

        StoreSettings settings = SettingsService.LoadSettings();
        EnableRfidLogin = settings.EnableRfidLogin;

        if (TempData.TryGetValue("SuccessMessage", out var val) && val is string msg)
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

        if (EnableRfidLogin && !string.IsNullOrWhiteSpace(RfidCardId))
        {
            Utils.RequestResult<string> loginResult = await AuthenticationService.LoginByRfidAsync(RfidCardId);

            if (loginResult.Type != Utils.Result.Success)
            {
                ErrorMessage = loginResult.Message;
                return Page();
            }

            string? staffId = loginResult.Data;
            if (staffId is not null)
            {
                Response.Cookies.Append("ActiveStaffId", staffId, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });
            }

            return RedirectToPage("/Index");
        }

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Username and Password are required fields.";
            return Page();
        }

        LoginRequest loginRequest = new(
            Username: Username,
            Password: Password
        );

        Utils.RequestResult<string> loginResultNormal = await AuthenticationService.LoginAsync(loginRequest);

        if (loginResultNormal.Type != Utils.Result.Success)
        {
            ErrorMessage = loginResultNormal.Message;
            return Page();
        }

        string? staffIdNormal = loginResultNormal.Data;
        if (staffIdNormal is not null)
        {
            Response.Cookies.Append("ActiveStaffId", staffIdNormal, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });
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
