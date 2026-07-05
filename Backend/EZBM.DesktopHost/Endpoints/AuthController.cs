using Microsoft.AspNetCore.Mvc;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.Core.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Represents a login request payload.
/// </summary>
public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

/// <summary>
/// Represents the onboarding / setup request payload.
/// </summary>
public record SetupRequest(
    [Required] string StoreName,
    [Required] string Currency,
    [Required] string AdminUsername,
    [Required] string AdminPassword,
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber,
    string? Position,
    Staff.Frequency PayFrequency,
    float PayRate
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

        if (!AuthenticationService.VerifyPassword(request.Password, staff.Password ?? string.Empty))
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

    /// <summary>
    /// Initializes system settings, creates default roles, and registers the initial administrator profile.
    /// </summary>
    /// <param name="request">The onboarding configurations payload.</param>
    /// <returns>An HTTP result indicating the status of the setup.</returns>
    public static async Task<IResult> Setup([FromBody] SetupRequest request)
    {
        using AppDbContext context = new();
        if (await context.User.AnyAsync())
        {
            return Results.Json(new { error = "System has already been initialized." }, statusCode: StatusCodes.Status403Forbidden);
        }

        // 1. Save store settings
        StoreSettings settings = SettingsService.LoadSettings();
        settings.StoreName = request.StoreName;
        settings.Currency = request.Currency;
        settings.StoreOpeningDate = DateTime.UtcNow;
        SettingsService.SaveSettings(settings);

        // 2. Hash password and generate entities
        string hashedPass = AuthenticationService.HashPassword(request.AdminPassword);

        // Seed default Admin and Cashier Roles
        ulong adminRoleId = Utils.GenerateEntityId();
        var adminPerms = new Dictionary<string, int>
        {
            { "AccessAdminSettings", 1 },
            { "AccessPOS", 1 },
            { "AccessInventory", 1 }
        };
        Role adminRole = new(adminRoleId, "Admin", System.Text.Json.JsonSerializer.Serialize(adminPerms));
        context.Role.Add(adminRole);

        ulong cashierRoleId = Utils.GenerateEntityId();
        var cashierPerms = new Dictionary<string, int>
        {
            { "AccessAdminSettings", -1 },
            { "AccessPOS", 1 },
            { "AccessInventory", 0 }
        };
        Role cashierRole = new(cashierRoleId, "Cashier", System.Text.Json.JsonSerializer.Serialize(cashierPerms));
        context.Role.Add(cashierRole);

        Staff admin = new(
            id: Utils.GenerateEntityId(),
            username: request.AdminUsername,
            payFrequency: request.PayFrequency,
            payRate: request.PayRate,
            password: hashedPass,
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            phoneNumber: request.PhoneNumber,
            position: request.Position ?? "Administrator"
        );
        admin.Roles.Add(adminRole);

        context.Staff.Add(admin);
        await context.SaveChangesAsync();

        Log.Me(() => "System initialized and onboarding setup completed successfully.");

        return Results.Ok(new
        {
            success = true,
            id = admin.Id,
            username = admin.Username,
            position = admin.Position
        });
    }
}