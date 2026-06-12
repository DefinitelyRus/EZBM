using EZBM.Core.Data;
using EZBM.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZBM.DesktopClient.Helpers;

/// <summary>
/// Helper for managing active employee login state and attendance status via cookies and database checks.
/// <br/><br/>
/// <i>Documented by: Google Antigravity</i>
/// </summary>
public static class StateHelper
{

    #region State Retrieval Operations

    /// <summary>
    /// Retrieves the currently logged-in staff member based on the cookie.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>The active Staff member, or null if not logged in.</returns>
    public static async Task<Staff?> GetActiveStaffAsync(
        HttpContext httpContext
    )
    {
        string? activeStaffIdStr = httpContext.Request.Cookies["ActiveStaffId"];

        if (string.IsNullOrEmpty(activeStaffIdStr))
            return null;

        if (!ulong.TryParse(activeStaffIdStr, out ulong staffId))
            return null;

        try
        {
            using AppDbContext context = new();
            Staff? staff = await context.Staff.FindAsync(staffId);
            return staff;
        }

        catch
        {
            return null;
        }
    }


    /// <summary>
    /// Checks if the given staff member is currently clocked in.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="staffId">The ID of the staff member.</param>
    /// <returns>True if clocked in, false otherwise.</returns>
    public static async Task<bool> IsClockedInAsync(
        ulong staffId
    )
    {
        try
        {
            using AppDbContext context = new();
            bool isClockedIn = await context.Attendance.AnyAsync(
                a => a.Staff.Id == staffId && a.TimeOut == null
            );

            return isClockedIn;
        }

        catch
        {
            return false;
        }
    }

    #endregion

}
