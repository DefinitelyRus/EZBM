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
/// Exposes endpoints for managing staff payroll adjustments (bonuses and salary advances).
/// </summary>
public static class StaffAdjustmentController
{
    public static async Task<IResult> CreateStaffAdjustment(
        [FromBody] CreateStaffAdjustmentRequest request)
    {
        try
        {
            using AppDbContext context = new();

            // Verify staff member exists first
            bool staffExists = await context.Staff.AnyAsync(s => s.Id == request.StaffId);
            if (!staffExists)
            {
                object errorObj = new { error = $"Staff member with ID {request.StaffId} not found." };
                return Results.BadRequest(errorObj);
            }

            StaffAdjustment adjustment = new(
                id: Utils.GenerateEntityId(),
                staffId: request.StaffId,
                adjustmentType: request.AdjustmentType,
                amount: request.Amount,
                deductFromCurrentPayroll: request.DeductFromCurrentPayroll,
                isPaid: request.IsPaid,
                timestamp: request.Timestamp ?? DateTime.UtcNow,
                notes: request.Notes
            );

            context.StaffAdjustment.Add(adjustment);
            await context.SaveChangesAsync();

            return Results.Ok(adjustment);
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to create staff adjustment: {ex.Message}");
        }
    }

    public static async Task<IResult> GetStaffAdjustment(
        [FromBody] GetStaffAdjustmentRequest request)
    {
        try
        {
            using AppDbContext context = new();
            StaffAdjustment? adjustment = await context.StaffAdjustment.FindAsync(request.Id);

            if (adjustment is null)
            {
                object errorObj = new { error = $"Staff adjustment with ID {request.Id} not found." };
                return Results.NotFound(errorObj);
            }

            return Results.Ok(adjustment);
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to retrieve staff adjustment: {ex.Message}");
        }
    }

    public static async Task<IResult> FindStaffAdjustments(
        [FromBody] FindStaffAdjustmentRequest request)
    {
        try
        {
            using AppDbContext context = new();
            IQueryable<StaffAdjustment> query = context.StaffAdjustment;

            if (request is not null)
            {
                if (request.Id is not null)
                {
                    string idStr = request.Id.Value.ToString();
                    query = query.Where(a => a.Id.ToString().Contains(idStr));
                }
                if (request.StaffId is not null)
                    query = query.Where(a => a.StaffId == request.StaffId);
                if (!string.IsNullOrEmpty(request.AdjustmentType))
                    query = query.Where(a => a.AdjustmentType.Contains(request.AdjustmentType));
                if (request.DeductFromCurrentPayroll is not null)
                    query = query.Where(a => a.DeductFromCurrentPayroll == request.DeductFromCurrentPayroll);
                if (request.IsPaid is not null)
                    query = query.Where(a => a.IsPaid == request.IsPaid);
            }

            query = Utils.ApplySortingAndPagination(
                query, request?.Limit, request?.Offset, request?.SortBy, request?.SortOrder
            );

            List<StaffAdjustment> results = await query.ToListAsync();
            return Results.Ok(results);
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to find staff adjustments: {ex.Message}");
        }
    }

    public static async Task<IResult> DeleteStaffAdjustment(
        [FromBody] DeleteStaffAdjustmentRequest request)
    {
        try
        {
            using AppDbContext context = new();
            StaffAdjustment? adjustment = await context.StaffAdjustment.FindAsync(request.Id);
            if (adjustment is null)
            {
                object errorObj = new { error = $"Staff adjustment with ID {request.Id} not found." };
                return Results.NotFound(errorObj);
            }

            context.StaffAdjustment.Remove(adjustment);
            await context.SaveChangesAsync();
            return Results.Ok(new { success = true });
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to delete staff adjustment: {ex.Message}");
        }
    }
}
