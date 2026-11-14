using Backend.Core.Common;
using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.DesktopHost.Services;

/// <summary>
/// Service managing employee attendance and clock actions.
/// </summary>
public class AttendanceService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    #region Attendance Methods

    /// <summary>
    /// Records a clock-in or clock-out action for an employee.
    /// </summary>
    public async Task<AttendanceResponse> ClockActionAsync(
        ClockActionRequest request
    )
    {
        if (request.UserId <= 0)
        {
            throw Esc.New<ArgumentException>(
                "Invalid user identifier.",
                request.UserId
            );
        }

        string normalizedAction = request.ActionType.Trim();
        bool isActionValid =
            normalizedAction.Equals("In", StringComparison.OrdinalIgnoreCase) ||
            normalizedAction.Equals("Out", StringComparison.OrdinalIgnoreCase);

        if (!isActionValid)
        {
            throw Esc.New<ArgumentException>(
                "ActionType must be either 'In' or 'Out'.",
                request.ActionType
            );
        }

        AttendanceRecord record = new()
        {
            UserId = request.UserId,
            ActionType = char.ToUpperInvariant(normalizedAction[0]) +
                         normalizedAction[1..].ToLowerInvariant(),
            Timestamp = DateTime.UtcNow,
            Remarks = request.Notes
        };

        await _context.AttendanceRecords.AddAsync(record);
        await _context.SaveChangesAsync();

        AttendanceResponse response = new(
            Id: record.Id,
            UserId: record.UserId,
            ActionType: record.ActionType,
            Timestamp: record.Timestamp,
            Notes: record.Remarks
        );

        return response;
    }

    /// <summary>
    /// Retrieves historical attendance logs filtered by user and date range.
    /// </summary>
    public async Task<List<AttendanceResponse>> GetAttendanceHistoryAsync(
        long? userId,
        DateTime? startDate,
        DateTime? endDate
    )
    {
        IQueryable<AttendanceRecord> query = _context.AttendanceRecords
            .AsNoTracking();

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= endDate.Value);
        }

        List<AttendanceResponse> results = await query
            .OrderByDescending(a => a.Timestamp)
            .Select(a => new AttendanceResponse(
                a.Id,
                a.UserId,
                a.ActionType,
                a.Timestamp,
                a.Remarks
            ))
            .ToListAsync();

        return results;
    }

    #endregion
}
