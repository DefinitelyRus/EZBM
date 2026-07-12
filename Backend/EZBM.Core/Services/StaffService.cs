using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Tools;
using Microsoft.EntityFrameworkCore;

namespace EZBM.Core.Services;

/// <summary>
/// Provides services for managing staff, attendance, and payroll records.
/// </summary>
public static class StaffService
{
    #region Staff Requests

    /// <summary>
    /// Gets a staff profile by username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>The Staff entity if found, otherwise null.</returns>
    public static async Task<Staff?> GetStaffByUsernameAsync(string username)
    {
        try
        {
            using AppDbContext context = new();
            Staff? staff = await context.Staff.FirstOrDefaultAsync(
                s => s.Username == username
            );
            return staff;
        }

        catch (Exception ex)
        {
            Log.Err($"Error when getting staff by username: {ex.Message}");
            return null;
        }
    }

        /// <summary>
    /// Creates a new staff member profile.
    /// </summary>
    /// <param name="request">The request parameters containing staff profile details.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> CreateStaffAsync(
        CreateStaffRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();

            bool exists = await context.Staff.AnyAsync(
                s => s.Username == request.Username
            );

            if (exists)
            {
                message = $"Staff username '{request.Username}' already exists.";
                Log.Me(message);
                Utils.RequestResult conflictResult = new(
                    Utils.Result.Failed_InvalidQuery, message
                );

                return conflictResult;
            }

            Staff staff = new(
                id: Utils.GenerateEntityId(),
                username: request.Username,
                payFrequency: request.PayFrequency,
                payRate: request.PayRate,
                password: request.Password != null ? AuthenticationService.HashPassword(request.Password) : null,
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                phoneNumber: request.PhoneNumber,
                position: request.Position,
                commissionRate: request.CommissionRate
            )
            {
                RfidCardId = request.RfidCardId,
                Permissions = request.Permissions ?? new(),
                PermissionsAfterExpiry = request.PermissionsAfterExpiry ?? new(),
                ExpirationDate = request.ExpirationDate
            };

            if (request.RoleIds is not null && request.RoleIds.Count > 0)
            {
                staff.Roles = await context.Role.Where(r => request.RoleIds.Contains(r.Id)).ToListAsync();
            }

            context.Staff.Add(staff);
            await context.SaveChangesAsync();

            message = $"Staff '{staff.Username}' created successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when creating staff: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Retrieves a specific staff member by identifier.
    /// </summary>
    /// <param name="request">The request containing the staff ID.</param>
    /// <returns>A RequestResult containing the Staff entity.</returns>
    public static async Task<Utils.RequestResult<Staff>> GetStaffAsync(
        GetStaffRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Staff? staff = await context.Staff.FindAsync(request.Id);

            if (staff is null)
            {
                message = $"Staff with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult<Staff> failResult = new(
                    Utils.Result.Failed_NoResults, message, null
                );

                return failResult;
            }

