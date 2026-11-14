using Backend.Core.Common;
using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.DesktopHost.Services;

/// <summary>
/// Service calculating shift hours and payroll summaries.
/// </summary>
public class PayrollService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    #region Payroll Calculation

    /// <summary>
    /// Calculates total shift hours and gross pay for an employee.
    /// </summary>
    public async Task<PayrollSummaryResponse> CalculatePayrollAsync(
        long userId,
        decimal hourlyRate,
        DateTime? startDate,
        DateTime? endDate
    )
    {
        if (hourlyRate < 0)
        {
            throw Esc.New<ArgumentException>(
                "Hourly rate cannot be negative.",
                hourlyRate
            );
        }

        IQueryable<AttendanceRecord> query = _context.AttendanceRecords
            .AsNoTracking()
            .Where(a => a.UserId == userId);

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= endDate.Value);
        }

        List<AttendanceRecord> records = await query
            .OrderBy(a => a.Timestamp)
            .ToListAsync();

        List<ShiftDto> shifts = [];
        AttendanceRecord? activeClockIn = null;
        double totalHours = 0;

        foreach (AttendanceRecord record in records)
        {
            bool isClockIn = record.ActionType
                .Equals("In", StringComparison.OrdinalIgnoreCase);
            bool isClockOut = record.ActionType
                .Equals("Out", StringComparison.OrdinalIgnoreCase);

            if (isClockIn)
            {
                activeClockIn = record;
            }
            else if (isClockOut && activeClockIn is not null)
            {
                TimeSpan duration =
                    record.Timestamp - activeClockIn.Timestamp;
                double shiftHours = Math.Max(0, duration.TotalHours);
                totalHours += shiftHours;

                shifts.Add(new ShiftDto(
                    ClockIn: activeClockIn.Timestamp,
                    ClockOut: record.Timestamp,
                    HoursWorked: Math.Round(shiftHours, 2)
                ));

                activeClockIn = null;
            }
        }

        decimal grossPay = (decimal)totalHours * hourlyRate;

        PayrollSummaryResponse response = new(
            UserId: userId,
            TotalHours: Math.Round(totalHours, 2),
            HourlyRate: hourlyRate,
            GrossPay: Math.Round(grossPay, 2),
            Shifts: shifts
        );

        return response;
    }

    #endregion
}
