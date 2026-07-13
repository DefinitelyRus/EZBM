using EZBM.Core.Entities;
using EZBM.Core.Data;
using EZBM.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using EZBM.DesktopHost.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints for managing system permissions roles.
/// </summary>
public static class RoleController
{
    public static async Task<IResult> CreateRole(
        [FromBody] CreateRoleRequest request)
    {
        try
        {
            using AppDbContext context = new();
            Role role = new(
                id: Utils.GenerateEntityId(),
                name: request.Name,
                permissionsJson: request.PermissionsJson ?? "{}"
            );

            context.Role.Add(role);
            await context.SaveChangesAsync();

            return Results.Ok(role);
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to create role: {ex.Message}");
        }
    }

    public static async Task<IResult> GetRole(
        [FromBody] GetRoleRequest request)
    {
        try
        {
            using AppDbContext context = new();
            Role? role = await context.Role.FindAsync(request.Id);

            if (role is null)
            {
                object errorObj = new { error = $"Role with ID {request.Id} not found." };
                return Results.NotFound(errorObj);
            }

            return Results.Ok(role);
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to retrieve role: {ex.Message}");
        }
    }

    public static async Task<IResult> FindRoles(
        [FromBody] FindRoleRequest request)
    {
        try
        {
            using AppDbContext context = new();
            IQueryable<Role> query = context.Role;

            if (request is not null)
            {
                if (request.Id is not null)
                {
                    string idStr = request.Id.Value.ToString();
                    query = query.Where(r => r.Id.ToString().Contains(idStr));
                }
                if (!string.IsNullOrEmpty(request.Name))
                    query = query.Where(r => r.Name.Contains(request.Name));
            }

            query = Utils.ApplySortingAndPagination(
                query, request?.Limit, request?.Offset, request?.SortBy, request?.SortOrder
            );

            List<Role> results = await query.ToListAsync();
            return Results.Ok(results);
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to find roles: {ex.Message}");
        }
    }

    public static async Task<IResult> UpdateRole(
        [FromBody] UpdateRoleRequest request)
    {
        try
        {
            using AppDbContext context = new();
            Role? role = await context.Role.FindAsync(request.Id);

            if (role is null)
            {
                object errorObj = new { error = $"Role with ID {request.Id} not found." };
                return Results.NotFound(errorObj);
            }

            role.Name = request.Name ?? role.Name;
            role.PermissionsJson = request.PermissionsJson ?? role.PermissionsJson;

            await context.SaveChangesAsync();
            return Results.Ok(new { success = true });
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to update role: {ex.Message}");
        }
    }

    public static async Task<IResult> DeleteRole(
        [FromBody] DeleteRoleRequest request)
    {
        try
        {
            using AppDbContext context = new();
            Role? role = await context.Role.FindAsync(request.Id);
            if (role is null)
            {
                object errorObj = new { error = $"Role with ID {request.Id} not found." };
                return Results.NotFound(errorObj);
            }

            context.Role.Remove(role);
            await context.SaveChangesAsync();
            return Results.Ok(new { success = true });
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to delete role: {ex.Message}");
        }
    }
}