            message = $"Staff '{staff.Username}' found successfully.";
            Log.Me(message);
            Utils.RequestResult<Staff> successResult = new(
                Utils.Result.Success, message, staff
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when getting staff with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult<Staff> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, null
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Finds staff profiles matching the specified query filters.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>A RequestResult containing the list of matching Staff members.</returns>
    public static async Task<Utils.RequestResult<List<Staff>>> FindStaffAsync(
        FindStaffRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            IQueryable<Staff> query = context.Staff;

            if (request is not null)
            {
                if (request.Id is not null)
                {
                    string requestIdStr = request.Id.Value.ToString();
                    query = query.Where(
                        s => s.Id.ToString().Contains(requestIdStr)
                    );
                }

                if (!string.IsNullOrEmpty(request.Username))
                    query = query.Where(
                        s => s.Username.Contains(request.Username)
                    );

                if (!string.IsNullOrEmpty(request.FirstName))
                    query = query.Where(
                        s => s.FirstName != null &&
                        s.FirstName.Contains(request.FirstName)
                    );

                if (!string.IsNullOrEmpty(request.LastName))
                    query = query.Where(
                        s => s.LastName != null &&
                        s.LastName.Contains(request.LastName)
                    );

                if (!string.IsNullOrEmpty(request.Position))
                    query = query.Where(
                        s => s.Position != null &&
                        s.Position.Contains(request.Position)
                    );

                if (request.PayFrequency is not null)
                    query = query.Where(
                        s => s.PayFrequency == request.PayFrequency
                    );
            }

            query = Utils.ApplySortingAndPagination(query, request?.Limit, request?.Offset, request?.SortBy, request?.SortOrder);

            List<Staff> results = await query.ToListAsync();

            if (results.Count == 0)
            {
                message = "No staff records found matching the query.";
                Log.Me(message);
                Utils.RequestResult<List<Staff>> noResults = new(
                    Utils.Result.Success_NoResults, message, []
                );

                return noResults;
            }

            message = $"Found {results.Count} staff record(s) matching query.";
            Log.Me(message);
            Utils.RequestResult<List<Staff>> successResult = new(
                Utils.Result.Success, message, results
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when finding staff: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult<List<Staff>> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, []
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Updates an existing staff member's profile.
    /// </summary>
    /// <param name="request">The request parameters containing update fields and ID.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> UpdateStaffAsync(
        UpdateStaffRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Staff? staff = await context.Staff.Include(s => s.Roles).FirstOrDefaultAsync(s => s.Id == request.Id);

            if (staff is null)
            {
                message = $"Staff with ID {request.Id} not found.";
                Log.Me(message);
                Utils.RequestResult failResult = new(
                    Utils.Result.Failed_NoResults, message
                );

                return failResult;
            }

            staff.Username = request.Username ?? staff.Username;
            staff.Password = request.Password != null ? AuthenticationService.HashPassword(request.Password) : staff.Password;
            staff.FirstName = request.FirstName ?? staff.FirstName;
            staff.LastName = request.LastName ?? staff.LastName;
            staff.Email = request.Email ?? staff.Email;
            staff.PhoneNumber = request.PhoneNumber ?? staff.PhoneNumber;
            staff.Position = request.Position ?? staff.Position;
            staff.PayFrequency = request.PayFrequency ?? staff.PayFrequency;
            staff.PayRate = request.PayRate ?? staff.PayRate;
            staff.CommissionRate = request.CommissionRate ?? staff.CommissionRate;
            staff.RfidCardId = request.RfidCardId ?? staff.RfidCardId;
            staff.Permissions = request.Permissions ?? staff.Permissions;
            staff.PermissionsAfterExpiry = request.PermissionsAfterExpiry ?? staff.PermissionsAfterExpiry;
            staff.ExpirationDate = request.ExpirationDate ?? staff.ExpirationDate;

            if (request.RoleIds is not null)
            {
                staff.Roles = await context.Role.Where(r => request.RoleIds.Contains(r.Id)).ToListAsync();
            }

            await context.SaveChangesAsync();

            message = $"Staff member '{staff.Username}' updated successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when updating staff: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Deletes a specific staff profile.
    /// </summary>
    /// <param name="request">The request containing the staff ID to delete.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> DeleteStaffAsync(
        DeleteStaffRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Staff? staff = await context.Staff.FindAsync(request.Id);

            if (staff is null)
            {
                message = $"Staff with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult noResults = new(
                    Utils.Result.Failed_NoResults, message
                );

                return noResults;
            }

            context.Staff.Remove(staff);
            await context.SaveChangesAsync();

            message = $"Staff with ID {staff.Id} deleted successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when deleting staff with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    #endregion

    #region Attendance Requests

    /// <summary>
    /// Logs a clock-in or clock-out event for a staff member.
    /// </summary>
    /// <param name="request">The request parameters containing staff ID and action type.</param>
    /// <returns>A RequestResult containing the timestamp of the logged event.</returns>
    public static async Task<Utils.RequestResult<DateTime>> LogAttendanceAsync(
        LogAttendanceRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Staff? staff = await context.Staff.FindAsync(request.StaffId);

            if (staff is null)
            {
                message = $"Staff with ID {request.StaffId} not found.";
                Log.Me(message);
                Utils.RequestResult<DateTime> noResults = new(
                    Utils.Result.Failed_NoResults, message, DateTime.MinValue
                );

                return noResults;
            }

            DateTime serverTime = DateTime.UtcNow;

            if (request.ActionType.Equals("In", StringComparison.OrdinalIgnoreCase))
            {
                Attendance? activeAttendance = await context.Attendance
                    .FirstOrDefaultAsync(
                        a => a.Staff.Id == request.StaffId && a.TimeOut == null
                    );

                if (activeAttendance is not null)
                {
                    message = $"Staff member '{staff.Username}' is already clocked in.";
                    Log.Me(message);
                    Utils.RequestResult<DateTime> warnResult = new(
                        Utils.Result.Success_Warning, message, activeAttendance.TimeIn
                    );

                    return warnResult;
                }

                Attendance attendance = new(
                    id: Utils.GenerateEntityId(),
                    staff: staff,
                    timeIn: serverTime,
                    timeOut: null
                );

                context.Attendance.Add(attendance);
                await context.SaveChangesAsync();

                message = $"Staff member '{staff.Username}' clocked in successfully at {serverTime}.";
                Log.Me(message);
                Utils.RequestResult<DateTime> successResult = new(
                    Utils.Result.Success, message, serverTime
                );

                return successResult;
            }
            else if (request.ActionType.Equals("Out", StringComparison.OrdinalIgnoreCase))
            {
                Attendance? activeAttendance = await context.Attendance
                    .Where(
                        a => a.Staff.Id == request.StaffId && a.TimeOut == null
                    )
                    .OrderByDescending(
                        a => a.TimeIn
                    )
                    .FirstOrDefaultAsync();

                if (activeAttendance is null)
                {
                    Attendance fallbackAttendance = new(
                        id: Utils.GenerateEntityId(),
                        staff: staff,
                        timeIn: serverTime,
                        timeOut: serverTime
                    );

                    context.Attendance.Add(fallbackAttendance);
                    await context.SaveChangesAsync();

                    message = $"No active clock-in found for '{staff.Username}'. Logged new completed session at {serverTime}.";
                    Log.Me(message);
                    Utils.RequestResult<DateTime> warnResult = new(
                        Utils.Result.Success_Warning, message, serverTime
                    );

                    return warnResult;
                }

                activeAttendance.TimeOut = serverTime;
                await context.SaveChangesAsync();

                message = $"Staff member '{staff.Username}' clocked out successfully at {serverTime}.";
                Log.Me(message);
                Utils.RequestResult<DateTime> successResult = new(
                    Utils.Result.Success, message, serverTime
                );

                return successResult;
            }
            else
            {
                message = $"Invalid action type '{request.ActionType}'. Must be 'In' or 'Out'.";
                Log.Me(message);
                Utils.RequestResult<DateTime> invalidResult = new(
                    Utils.Result.Failed_InvalidQuery, message, DateTime.MinValue
                );

                return invalidResult;
            }
        }

        catch (Exception ex)
        {
            message = $"Error when logging attendance: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult<DateTime> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, DateTime.MinValue
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Creates a manual attendance entry.
    /// </summary>
    /// <param name="request">The request containing attendance details.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> CreateAttendanceAsync(
        CreateAttendanceRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Staff? staff = await context.Staff.FindAsync(request.StaffId);

            if (staff is null)
            {
                message = $"Staff with ID {request.StaffId} not found.";
                Log.Me(message);
                Utils.RequestResult noStaffResult = new(
                    Utils.Result.Failed_NoResults, message
                );

                return noStaffResult;
            }

            Attendance attendance = new(
                id: Utils.GenerateEntityId(),
                staff: staff,
                timeIn: request.TimeIn,
                timeOut: request.TimeOut
            );

            context.Attendance.Add(attendance);
            await context.SaveChangesAsync();

            message = $"Attendance record created successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when creating attendance record: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Retrieves a specific attendance record.
    /// </summary>
    /// <param name="request">The request containing the attendance ID.</param>
    /// <returns>A RequestResult containing the Attendance entity.</returns>
    public static async Task<Utils.RequestResult<Attendance>> GetAttendanceAsync(
        GetAttendanceRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Attendance? attendance = await context.Attendance
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(
                    a => a.Id == request.Id
                );

            if (attendance is null)
            {
                message = $"Attendance record with ID {request.Id} not found.";
                Log.Me(message);
                Utils.RequestResult<Attendance> failResult = new(
                    Utils.Result.Failed_NoResults, message, null
                );

                return failResult;
            }

            message = $"Attendance record with ID {request.Id} found successfully.";
            Log.Me(message);
            Utils.RequestResult<Attendance> successResult = new(
                Utils.Result.Success, message, attendance
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when getting attendance record: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult<Attendance> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, null
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Finds attendance records matching the specified query filters.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>A RequestResult containing the list of matching Attendance records.</returns>
    public static async Task<Utils.RequestResult<List<Attendance>>> FindAttendanceAsync(
        FindAttendanceRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            IQueryable<Attendance> query = context.Attendance.Include(a => a.Staff);

            if (request is not null)
            {
                if (request.Id is not null)
                {
                    string requestIdStr = request.Id.Value.ToString();
                    query = query.Where(
                        a => a.Id.ToString().Contains(requestIdStr)
                    );
                }

                if (request.StaffId is not null)
                    query = query.Where(
                        a => a.Staff.Id == request.StaffId
                    );

                if (request.MinTimeIn is not null)
                    query = query.Where(
                        a => a.TimeIn >= request.MinTimeIn
                    );

                if (request.MaxTimeIn is not null)
                    query = query.Where(
                        a => a.TimeIn <= request.MaxTimeIn
                    );

                if (request.MinTimeOut is not null)
                    query = query.Where(
                        a => a.TimeOut != null && a.TimeOut >= request.MinTimeOut
                    );

                if (request.MaxTimeOut is not null)
                    query = query.Where(
                        a => a.TimeOut != null && a.TimeOut <= request.MaxTimeOut
                    );
            }

            query = Utils.ApplySortingAndPagination(query, request?.Limit, request?.Offset, request?.SortBy, request?.SortOrder);

            List<Attendance> results = await query.ToListAsync();

            if (results.Count == 0)
            {
                message = "No attendance records found matching query.";
                Log.Me(message);
                Utils.RequestResult<List<Attendance>> noResults = new(
                    Utils.Result.Success_NoResults, message, []
                );
                return noResults;
            }

            message = $"Found {results.Count} attendance record(s) matching query.";
            Log.Me(message);
            Utils.RequestResult<List<Attendance>> successResult = new(
                Utils.Result.Success, message, results
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when finding attendance records: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult<List<Attendance>> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, []
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Updates a specific attendance record.
    /// </summary>
    /// <param name="request">The request parameters containing update fields and ID.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> UpdateAttendanceAsync(
        UpdateAttendanceRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Attendance? attendance = await context.Attendance.FindAsync(request.Id);

            if (attendance is null)
            {
                message = $"Attendance record with ID {request.Id} not found.";
                Log.Me(message);
                Utils.RequestResult failResult = new(
                    Utils.Result.Failed_NoResults, message
                );

                return failResult;
            }

            if (request.StaffId is not null)
            {
                Staff? staff = await context.Staff.FindAsync(request.StaffId.Value);
                if (staff is null)
                {
                    message = $"Staff with ID {request.StaffId} not found.";
                    Log.Me(message);
                    Utils.RequestResult noStaffResult = new(
                        Utils.Result.Failed_NoResults, message
                    );

                    return noStaffResult;
                }
                attendance.Staff = staff;
            }

            attendance.TimeIn = request.TimeIn ?? attendance.TimeIn;
            attendance.TimeOut = request.TimeOut ?? attendance.TimeOut;

            await context.SaveChangesAsync();

            message = $"Attendance record with ID {attendance.Id} updated.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when updating attendance: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Deletes a specific attendance record.
    /// </summary>
    /// <param name="request">The request containing the attendance ID to delete.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> DeleteAttendanceAsync(
        DeleteAttendanceRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Attendance? attendance = await context.Attendance.FindAsync(request.Id);

            if (attendance is null)
            {
                message = $"Attendance record with ID {request.Id} not found.";
                Log.Me(message);
                Utils.RequestResult noResults = new(
                    Utils.Result.Failed_NoResults, message
                );

                return noResults;
            }

            context.Attendance.Remove(attendance);
            await context.SaveChangesAsync();

            message = $"Attendance record with ID {attendance.Id} deleted.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when deleting attendance: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    #endregion

    #region Payroll Requests

    /// <summary>
    /// Creates a new payroll record.
    /// </summary>
    /// <param name="request">The request containing payroll details.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> CreatePayrollAsync(
        CreatePayrollRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Staff? staff = await context.Staff.FindAsync(request.StaffId);

            if (staff is null)
            {
                message = $"Staff with ID {request.StaffId} not found.";
                Log.Me(message);
                Utils.RequestResult noStaffResult = new(
                    Utils.Result.Failed_NoResults, message
                );

                return noStaffResult;
            }

            Payroll payroll = new(
                id: Utils.GenerateEntityId(),
                staff: staff,
                periodStart: request.PeriodStart,
                periodEnd: request.PeriodEnd,
                totalHours: request.TotalHours,
                grossAmount: request.GrossAmount,
                modifiers: request.Modifiers,
                netAmount: request.NetAmount,
                payDate: request.PayDate,
                notes: request.Notes
            );

            // Mark unpaid adjustments as paid
            var unpaidAdjustments = await context.StaffAdjustment
                .Where(sa => sa.StaffId == request.StaffId && sa.IsPaid == false && sa.Timestamp <= request.PeriodEnd)
                .ToListAsync();
            foreach (var sa in unpaidAdjustments)
            {
                sa.IsPaid = true;
            }

            context.Payroll.Add(payroll);
            await context.SaveChangesAsync();

            message = $"Payroll record created successfully with ID {payroll.Id}.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when creating payroll: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Retrieves a specific payroll record.
    /// </summary>
    /// <param name="request">The request containing the payroll ID.</param>
    /// <returns>A RequestResult containing the Payroll entity.</returns>
    public static async Task<Utils.RequestResult<Payroll>> GetPayrollAsync(
        GetPayrollRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Payroll? payroll = await context.Payroll
                .Include(p => p.Staff)
                .FirstOrDefaultAsync(
                    p => p.Id == request.Id
                );

            if (payroll is null)
            {
                message = $"Payroll record with ID {request.Id} not found.";
                Log.Me(message);
                Utils.RequestResult<Payroll> failResult = new(
                    Utils.Result.Failed_NoResults, message, null
                );

                return failResult;
            }

            message = $"Payroll record with ID {request.Id} found successfully.";
            Log.Me(message);
            Utils.RequestResult<Payroll> successResult = new(
                Utils.Result.Success, message, payroll
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when getting payroll record: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult<Payroll> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, null
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Finds payroll records matching the specified query filters.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>A RequestResult containing the list of matching Payroll records.</returns>
    public static async Task<Utils.RequestResult<List<Payroll>>> FindPayrollAsync(
        FindPayrollRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            IQueryable<Payroll> query = context.Payroll.Include(p => p.Staff);

            if (request is not null)
            {
                if (request.Id is not null)
                {
                    string requestIdStr = request.Id.Value.ToString();
                    query = query.Where(
                        p => p.Id.ToString().Contains(requestIdStr)
                    );
                }

                if (request.StaffId is not null)
                    query = query.Where(
                        p => p.Staff.Id == request.StaffId
                    );

                if (request.MinPeriodStart is not null)
                    query = query.Where(
                        p => p.PeriodStart >= request.MinPeriodStart
                    );

                if (request.MaxPeriodStart is not null)
                    query = query.Where(
                        p => p.PeriodStart <= request.MaxPeriodStart
                    );

                if (request.MinPeriodEnd is not null)
                    query = query.Where(
                        p => p.PeriodEnd >= request.MinPeriodEnd
                    );

                if (request.MaxPeriodEnd is not null)
                    query = query.Where(
                        p => p.PeriodEnd <= request.MaxPeriodEnd
                    );

                if (request.MinNetAmount is not null)
                    query = query.Where(
                        p => p.Amount >= request.MinNetAmount
                    );

                if (request.MaxNetAmount is not null)
                    query = query.Where(
                        p => p.Amount <= request.MaxNetAmount
                    );
            }

            query = Utils.ApplySortingAndPagination(query, request?.Limit, request?.Offset, request?.SortBy, request?.SortOrder);

            List<Payroll> results = await query.ToListAsync();

            if (results.Count == 0)
            {
                message = "No payroll records found matching query.";
                Log.Me(message);
                Utils.RequestResult<List<Payroll>> noResults = new(
                    Utils.Result.Success_NoResults, message, []
                );

                return noResults;
            }

            message = $"Found {results.Count} payroll record(s) matching query.";
            Log.Me(message);
            Utils.RequestResult<List<Payroll>> successResult = new(
                Utils.Result.Success, message, results
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when finding payroll records: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult<List<Payroll>> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, []
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Deletes a specific payroll record.
    /// </summary>
    /// <param name="request">The request containing the payroll ID to delete.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> DeletePayrollAsync(
        DeletePayrollRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Payroll? payroll = await context.Payroll.FindAsync(request.Id);

            if (payroll is null)
            {
                message = $"Payroll record with ID {request.Id} not found.";
                Log.Me(message);
                Utils.RequestResult noResults = new(
                    Utils.Result.Failed_NoResults, message
                );

                return noResults;
            }

            context.Payroll.Remove(payroll);
            await context.SaveChangesAsync();

            message = $"Payroll record with ID {payroll.Id} deleted successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when deleting payroll: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    #endregion

    #region Calculate Payroll

    public record CalculatedPayrollDetails(
        float TotalHours,
        float GrossAmount,
        float CommissionsAndBonuses,
        float Deductions,
        float NetAmount,
        List<ulong> AdjustmentIds
    );

    /// <summary>
    /// Calculates payroll metrics for a staff member over a given period, including auto-deductions.
    /// </summary>
    public static async Task<Utils.RequestResult<CalculatedPayrollDetails>> CalculatePayrollDetailsAsync(
        ulong staffId, DateTime periodStart, DateTime periodEnd)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Staff? staff = await context.Staff.FindAsync(staffId);
            if (staff is null)
            {
                message = $"Staff with ID {staffId} not found.";
                return new Utils.RequestResult<CalculatedPayrollDetails>(Utils.Result.Failed_NoResults, message, null);
            }

            // Fetch attendances
            var attendances = await context.Attendance
                .Where(a => a.Staff.Id == staffId && a.TimeIn >= periodStart && a.TimeIn <= periodEnd && a.TimeOut != null)
                .ToListAsync();

            float totalHours = 0;
            foreach (var a in attendances)
            {
                totalHours += (float)(a.TimeOut!.Value - a.TimeIn).TotalHours;
            }

            float gross = 0;
            if (staff.PayFrequency == Staff.Frequency.Hourly)
            {
                gross = totalHours * staff.PayRate;
            }
            else if (staff.PayFrequency == Staff.Frequency.Daily)
            {
                int daysWorked = attendances.Select(a => a.TimeIn.Date).Distinct().Count();
                gross = daysWorked * staff.PayRate;
            }
            else
            {
                gross = staff.PayRate;
            }

            // Fetch unpaid adjustments
            var adjustments = await context.StaffAdjustment
                .Where(sa => sa.StaffId == staffId && sa.IsPaid == false && sa.Timestamp <= periodEnd)
                .ToListAsync();

            float commissionsAndBonuses = 0;
            float deductions = 0;
            List<ulong> adjIds = [];

            foreach (var sa in adjustments)
            {
                adjIds.Add(sa.Id);
                if (sa.AdjustmentType.Equals("Commission", StringComparison.OrdinalIgnoreCase) ||
                    sa.AdjustmentType.Equals("Bonus", StringComparison.OrdinalIgnoreCase))
                {
                    commissionsAndBonuses += sa.Amount;
                }
                else if (sa.DeductFromCurrentPayroll)
                {
                    deductions += sa.Amount;
                }
            }

            float net = gross + commissionsAndBonuses - deductions;
            if (net < 0) net = 0;

            CalculatedPayrollDetails details = new(totalHours, gross, commissionsAndBonuses, deductions, net, adjIds);
            message = $"Calculated payroll details for staff {staff.Username}.";
            return new Utils.RequestResult<CalculatedPayrollDetails>(Utils.Result.Success, message, details);
        }
        catch (Exception ex)
        {
            message = $"Error calculating payroll details: {ex.Message}";
            return new Utils.RequestResult<CalculatedPayrollDetails>(Utils.Result.Failed_UnhandledException, message, null);
        }
    }

    #endregion
}
