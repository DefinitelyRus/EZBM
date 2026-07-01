using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.DesktopHost.Tools;
using Microsoft.AspNetCore.Mvc;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints for managing staff profiles.
/// </summary>
public static class StaffController
{

    #region Staff requests

    /// <summary>
    /// Creates a new staff member profile.
    /// </summary>
    /// <param name="request">The request parameters containing staff profile details.</param>
    /// <returns>An HTTP result indicating the status of the staff creation.</returns>
    public static async Task<IResult> CreateStaff([FromBody] CreateStaffRequest request)
    {
        Utils.RequestResult result = await StaffService.CreateStaffAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Retrieves a specific staff member by their identifier.
    /// </summary>
    /// <param name="request">The request parameters containing the staff ID.</param>
    /// <returns>An HTTP result with the staff details if found.</returns>
    public static async Task<IResult> GetStaff([FromBody] GetStaffRequest request)
    {
        Utils.RequestResult<Staff> result = await StaffService.GetStaffAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Finds staff profiles matching the specified query filters.
    /// </summary>
    /// <param name="request">The request parameters containing search filters.</param>
    /// <returns>An HTTP result with the list of matching staff members.</returns>
    public static async Task<IResult> FindStaff([FromBody] FindStaffRequest request)
    {
        Utils.RequestResult<List<Staff>> result = await StaffService.FindStaffAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Updates a specific staff member's profile.
    /// </summary>
    /// <param name="request">The request parameters containing update fields and ID.</param>
    /// <returns>An HTTP result indicating the status of the update.</returns>
    public static async Task<IResult> UpdateStaff([FromBody] UpdateStaffRequest request)
    {
        Utils.RequestResult result = await StaffService.UpdateStaffAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Deletes a specific staff profile by its identifier.
    /// </summary>
    /// <param name="request">The request parameters containing the staff ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeleteStaff([FromBody] DeleteStaffRequest request)
    {
        Utils.RequestResult result = await StaffService.DeleteStaffAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    #endregion

    #region Attendance requests

    /// <summary>
    /// Logs a clock-in or clock-out event for a staff member.
    /// </summary>
    /// <param name="request">The request parameters containing staff ID and action type ("In" or "Out").</param>
    /// <returns>An HTTP result containing the timestamp of the logged event.</returns>
    public static async Task<IResult> LogAttendance(
        [FromBody] LogAttendanceRequest request)
    {
        Utils.RequestResult<DateTime> result = await StaffService.LogAttendanceAsync(request);

        if (result.Type == Utils.Result.Success)
        {
            return Results.Ok(new
            {
                success = true,
                timestamp = result.Data
            });
        }

        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Creates a manual attendance entry.
    /// </summary>
    /// <param name="request">The request parameters containing attendance details.</param>
    /// <returns>An HTTP result indicating the status of the attendance creation.</returns>
    public static async Task<IResult> CreateAttendance([FromBody] CreateAttendanceRequest request)
    {
        Utils.RequestResult result = await StaffService.CreateAttendanceAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Retrieves a specific attendance record by its identifier.
    /// </summary>
    /// <param name="request">The request parameters containing the attendance ID.</param>
    /// <returns>An HTTP result with the attendance details if found.</returns>
    public static async Task<IResult> GetAttendance([FromBody] GetAttendanceRequest request)
    {
        Utils.RequestResult<Attendance> result = await StaffService.GetAttendanceAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Finds attendance records matching the specified query filters.
    /// </summary>
    /// <param name="request">The request parameters containing search filters.</param>
    /// <returns>An HTTP result with the list of matching attendance records.</returns>
    public static async Task<IResult> FindAttendance([FromBody] FindAttendanceRequest request)
    {
        Utils.RequestResult<List<Attendance>> result = await StaffService.FindAttendanceAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Updates a specific attendance record.
    /// </summary>
    /// <param name="request">The request parameters containing update fields and ID.</param>
    /// <returns>An HTTP result indicating the status of the update.</returns>
    public static async Task<IResult> UpdateAttendance([FromBody] UpdateAttendanceRequest request)
    {
        Utils.RequestResult result = await StaffService.UpdateAttendanceAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Deletes a specific attendance record by its identifier.
    /// </summary>
    /// <param name="request">The request parameters containing the attendance ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeleteAttendance([FromBody] DeleteAttendanceRequest request)
    {
        Utils.RequestResult result = await StaffService.DeleteAttendanceAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    #endregion

    #region Payroll requests

    /// <summary>
    /// Creates a new payroll record.
    /// </summary>
    /// <param name="request">The request parameters containing payroll details.</param>
    /// <returns>An HTTP result indicating the status of the payroll creation.</returns>
    public static async Task<IResult> CreatePayroll([FromBody] CreatePayrollRequest request)
    {
        Utils.RequestResult result = await StaffService.CreatePayrollAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Retrieves a specific payroll record by its identifier.
    /// </summary>
    /// <param name="request">The request parameters containing the payroll ID.</param>
    /// <returns>An HTTP result with the payroll details if found.</returns>
    public static async Task<IResult> GetPayroll([FromBody] GetPayrollRequest request)
    {
        Utils.RequestResult<Payroll> result = await StaffService.GetPayrollAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Finds payroll records matching the specified query filters.
    /// </summary>
    /// <param name="request">The request parameters containing search filters.</param>
    /// <returns>An HTTP result with the list of matching payroll records.</returns>
    public static async Task<IResult> FindPayroll([FromBody] FindPayrollRequest request)
    {
        Utils.RequestResult<List<Payroll>> result = await StaffService.FindPayrollAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Deletes a specific payroll record by its identifier.
    /// </summary>
    /// <param name="request">The request parameters containing the payroll ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeletePayroll([FromBody] DeletePayrollRequest request)
    {
        Utils.RequestResult result = await StaffService.DeletePayrollAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    #endregion
}
