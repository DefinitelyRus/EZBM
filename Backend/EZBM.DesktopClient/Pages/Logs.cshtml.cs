using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EZBM.DesktopClient.Pages;

/// <summary>
/// Page model for shift attendance logs, payroll records, and system audit logs.
/// </summary>
public class LogsModel : PageModel
{
    #region Properties

    /// <summary>
    /// The currently selected tab (either "attendance", "payroll", or "actions").
    /// </summary>
    public string ActiveTab { get; set; } = "attendance";

    /// <summary>
    /// List of attendance records.
    /// </summary>
    public List<Attendance> AttendanceLogs { get; set; } = new();

    /// <summary>
    /// List of payroll history records.
    /// </summary>
    public List<Payroll> PayrollLogs { get; set; } = new();

    /// <summary>
    /// List of system action logs.
    /// </summary>
    public List<ActionLog> ActionLogs { get; set; } = new();

    /// <summary>
    /// List of all staff members (used in payroll/attendance creation forms).
    /// </summary>
    public List<Staff> StaffList { get; set; } = new();

    /// <summary>
    /// Temporary error message.
    /// </summary>
    [TempData]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Temporary success message.
    /// </summary>
    [TempData]
    public string? SuccessMessage { get; set; }

    #endregion

    #region Handlers

    /// <summary>
    /// Handles GET request to fetch logs and staff options.
    /// </summary>
    public async Task OnGetAsync(
        string? tab
    )
    {
        if (!string.IsNullOrEmpty(tab))
            ActiveTab = tab.ToLower();

        using AppDbContext context = new();

        // Load attendance
        Utils.RequestResult<List<Attendance>> attendanceResult = await StaffService.FindAttendanceAsync(null!);
        AttendanceLogs = attendanceResult.Data ?? new List<Attendance>();

        // Load payroll
        Utils.RequestResult<List<Payroll>> payrollResult = await StaffService.FindPayrollAsync(null!);
        PayrollLogs = payrollResult.Data ?? new List<Payroll>();

        // Load staff for dropdowns
        Utils.RequestResult<List<Staff>> staffResult = await StaffService.FindStaffAsync(null!);
        StaffList = staffResult.Data ?? new List<Staff>();

        // Load action logs
        ActionLogs = await context.ActionLog.OrderByDescending(l => l.Timestamp).ToListAsync();
    }

