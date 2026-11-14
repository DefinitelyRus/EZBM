namespace Backend.DesktopHost.DTOs;

/// <summary>
/// Request record for employee clock-in or clock-out.
/// </summary>
public record ClockActionRequest(
    long UserId,
    string ActionType,
    string? Notes
);

/// <summary>
/// Response record for recorded attendance event.
/// </summary>
public record AttendanceResponse(
    long Id,
    long UserId,
    string ActionType,
    DateTime Timestamp,
    string? Notes
);
