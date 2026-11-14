namespace Backend.Core.Models;

/// <summary>
/// Record representing employee clock-in and clock-out actions.
/// </summary>
public class AttendanceRecord : Entity
{
    /// <summary>
    /// Identifier of the employee user.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// The action type performed ('In' or 'Out').
    /// </summary>
    public string ActionType { get; set; } = "In";

    /// <summary>
    /// Timestamp when the attendance event was recorded.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Remarks regarding the shift action.
    /// </summary>
    public string? Remarks { get; set; }
}
