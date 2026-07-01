using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EZBM.DesktopClient.Pages;

/// <summary>
/// Page model for shift attendance logs and payroll records.
/// </summary>
public class LogsModel : PageModel
{

    #region Properties

    /// <summary>
    /// The currently selected tab (either "attendance" or "payroll").
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

        // Load attendance
        Utils.RequestResult<List<Attendance>> attendanceResult = await StaffService.FindAttendanceAsync(null!);
        AttendanceLogs = attendanceResult.Data ?? new List<Attendance>();

        // Load payroll
        Utils.RequestResult<List<Payroll>> payrollResult = await StaffService.FindPayrollAsync(null!);
        PayrollLogs = payrollResult.Data ?? new List<Payroll>();

        // Load staff for dropdowns
        Utils.RequestResult<List<Staff>> staffResult = await StaffService.FindStaffAsync(null!);
        StaffList = staffResult.Data ?? new List<Staff>();
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

    #endregion

}
