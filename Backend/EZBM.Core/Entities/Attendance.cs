namespace EZBM.Core.Entities;

/// <summary>
/// Represents an attendance record for a staff member, tracking their clock-in and clock-out times.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editors(s): None<br/>
/// Documented by: Google Gemini</i>
/// </summary>
public class Attendance : Entity
{
    /// <summary>
    /// The unique identifier of the staff member associated with this record.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public int StaffId { get; set; }

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
    /// Initializes a new instance of the Attendance class.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="id">The unique identifier for this entity.</param>
    /// <param name="staffId">The ID of the staff member.</param>
    /// <param name="timeIn">The clock-in timestamp.</param>
    /// <param name="timeOut">The optional clock-out timestamp.</param>
    public Attendance(int id, int staffId, DateTime timeIn, DateTime? timeOut = null)
    {
        Id = id;
        StaffId = staffId;
        TimeIn = timeIn;
        TimeOut = timeOut;
    }
}