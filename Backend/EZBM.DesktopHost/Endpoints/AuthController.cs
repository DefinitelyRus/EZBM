using Microsoft.AspNetCore.Mvc;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using System.ComponentModel.DataAnnotations;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Represents a login request payload.
/// </summary>
public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

/// <summary>
/// Exposes authentication and authorization endpoints.
/// </summary>
public static class AuthController
{
    /// <summary>
    /// Authenticates a staff member.
    /// </summary>
    /// <param name="request">The credentials request payload.</param>
    /// <returns>An HTTP result containing the staff member details if successful.</returns>
    public static async Task<IResult> Login([FromBody] LoginRequest request)
    {
        Staff? staff = await StaffService.GetStaffByUsernameAsync(request.Username);

        if (staff is null)
        {
            Log.Me(() => $"Staff with username '{request.Username}' not found in database.");
            return Results.Json(new { error = "Invalid username or password." }, statusCode: StatusCodes.Status401Unauthorized);
        }

        if (staff.Password != request.Password)
        {
            Log.Me(() => $"Incorrect password for staff member with username '{request.Username}'.");
            return Results.Json(new { error = "Invalid username or password." }, statusCode: StatusCodes.Status401Unauthorized);
        }

        Log.Me(() => "Logged in successfully.");

        return Results.Ok(new
        {
            id = staff.Id,
            username = staff.Username,
            position = staff.Position,
            payFrequency = staff.PayFrequency.ToString(),
            payRate = staff.PayRate
        });
    }
}