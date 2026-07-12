using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EZBM.DesktopClient.Helpers;

/// <summary>
/// Helper for managing active employee login state and attendance status via cookies and database checks.
/// </summary>
public static class StateHelper
{
    #region State Retrieval Operations

    /// <summary>
    /// Retrieves the currently logged-in staff member based on the cookie.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>The active Staff member, or null if not logged in.</returns>
    public static async Task<Staff?> GetActiveStaffAsync(
        HttpContext httpContext
    )
    {
        if (httpContext == null) return null;

        if (httpContext.Items.TryGetValue("ActiveStaff", out var cached) && cached is Staff cachedStaff)
            return cachedStaff;

        string? activeStaffIdStr = httpContext.Request.Cookies["ActiveStaffId"];

        if (string.IsNullOrEmpty(activeStaffIdStr))
            return null;

        if (!ulong.TryParse(activeStaffIdStr, out ulong staffId))
            return null;

        try
        {
            using AppDbContext context = new();
            Staff? staff = await context.Staff.FindAsync(staffId);
            if (staff is not null)
            {
                httpContext.Items["ActiveStaff"] = staff;
            }
            return staff;
        }

        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Checks if the given staff member is currently clocked in.
    /// </summary>
    /// <param name="staffId">The ID of the staff member.</param>
    /// <returns>True if clocked in, false otherwise.</returns>
    public static async Task<bool> IsClockedInAsync(
        ulong staffId
    )
    {
        try
        {
            using AppDbContext context = new();
            bool isClockedIn = await context.Attendance.AnyAsync(
                a => a.Staff.Id == staffId && a.TimeOut == null
            );

            return isClockedIn;
        }

        catch
        {
            return false;
        }
    }

    #endregion

    #region Formatting & Settings Helpers

    /// <summary>
    /// Gets the currency symbol matching the active store settings.
    /// </summary>
    /// <returns>The currency symbol string.</returns>
    public static string GetCurrencySymbol()
    {
        var settings = SettingsService.LoadSettings();
        return settings.Currency switch
        {
            "PHP" => "₱",
            "USD" => "$",
            "EUR" => "€",
            "SGD" => "S$",
            _ => settings.Currency + " "
        };
    }

    /// <summary>
    /// Formats a float value as a currency string with the active currency symbol.
    /// </summary>
    /// <param name="amount">The float amount to format.</param>
    /// <returns>The formatted currency string, or "N/A" if amount is null.</returns>
    public static string FormatCurrency(float? amount)
    {
        if (!amount.HasValue) return "N/A";
        return $"{GetCurrencySymbol()}{EZBM.Core.Tools.Utils.FormatDecimal(amount.Value)}";
    }

    /// <summary>
    /// Formats a quantity value cleanly according to display rules.
    /// </summary>
    public static string FormatQuantity(float quantity, Item.Unit unit, float? targetStock = null)
    {
        if (quantity == -1f || unit == Item.Unit.Unlimited || quantity >= 9990f)
        {
            return "Unlimited";
        }
        string qtyStr = EZBM.Core.Tools.Utils.FormatDecimal(quantity);
        string unitStr = unit == Item.Unit.Count ? "" : $" {unit}";
        
        if (targetStock.HasValue)
        {
            string targetStr = EZBM.Core.Tools.Utils.FormatDecimal(targetStock.Value);
            return $"{qtyStr}{unitStr} / {targetStr}{unitStr}";
        }
        return $"{qtyStr}{unitStr}";
    }

    /// <summary>
    /// Formats a date using the user's preferred date format settings.
    /// </summary>
    /// <param name="dateTime">The date to format.</param>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="includeTime">Whether to include time details.</param>
    /// <param name="includeSeconds">Whether to include seconds details.</param>
    /// <returns>The formatted date string.</returns>
    public static string FormatDate(DateTime? dateTime, HttpContext httpContext, bool includeTime = false, bool includeSeconds = false)
    {
        if (!dateTime.HasValue) return "N/A";
        string format = "yyyy-MM-dd";
        try
        {
            Staff? activeStaff = null;
            if (httpContext != null && httpContext.Items.TryGetValue("ActiveStaff", out var cached) && cached is Staff cachedStaff)
            {
                activeStaff = cachedStaff;
            }
            else if (httpContext != null)
            {
                activeStaff = GetActiveStaffAsync(httpContext).GetAwaiter().GetResult();
            }

            if (activeStaff != null)
            {
                string userSettingsJson = SettingsService.LoadUserSettings(activeStaff.Id);
                var userPrefs = JsonSerializer.Deserialize<EZBM.DesktopClient.Pages.SettingsModel.UserPreferences>(userSettingsJson);
                if (userPrefs != null && !string.IsNullOrEmpty(userPrefs.DateFormat))
                {
                    format = userPrefs.DateFormat;
                }
            }
        }
        catch
        {
            // ignore
        }

        if (includeTime)
        {
            format += includeSeconds ? " HH:mm:ss" : " HH:mm";
        }
        return dateTime.Value.ToString(format);
    }

    /// <summary>
    /// Calculates the expected cash in the drawer for the employee's active shift.
    /// </summary>
    /// <param name="staffId">The ID of the staff member.</param>
    /// <returns>The calculated expected cash amount.</returns>
    public static async Task<float> CalculateExpectedShiftCashAsync(ulong staffId)
    {
        try
        {
            using AppDbContext context = new();
            var activeAttendance = await context.Attendance
                .Where(a => a.Staff.Id == staffId && a.TimeOut == null)
                .OrderByDescending(a => a.TimeIn)
                .FirstOrDefaultAsync();

            if (activeAttendance == null) return 0f;

            var transactions = await context.Transaction
                .Include(t => t.ChildTransactions)
                .Where(t => t.Staff.Id == staffId && t.Timestamp >= activeAttendance.TimeIn)
                .ToListAsync();

            float expectedCash = 0f;
            foreach (var tx in transactions)
            {
                if (tx.TransactionType == Transaction.Type.Income)
                {
                    if (tx.PaymentMethod == Transaction.PayMethod.Cash)
                    {
                        expectedCash += tx.Amount;
                    }
                    else if (tx.PaymentMethod == Transaction.PayMethod.Mixed)
                    {
                        expectedCash += tx.ChildTransactions
                            .Where(c => c.PaymentMethod == Transaction.PayMethod.Cash)
                            .Sum(c => c.Amount);
                    }
                }
                else if (tx.TransactionType == Transaction.Type.Expense)
                {
                    if (tx.PaymentMethod == Transaction.PayMethod.Cash)
                    {
                        expectedCash -= tx.Amount;
                    }
                }
            }

            return expectedCash;
        }
        catch
        {
            return 0f;
        }
    }

    #endregion
}

