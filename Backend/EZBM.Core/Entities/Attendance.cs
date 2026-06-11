namespace EZBM.Core.Entities;

/// <summary>
/// Represents an attendance record for a staff member, tracking their clock-in and clock-out times.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editor(s): Google Antigravity<br/>
/// Documented by: Google Gemini, Google Antigravity</i>
/// </summary>
public class Attendance : Entity
{
    /// <summary>
    /// The staff member associated with this record.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public Staff Staff { get; set; }

    /// <summary>
    /// The date and time when the staff member clocked in.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime TimeIn { get; set; }

    /// <summary>
    /// The date and time when the staff member clocked out, or null if still active.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime? TimeOut { get; set; }

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
#pragma warning disable CS8618
    protected Attendance() { }
#pragma warning restore CS8618

    /// <summary>
    /// Initializes a new instance of the Attendance class.
    /// <br/><br/>
    /// <i>Author(s): DefinitelyRus<br/>
    /// Editor(s): Google Antigravity<br/>
    /// Documented by: Google Gemini, Google Antigravity</i>
    /// </summary>
    /// <param name="id">The unique identifier for this entity.</param>
    /// <param name="staff">The ID of the staff member.</param>
    /// <param name="timeIn">The clock-in timestamp.</param>
    /// <param name="timeOut">The optional clock-out timestamp.</param>
    public Attendance(ulong id, Staff staff, DateTime timeIn, DateTime? timeOut = null)
    {
        Id = id;
        Staff = staff;
        TimeIn = timeIn;
        TimeOut = timeOut;
    }
}