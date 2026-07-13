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
/// Exposes endpoints for managing customer profiles.
/// </summary>
public static class CustomerController
{
    public static async Task<IResult> CreateCustomer(
        [FromBody] CreateCustomerRequest request)
    {
        try
        {
            using AppDbContext context = new();
            Customer customer = new(
                id: Utils.GenerateEntityId(),
                firstName: request.FirstName,
                lastName: request.LastName,
                phoneNumber: request.PhoneNumber,
                email: request.Email
            )
            {
                RfidCardId = request.RfidCardId,
                Permissions = request.Permissions ?? new(),
                PermissionsAfterExpiry = request.PermissionsAfterExpiry ?? new(),
                ExpirationDate = request.ExpirationDate,
                AccessType = AccessCardType.Member
            };

            context.Customer.Add(customer);
            await context.SaveChangesAsync();

            return Results.Ok(customer);
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to create customer: {ex.Message}");
        }
    }

    public static async Task<IResult> GetCustomer(
        [FromBody] GetCustomerRequest request)
    {
        try
        {
            using AppDbContext context = new();
            Customer? customer = await context.Customer
                .Include(c => c.Roles)
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (customer is null)
            {
                object errorObj = new { error = $"Customer with ID {request.Id} not found." };
                return Results.NotFound(errorObj);
            }

            return Results.Ok(customer);
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to retrieve customer: {ex.Message}");
        }
    }

    public static async Task<IResult> FindCustomers(
        [FromBody] FindCustomerRequest request)
    {
        try
        {
            using AppDbContext context = new();
            IQueryable<Customer> query = context.Customer.Include(c => c.Roles);

            if (request is not null)
            {
                if (request.Id is not null)
                {
                    string idStr = request.Id.Value.ToString();
                    query = query.Where(c => c.Id.ToString().Contains(idStr));
                }
                if (!string.IsNullOrEmpty(request.FirstName))
                    query = query.Where(c => c.FirstName != null && c.FirstName.Contains(request.FirstName));
                if (!string.IsNullOrEmpty(request.LastName))
                    query = query.Where(c => c.LastName != null && c.LastName.Contains(request.LastName));
                if (!string.IsNullOrEmpty(request.PhoneNumber))
                    query = query.Where(c => c.PhoneNumber != null && c.PhoneNumber.Contains(request.PhoneNumber));
                if (!string.IsNullOrEmpty(request.Email))
                    query = query.Where(c => c.Email != null && c.Email.Contains(request.Email));
                if (!string.IsNullOrEmpty(request.RfidCardId))
                    query = query.Where(c => c.RfidCardId != null && c.RfidCardId.Contains(request.RfidCardId));
            }

            query = Utils.ApplySortingAndPagination(
                query, request?.Limit, request?.Offset, request?.SortBy, request?.SortOrder
            );

            List<Customer> results = await query.ToListAsync();
            return Results.Ok(results);
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to find customers: {ex.Message}");
        }
    }

    public static async Task<IResult> UpdateCustomer(
        [FromBody] UpdateCustomerRequest request)
    {
        try
        {
            using AppDbContext context = new();
            Customer? customer = await context.Customer
                .Include(c => c.Roles)
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (customer is null)
            {
                object errorObj = new { error = $"Customer with ID {request.Id} not found." };
                return Results.NotFound(errorObj);
            }

            customer.FirstName = request.FirstName ?? customer.FirstName;
            customer.LastName = request.LastName ?? customer.LastName;
            customer.PhoneNumber = request.PhoneNumber ?? customer.PhoneNumber;
            customer.Email = request.Email ?? customer.Email;
            customer.RfidCardId = request.RfidCardId ?? customer.RfidCardId;
            customer.Permissions = request.Permissions ?? customer.Permissions;
            customer.PermissionsAfterExpiry = request.PermissionsAfterExpiry ?? customer.PermissionsAfterExpiry;
            customer.ExpirationDate = request.ExpirationDate ?? customer.ExpirationDate;

            await context.SaveChangesAsync();
            return Results.Ok(new { success = true });
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to update customer: {ex.Message}");
        }
    }

    public static async Task<IResult> DeleteCustomer(
        [FromBody] DeleteCustomerRequest request)
    {
        try
        {
            using AppDbContext context = new();
            Customer? customer = await context.Customer.FindAsync(request.Id);
            if (customer is null)
            {
                object errorObj = new { error = $"Customer with ID {request.Id} not found." };
                return Results.NotFound(errorObj);
            }

            context.Customer.Remove(customer);
            await context.SaveChangesAsync();
            return Results.Ok(new { success = true });
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to delete customer: {ex.Message}");
        }
    }
}
