using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EZBM.DesktopClient.Pages;

/// <summary>
/// Page model for the staff directory and profile configuration.
/// <br/><br/>
/// <i>Documented by: Google Antigravity</i>
/// </summary>
public class StaffModel : PageModel
{

    #region Properties

    /// <summary>
    /// The list of staff members retrieved from the database.
    /// </summary>
    public List<Staff> StaffList { get; set; } = new();

    /// <summary>
    /// Profile currently being edited, if action is edit.
    /// </summary>
    public Staff? EditingStaff { get; set; }

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
    /// Handles GET request for listing staff and setting up edit mode.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    public async Task OnGetAsync(
        string? action,
        ulong? id
    )
    {
        Utils.RequestResult<List<Staff>> result = await StaffService.FindStaffAsync(null!);
        StaffList = result.Data ?? new List<Staff>();

        if (action == "edit" && id.HasValue)
        {
            Utils.RequestResult<Staff> getStaffResult = await StaffService.GetStaffAsync(
                new GetStaffRequest(id.Value)
            );

            if (getStaffResult.Type == Utils.Result.Success)
                EditingStaff = getStaffResult.Data;
        }
    }


    /// <summary>
    /// Handles creating a new staff member profile.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    public async Task<IActionResult> OnPostCreateAsync(
        string username,
        string? password,
        string? firstName,
        string? lastName,
        string? email,
        string? phoneNumber,
        string? position,
        Staff.Frequency payFrequency,
        float payRate
    )
    {
        CreateStaffRequest request = new(
            Username: username,
            Password: password,
            FirstName: firstName,
            LastName: lastName,
            Email: email,
            PhoneNumber: phoneNumber,
            Position: position,
            PayFrequency: payFrequency,
            PayRate: payRate
        );

        Utils.RequestResult result = await StaffService.CreateStaffAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = $"Staff member '{username}' registered successfully.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Staff");
    }


    /// <summary>
    /// Handles updating an existing staff member's profile.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    public async Task<IActionResult> OnPostUpdateAsync(
        ulong id,
        string username,
        string? password,
        string? firstName,
        string? lastName,
        string? email,
        string? phoneNumber,
        string? position,
        Staff.Frequency payFrequency,
        float payRate
    )
    {
        UpdateStaffRequest request = new(
            Id: id,
            Username: username,
            Password: password,
            FirstName: firstName,
            LastName: lastName,
            Email: email,
            PhoneNumber: phoneNumber,
            Position: position,
            PayFrequency: payFrequency,
            PayRate: payRate
        );

        Utils.RequestResult result = await StaffService.UpdateStaffAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = $"Staff member ID {id} updated successfully.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Staff");
    }


    /// <summary>
    /// Handles deleting a staff profile.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    public async Task<IActionResult> OnPostDeleteAsync(
        ulong id
    )
    {
        DeleteStaffRequest request = new(Id: id);
        Utils.RequestResult result = await StaffService.DeleteStaffAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = $"Staff profile ID {id} deleted successfully.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Staff");
    }

    #endregion

}
