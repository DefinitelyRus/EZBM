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
    /// List of staff adjustments (bonuses, commissions, deductions).
    /// </summary>
    public List<StaffAdjustment> StaffAdjustments { get; set; } = new();

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

        // Load adjustments
        StaffAdjustments = await context.StaffAdjustment.OrderByDescending(a => a.Timestamp).ToListAsync();

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
        var result = await StaffService.CalculatePayrollDetailsAsync(staffId, periodStart, periodEnd);
        if (result.Type == Utils.Result.Success && result.Data != null)
        {
            var data = result.Data;
            return new JsonResult(new
            {
                success = true,
                totalHours = Math.Round(data.TotalHours, 2),
                grossAmount = Math.Round(data.GrossAmount, 2),
                commission = Math.Round(data.CommissionsAndBonuses, 2),
                deductions = Math.Round(data.Deductions, 2),
                netAmount = Math.Round(data.NetAmount, 2)
            });
        }
        else
        {
            return new JsonResult(new { success = false, message = result.Message });
        }
    }

    public async Task<IActionResult> OnPostCreateAdjustmentAsync(
        ulong staffId,
        string adjustmentType,
        float amount,
        bool deductFromCurrentPayroll,
        string? notes
    )
    {
        if (amount < 0f)
        {
            ErrorMessage = "Adjustment amount cannot be negative.";
            return RedirectToPage("/Logs", new { tab = "adjustments" });
        }

        try
        {
            using AppDbContext context = new();
            StaffAdjustment adj = new(
                id: Utils.GenerateEntityId(),
                staffId: staffId,
                adjustmentType: adjustmentType,
                amount: amount,
                deductFromCurrentPayroll: deductFromCurrentPayroll,
                isPaid: false,
                timestamp: DateTime.UtcNow,
                notes: notes
            );

            context.StaffAdjustment.Add(adj);
            await context.SaveChangesAsync();
            SuccessMessage = "Staff adjustment logged successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to log adjustment: {ex.Message}";
        }

        return RedirectToPage("/Logs", new { tab = "adjustments" });
    }

    public async Task<IActionResult> OnPostDeleteAdjustmentAsync(
        ulong id
    )
    {
        try
        {
            using AppDbContext context = new();
            var adj = await context.StaffAdjustment.FindAsync(id);
            if (adj != null)
            {
                context.StaffAdjustment.Remove(adj);
                await context.SaveChangesAsync();
                SuccessMessage = "Staff adjustment deleted successfully.";
            }
            else
            {
                ErrorMessage = "Adjustment not found.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete adjustment: {ex.Message}";
        }

        return RedirectToPage("/Logs", new { tab = "adjustments" });
    }

    #endregion
}
