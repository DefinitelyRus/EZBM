using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.DesktopClient.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EZBM.DesktopClient.Pages;

public class StaffModel : PageModel
{
    #region Properties
    public List<Staff> StaffList { get; set; } = new();
    public Staff? EditingStaff { get; set; }
    public List<Role> RolesList { get; set; } = new();
    public string ActiveTab { get; set; } = "directory";

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }
    #endregion

    #region Handlers

    /// <summary>
    /// Restricts page access to authenticated staff members.
    /// </summary>
    public override async Task OnPageHandlerExecutionAsync(
        Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context,
        Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutionDelegate next
    )
    {
        Staff? activeStaff = await StateHelper.GetActiveStaffAsync(HttpContext);

        if (activeStaff is null)
        {
            context.Result = RedirectToPage("/Login");
            return;
        }

        await next();
    }
    public async Task OnGetAsync(string? action, ulong? id, string? tab)
    {
        if (!string.IsNullOrEmpty(tab))
        {
            ActiveTab = tab;
        }

        using AppDbContext context = new();
        
        // Load Staff List with roles
        StaffList = await context.Staff.Include(s => s.Roles).ToListAsync();
        
        // Load Roles List
        RolesList = await context.Role.ToListAsync();

        if (action == "edit" && id.HasValue)
        {
            EditingStaff = await context.Staff.Include(s => s.Roles).FirstOrDefaultAsync(s => s.Id == id.Value);
        }
    }

    public async Task<IActionResult> OnPostCreateAsync(
        string username,
        string? password,
        string? firstName,
        string? lastName,
        string? email,
        string? phoneNumber,
        string? position,
        Staff.Frequency payFrequency,
        float payRate,
        float? commissionRate,
        List<ulong>? roleIds,
        string? rfidCardId
    )
    {
        if (payRate < 0f)
        {
            ErrorMessage = "Pay rate cannot be negative.";
            return RedirectToPage("/Staff", new { tab = "directory" });
        }

        float? commissionRateMultiplier = commissionRate.HasValue ? commissionRate.Value / 100f : null;

        CreateStaffRequest request = new(
            Username: username,
            Password: password,
            FirstName: firstName,
            LastName: lastName,
            Email: email,
            PhoneNumber: phoneNumber,
            Position: position,
            PayFrequency: payFrequency,
            PayRate: payRate,
            CommissionRate: commissionRateMultiplier,
            RfidCardId: rfidCardId
        );

        Utils.RequestResult result = await StaffService.CreateStaffAsync(request);

        if (result.Type == Utils.Result.Success)
        {
            // Now, bind roles if any
            using AppDbContext context = new();
            var staff = await context.Staff.Include(s => s.Roles).FirstOrDefaultAsync(s => s.Username == username);
            if (staff != null && roleIds != null && roleIds.Count > 0)
            {
                var selectedRoles = await context.Role.Where(r => roleIds.Contains(r.Id)).ToListAsync();
                staff.Roles.AddRange(selectedRoles);
                await context.SaveChangesAsync();
            }
            SuccessMessage = $"Staff member '{username}' registered successfully.";
        }
        else
        {
            ErrorMessage = result.Message;
        }

        return RedirectToPage("/Staff", new { tab = "directory" });
    }

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
        float payRate,
        float? commissionRate,
        List<ulong>? roleIds,
        string? rfidCardId
    )
    {
        if (payRate < 0f)
        {
            ErrorMessage = "Pay rate cannot be negative.";
            return RedirectToPage("/Staff", new { tab = "directory" });
        }

        float? commissionRateMultiplier = commissionRate.HasValue ? commissionRate.Value / 100f : null;

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
            PayRate: payRate,
            CommissionRate: commissionRateMultiplier,
            RfidCardId: rfidCardId
        );

        Utils.RequestResult result = await StaffService.UpdateStaffAsync(request);

        if (result.Type == Utils.Result.Success)
        {
            // Update roles
            using AppDbContext context = new();
            var staff = await context.Staff.Include(s => s.Roles).FirstOrDefaultAsync(s => s.Id == id);
            if (staff != null)
            {
                staff.Roles.Clear();
                if (roleIds != null && roleIds.Count > 0)
                {
                    var selectedRoles = await context.Role.Where(r => roleIds.Contains(r.Id)).ToListAsync();
                    staff.Roles.AddRange(selectedRoles);
                }
                await context.SaveChangesAsync();
            }
            SuccessMessage = $"Staff member ID {id} updated successfully.";
        }
        else
        {
            ErrorMessage = result.Message;
        }

        return RedirectToPage("/Staff", new { tab = "directory" });
    }

    public async Task<IActionResult> OnPostDeleteAsync(ulong id)
    {
        DeleteStaffRequest request = new(Id: id);
        Utils.RequestResult result = await StaffService.DeleteStaffAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = $"Staff profile ID {id} deleted successfully.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Staff", new { tab = "directory" });
    }

    public async Task<IActionResult> OnPostCreateRoleAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            ErrorMessage = "Role name cannot be empty.";
            return RedirectToPage("/Staff", new { tab = "roles" });
        }

        try
        {
            using AppDbContext context = new();
            Role role = new(
                id: Utils.GenerateEntityId(),
                name: roleName,
                permissionsJson: "{}"
            );
            context.Role.Add(role);
            await context.SaveChangesAsync();
            SuccessMessage = $"Role '{roleName}' created successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create role: {ex.Message}";
        }
        return RedirectToPage("/Staff", new { tab = "roles" });
    }

    public async Task<IActionResult> OnPostCreateRoleJsonAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return new JsonResult(new { success = false, message = "Role name cannot be empty." });
        }
        try
        {
            using AppDbContext context = new();
            var existing = await context.Role.FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());
            if (existing != null)
            {
                return new JsonResult(new { success = true, roleId = existing.Id.ToString(), roleName = existing.Name });
            }

            Role role = new(
                id: Utils.GenerateEntityId(),
                name: roleName,
                permissionsJson: "{}"
            );
            context.Role.Add(role);
            await context.SaveChangesAsync();
            return new JsonResult(new { success = true, roleId = role.Id.ToString(), roleName = role.Name });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }


    public async Task<IActionResult> OnPostSaveRolePermissionsAsync()
    {
        try
        {
            using AppDbContext context = new();
            var roles = await context.Role.ToListAsync();
            var permissionKeys = new List<string>
            {
                "Checkout", "ApplyDiscounts", "Refunds",
                "ViewInventory", "ModifyInventory", "ViewSensitiveInventoryCost",
                "ViewStaffInfo", "ManageStaff", "ManageAccess", "ManagePayroll",
                "ViewLogs", "DeleteLogs", "ManageSettings"
            };

            foreach (var role in roles)
            {
                var permissions = new Dictionary<string, int>();
                foreach (var key in permissionKeys)
                {
                    string formFieldName = $"perm_{role.Id}_{key}";
                    if (Request.Form.TryGetValue(formFieldName, out var valStr) && int.TryParse(valStr, out int val))
                    {
                        permissions[key] = val;
                    }
                    else
                    {
                        permissions[key] = 0;
                    }
                }
                role.PermissionsJson = JsonSerializer.Serialize(permissions);
            }

            await context.SaveChangesAsync();
            SuccessMessage = "Roles permission matrix updated successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save permissions: {ex.Message}";
        }
        return RedirectToPage("/Staff", new { tab = "roles" });
    }

    public async Task<IActionResult> OnPostDeleteRoleAsync(ulong id)
    {
        try
        {
            using AppDbContext context = new();
            var role = await context.Role.FindAsync(id);
            if (role != null)
            {
                context.Role.Remove(role);
                await context.SaveChangesAsync();
                SuccessMessage = "Role deleted successfully.";
            }
            else
            {
                ErrorMessage = "Role not found.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete role: {ex.Message}";
        }
        return RedirectToPage("/Staff", new { tab = "roles" });
    }
    #endregion
}
