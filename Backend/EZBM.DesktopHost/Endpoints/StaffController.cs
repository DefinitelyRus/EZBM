using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using Microsoft.AspNetCore.Mvc;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints for managing staff profiles.
/// <br/><br/>
/// <i>Documented by: Google Antigravity</i>
/// </summary>
public static class StaffController
{
    /// <summary>
    /// Creates a new staff member profile.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
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
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
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
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
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
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
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
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request parameters containing the staff ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeleteStaff([FromBody] DeleteStaffRequest request)
    {
        Utils.RequestResult result = await StaffService.DeleteStaffAsync(request);
        return EndpointHelpers.ToIResult(result);
    }
}
