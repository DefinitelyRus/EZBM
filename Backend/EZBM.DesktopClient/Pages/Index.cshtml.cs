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
    #region Nested DTOs
    public record PopularProductDto(ulong ItemId, string Name, float TotalQuantitySold, float TotalRevenue);
    public record LeaderboardDto(ulong CashierId, string Username, string Name, int SalesCount, float TotalRevenue);
    public record TrendDto(string Date, float TotalAmount, int SalesCount);
    public record LowStockAlertDto(ulong Id, string Name, float Quantity, float TargetStock, float LowStockThresholdPercentage);
    #endregion

    #region Properties
    public float TodaySales { get; set; }
    public float TodayProfit { get; set; }
    public float TotalSales { get; set; }
    public float GrossProfit { get; set; }
    public float NetProfit { get; set; }
    public List<TrendDto> SalesVolumeTrends { get; set; } = [];
    public List<PopularProductDto> PopularProducts { get; set; } = [];
    public List<LeaderboardDto> CashierLeaderboard { get; set; } = [];
    public List<LowStockAlertDto> LowStockAlerts { get; set; } = [];
    public List<Sale> RecentSales { get; set; } = [];
    #endregion

    #region Handlers

    public async Task OnGetAsync()
    {
        using AppDbContext context = new();
        DateTime todayUtc = DateTime.UtcNow.Date;

        List<SaleEntry> todaySaleEntries = await context.SaleEntry
            .Include(e => e.Sale)
            .Include(e => e.Item)
            .Where(e => e.Sale.Timestamp >= todayUtc)
            .ToListAsync();

        TodaySales = todaySaleEntries.Sum(e => e.Subtotal);
        TodayProfit = todaySaleEntries.Sum(
            e => e.Quantity * (e.UnitPrice - (e.Item.Cost ?? 0f))
        );

        // 1. Total Sales (All Time)
        TotalSales = await context.Sale.SumAsync(s => s.Amount);

        // 2. Gross Profit
        var saleEntries = await context.SaleEntry.Include(se => se.Item).ToListAsync();
        GrossProfit = saleEntries.Sum(se => (se.UnitPrice - (se.Item?.Cost ?? 0f)) * se.Quantity);

        // 3. Net Profit (Gross Profit - Total Payroll Paid)
        float totalPayrollPaid = await context.Payroll.SumAsync(p => p.Amount);
        NetProfit = GrossProfit - totalPayrollPaid;

        // 4. Sales Volume Trends (Last 7 Days)
        DateTime sevenDaysAgo = todayUtc.AddDays(-6);
        var salesInWeek = await context.Sale
            .Where(s => s.Timestamp >= sevenDaysAgo)
            .ToListAsync();

        for (int i = 0; i < 7; i++)
        {
            DateTime day = sevenDaysAgo.AddDays(i);
            float daySales = salesInWeek.Where(s => s.Timestamp.Date == day.Date).Sum(s => s.Amount);
            int dayCount = salesInWeek.Count(s => s.Timestamp.Date == day.Date);
            SalesVolumeTrends.Add(new TrendDto(day.ToString("yyyy-MM-dd"), daySales, dayCount));
        }

        // 5. Popular Products
        PopularProducts = saleEntries
            .Where(se => se.Item is Product)
            .GroupBy(se => se.Item!.Id)
            .Select(g => new PopularProductDto(
                ItemId: g.Key,
                Name: g.First().Item!.Name,
                TotalQuantitySold: g.Sum(se => se.Quantity),
                TotalRevenue: g.Sum(se => se.Subtotal)
            ))
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(5)
            .ToList();

        // 6. Cashier Leaderboard
        var salesList = await context.Sale.Include(s => s.Staff).ToListAsync();
        CashierLeaderboard = salesList
            .GroupBy(s => s.Staff?.Id)
            .Where(g => g.Key != null)
            .Select(g => new LeaderboardDto(
                CashierId: g.Key!.Value,
                Username: g.First().Staff?.Username ?? "Unknown",
                Name: g.First().Staff != null ? $"{g.First().Staff.FirstName} {g.First().Staff.LastName}" : "Unknown",
                SalesCount: g.Count(),
                TotalRevenue: g.Sum(s => s.Amount)
            ))
            .OrderByDescending(x => x.TotalRevenue)
            .ToList();

        // 7. Low Stock Alerts (TPH Check)
        LowStockAlerts = await context.Product
            .Where(p => p.Quantity < p.TargetStock * p.LowStockThresholdPercentage)
            .Select(p => new LowStockAlertDto(
                Id: p.Id,
                Name: p.Name,
                Quantity: p.Quantity,
                TargetStock: p.TargetStock,
                LowStockThresholdPercentage: p.LowStockThresholdPercentage
            ))
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
