using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes dashboard and analytics endpoints.
/// </summary>
public static class AnalyticsController
{
    /// <summary>
    /// Computes shop-wide dashboard metrics and return results as JSON.
    /// </summary>
    public static async Task<IResult> GetAnalytics()
    {
        try
        {
            using AppDbContext context = new();

            // Total Sales
            float totalSales = await context.Sale.SumAsync(s => s.Amount);

            // Gross Profit
            var saleEntries = await context.SaleEntry
                .Include(se => se.Item)
                .ToListAsync();

            float grossProfit = 0;
            foreach (var se in saleEntries)
            {
                float cost = se.Item?.Cost ?? 0f;
                grossProfit += (se.UnitPrice - cost) * se.Quantity;
            }

            // Net Profit (Gross Profit - Total Payroll Paid)
            float totalPayrollPaid = await context.Payroll.SumAsync(p => p.Amount);
            float netProfit = grossProfit - totalPayrollPaid;

            // Sales Volume Trends (Last 7 days)
            DateTime sevenDaysAgo = DateTime.UtcNow.Date.AddDays(-6);
            var salesInWeek = await context.Sale
                .Where(s => s.Timestamp >= sevenDaysAgo)
                .ToListAsync();

            var trends = new List<object>();
            for (int i = 0; i < 7; i++)
            {
                DateTime day = sevenDaysAgo.AddDays(i);
                float daySales = salesInWeek.Where(s => s.Timestamp.Date == day.Date).Sum(s => s.Amount);
                int dayCount = salesInWeek.Count(s => s.Timestamp.Date == day.Date);
                trends.Add(new
                {
                    Date = day.ToString("yyyy-MM-dd"),
                    TotalAmount = daySales,
                    SalesCount = dayCount
                });
            }

            // Popular Products
            var popularProducts = saleEntries
                .Where(se => se.Item is Product)
                .GroupBy(se => se.Item!.Id)
                .Select(g => new
                {
                    ItemId = g.Key,
                    Name = g.First().Item!.Name,
                    TotalQuantitySold = g.Sum(se => se.Quantity),
                    TotalRevenue = g.Sum(se => se.Subtotal)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(5)
                .ToList();

            // Cashier Leaderboard
            var salesList = await context.Sale.Include(s => s.Staff).ToListAsync();
            var leaderboard = salesList
                .GroupBy(s => s.Staff?.Id)
                .Where(g => g.Key != null)
                .Select(g => new
                {
                    CashierId = g.Key,
                    Username = g.First().Staff?.Username ?? "Unknown",
                    SalesCount = g.Count(),
                    TotalRevenue = g.Sum(s => s.Amount)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            // Alert for low stock items
            var lowStockItems = await context.Product
                .Where(p => p.Quantity < p.TargetStock * p.LowStockThresholdPercentage)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Quantity,
                    p.TargetStock,
                    p.LowStockThresholdPercentage
                })
                .ToListAsync();

            return Results.Ok(new
            {
                TotalSales = totalSales,
                GrossProfit = grossProfit,
                NetProfit = netProfit,
                SalesVolumeTrends = trends,
                PopularProducts = popularProducts,
                CashierLeaderboard = leaderboard,
                LowStockAlerts = lowStockItems
            });
        }
        catch (Exception ex)
        {
            return Results.Json(new { error = $"Failed to fetch analytics: {ex.Message}" }, statusCode: 500);
        }
    }
}
