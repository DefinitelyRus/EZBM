using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.DesktopClient.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EZBM.DesktopClient.Pages;

/// <summary>
/// Page model for the analytics dashboard (home screen).
/// </summary>
public class IndexModel : PageModel
{

    #region Properties

    /// <summary>
    /// Today's total sales revenue.
    /// </summary>
    public float TodaySales { get; set; }

    /// <summary>
    /// Today's total profit.
    /// </summary>
    public float TodayProfit { get; set; }

    /// <summary>
    /// List of low stock items.
    /// </summary>
    public List<Item> LowStockItems { get; set; } = new();

    /// <summary>
    /// List of recent sales (latest 10 entries).
    /// </summary>
    public List<Sale> RecentSales { get; set; } = new();

    #endregion

    #region Handlers

    /// <summary>
    /// Handles GET request to fetch daily performance metrics and low stock alerts.
    /// </summary>
    public async Task OnGetAsync()
    {
        using AppDbContext context = new();
        DateTime todayUtc = DateTime.UtcNow.Date;

        // Fetch sale entries since start of today (UTC)
        List<SaleEntry> todaySaleEntries = await context.SaleEntry
            .Include(e => e.Sale)
            .Include(e => e.Item)
            .Where(e => e.Sale.Timestamp >= todayUtc)
            .ToListAsync();

        TodaySales = todaySaleEntries.Sum(e => e.Subtotal);

        TodayProfit = todaySaleEntries.Sum(
            e => e.Quantity * (e.UnitPrice - (e.Item.Cost ?? 0f))
        );

        var settings = SettingsService.LoadSettings();
        LowStockItems = await context.Item
            .Where(i => i.Quantity < settings.LowStockThreshold)
            .ToListAsync();

        RecentSales = await context.Sale
            .Include(s => s.Staff)
            .OrderByDescending(s => s.Timestamp)
            .Take(10)
            .ToListAsync();
    }


    /// <summary>
    /// Handles POST request to toggle employee attendance clock-in/out.
    /// </summary>
    public async Task<IActionResult> OnPostToggleAttendanceAsync(
        ulong staffId,
        string? returnUrl
    )
    {
        bool clockedIn = await StateHelper.IsClockedInAsync(staffId);
        string actionType = clockedIn ? "Out" : "In";

        LogAttendanceRequest request = new(
            StaffId: staffId,
            ActionType: actionType
        );

        await StaffService.LogAttendanceAsync(request);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToPage("/Index");
    }

    #endregion

}
