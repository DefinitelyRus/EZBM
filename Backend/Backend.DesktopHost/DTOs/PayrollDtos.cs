namespace Backend.DesktopHost.DTOs;

/// <summary>
/// Shift pair summary for an employee.
/// </summary>
public record ShiftDto(
    DateTime ClockIn,
    DateTime? ClockOut,
    double HoursWorked,
    bool IsCompleted
);

/// <summary>
/// Summary response record for employee payroll calculations.
/// </summary>
public record PayrollSummaryResponse(
    long UserId,
    double TotalHours,
    decimal HourlyRate,
    decimal GrossPay,
    int UnpairedClockIns,
    List<ShiftDto> Shifts
);