    /// <summary>
    /// Handles manual creation of attendance records.
    /// </summary>
    public async Task<IActionResult> OnPostCreateAttendanceAsync(
        ulong staffId,
        DateTime timeIn,
        DateTime? timeOut
    )
    {
        CreateAttendanceRequest request = new(
            StaffId: staffId,
            TimeIn: timeIn,
            TimeOut: timeOut
        );

        Utils.RequestResult result = await StaffService.CreateAttendanceAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = "Attendance record created successfully.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Logs", new { tab = "attendance" });
    }

    /// <summary>
    /// Handles manual creation of payroll logs.
    /// </summary>
    public async Task<IActionResult> OnPostCreatePayrollAsync(
        ulong staffId,
        DateTime periodStart,
        DateTime periodEnd,
        float totalHours,
        float grossAmount,
        float modifiers,
        float netAmount,
        DateTime? payDate,
        string? notes
    )
    {
        if (totalHours < 0f)
        {
            ErrorMessage = "Total hours cannot be negative.";
            return RedirectToPage("/Logs", new { tab = "payroll" });
        }

        if (grossAmount < 0f)
        {
            ErrorMessage = "Gross amount cannot be negative.";
            return RedirectToPage("/Logs", new { tab = "payroll" });
        }

        if (netAmount < 0f)
        {
            ErrorMessage = "Net paid amount cannot be negative.";
            return RedirectToPage("/Logs", new { tab = "payroll" });
        }

        CreatePayrollRequest request = new(
            StaffId: staffId,
            PeriodStart: periodStart,
            PeriodEnd: periodEnd,
            TotalHours: totalHours,
            GrossAmount: grossAmount,
            Modifiers: modifiers,
            NetAmount: netAmount,
            PayDate: payDate,
            Notes: notes
        );

        Utils.RequestResult result = await StaffService.CreatePayrollAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = "Payroll log created successfully.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Logs", new { tab = "payroll" });
    }

    /// <summary>
    /// Handles deleting an attendance record.
    /// </summary>
    public async Task<IActionResult> OnPostDeleteAttendanceAsync(
        ulong id
    )
    {
        DeleteAttendanceRequest request = new(Id: id);
        Utils.RequestResult result = await StaffService.DeleteAttendanceAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = "Attendance record deleted.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Logs", new { tab = "attendance" });
    }

    /// <summary>
    /// Handles deleting a payroll record.
    /// </summary>
    public async Task<IActionResult> OnPostDeletePayrollAsync(
        ulong id
    )
    {
        DeletePayrollRequest request = new(Id: id);
        Utils.RequestResult result = await StaffService.DeletePayrollAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = "Payroll record deleted.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Logs", new { tab = "payroll" });
    }

    /// <summary>
    /// Calculates stats (hours, gross pay, upgrade commission, net pay) dynamically for payroll form.
    /// </summary>
    public async Task<IActionResult> OnGetCalculatePayrollStatsAsync(
        ulong staffId,
        DateTime periodStart,
        DateTime periodEnd)
    {
        try
        {
            using AppDbContext db = new();
            Staff? staff = await db.Staff.FindAsync(staffId);
            if (staff is null)
            {
                return new JsonResult(new { success = false, message = "Staff member not found" });
            }

            List<Attendance> attendanceLogs = await db.Attendance
                .Where(a => a.Staff.Id == staffId && a.TimeIn >= periodStart && a.TimeIn <= periodEnd && a.TimeOut is not null)
                .ToListAsync();

            double totalHours = attendanceLogs.Sum(a => (a.TimeOut!.Value - a.TimeIn).TotalHours);
            float grossAmount = (float)(totalHours * staff.PayRate);

            float commission = await CalculateUpgradeCommissionsAsync(staffId, periodStart, periodEnd);
            float netAmount = grossAmount + commission;

            return new JsonResult(new
            {
                success = true,
                totalHours = Math.Round(totalHours, 2),
                grossAmount = Math.Round(grossAmount, 2),
                commission = Math.Round(commission, 2),
                netAmount = Math.Round(netAmount, 2)
            });
        }

        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }

    private static async Task<float> CalculateUpgradeCommissionsAsync(
        ulong staffId,
        DateTime periodStart,
        DateTime periodEnd)
    {
        using AppDbContext db = new();
        StoreSettings settings = SettingsService.LoadSettings();
        Dictionary<string, float> commissionRates = settings.MembershipCommissions;

        List<Sale> sales = await db.Sale
            .Include(s => s.Customer)
            .Where(s => s.Staff.Id == staffId && s.Timestamp >= periodStart && s.Timestamp <= periodEnd)
            .ToListAsync();

        float totalCommission = 0f;

        foreach (Sale sale in sales)
        {
            List<SaleEntry> entries = await db.SaleEntry
                .Include(se => se.Item)
                .Where(se => se.Sale.Id == sale.Id)
                .ToListAsync();

            foreach (SaleEntry entry in entries)
            {
                string itemName = entry.Item.Name;
                if (commissionRates.ContainsKey(itemName))
                {
                    float currentRate = commissionRates[itemName];
                    float subtractRate = 0f;

                    if (sale.Customer is not null)
                    {
                        List<SaleEntry> prevEntries = await db.SaleEntry
                            .Include(se => se.Sale)
                            .Include(se => se.Item)
                            .Where(se => se.Sale.Customer is not null && se.Sale.Customer.Id == sale.Customer.Id && se.Sale.Timestamp < sale.Timestamp)
                            .ToListAsync();

                        foreach (SaleEntry prevEntry in prevEntries)
                        {
                            string prevItemName = prevEntry.Item.Name;
                            if (commissionRates.ContainsKey(prevItemName))
                            {
                                float prevRate = commissionRates[prevItemName];
                                if (prevRate > subtractRate && prevRate < currentRate)
                                {
                                    subtractRate = prevRate;
                                }
                            }
                        }
                    }

                    totalCommission += (currentRate - subtractRate);
                }
            }
        }

        return totalCommission;
    }

    #endregion
}
