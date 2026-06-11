using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using Microsoft.AspNetCore.Mvc;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints for managing staff attendance log sheets.
/// <br/><br/>
/// <i>Documented by: Google Antigravity</i>
/// </summary>
public static class AttendanceController
{
    /// <summary>
    /// Logs a clock-in or clock-out event for a staff member.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request parameters containing staff ID and action type ("In" or "Out").</param>
    /// <returns>An HTTP result containing the timestamp of the logged event.</returns>
    public static async Task<IResult> LogAttendance([FromBody] LogAttendanceRequest request)
    {
        Utils.RequestResult<DateTime> result = await StaffService.LogAttendanceAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Creates a manual attendance entry.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
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
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
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
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
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
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
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
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request parameters containing the attendance ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeleteAttendance([FromBody] DeleteAttendanceRequest request)
    {
        Utils.RequestResult result = await StaffService.DeleteAttendanceAsync(request);
        return EndpointHelpers.ToIResult(result);
    }
}
