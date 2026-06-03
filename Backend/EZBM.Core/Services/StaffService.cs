using EZBM.Core.Entities;
using EZBM.Core.Tools;
using EZBM.Core.Data;
using Microsoft.EntityFrameworkCore;
using static EZBM.Core.Entities.Staff;

/*
 * For the record, I am quite annoyed at how AI tools keep generating
 * code that I did not ask it to generate.
 *
 * They're good and they're already there, so I won't remove it now,
 * but I'm writing the code manually for a reason, damn it.
 *
 * Now I have to go read through all this code to make sure
 * it actually does what I need it to do.
 * I'm dyslexic ffs!
 *
 * - DefinitelyRus
 */

namespace EZBM.Core.Services;

/// <summary>
/// Service class for managing Staff members, Attendance records, and Payroll details in the database.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus, Google Antigravity<br/>
/// Editor(s): Google Antigravity<br/>
/// Documented by: Google Antigravity</i>
/// </summary>
internal static class StaffService
{
    #region Factory Methods

    /// <summary>
    /// Creates a Staff instance from a JSON string.
    /// <br/><br/>
    /// <i>Author(s): DefinitelyRus<br/>
    /// Editor(s): Google Antigravity<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="json">The JSON substring containing staff details.</param>
    internal static Staff? CreateStaffInstance(string json)
    {
        Dictionary<string, object>? contents = Utils.ConvertFromJson(json);

        if (contents is null)
        {
            Log.Err(() => "Unable to parse the input JSON as a Staff object.");
            return null;
        }

        ulong id = 0;
        string username = null!;
        Frequency payFrequency = Frequency.Invalid;
        float payRate = 0;
        string? password = null;
        string? firstName = null;
        string? lastName = null;
        string? email = null;
        string? phoneNumber = null;
        string? position = null;

        foreach (KeyValuePair<string, object> kvp in contents)
        {
            switch (kvp.Key.ToLowerInvariant())
            {
                case "id":
                    ulong? v_id = Utils.GetAsUlong(kvp.Value);
                    if (v_id.HasValue) id = v_id.Value;
                    else Log.Err(() => "The provided 'id' is not a valid 64-bit unsigned integer.");
                    break;

                case "username":
                    string? v_un = Utils.GetAsString(kvp.Value);
                    if (!string.IsNullOrWhiteSpace(v_un)) username = v_un;
                    else Log.Err(() => "The provided 'username' is null or empty.");
                    break;

                case "payfrequency":
                    string? v_pf = Utils.GetAsString(kvp.Value);
                    payFrequency = v_pf?.ToLowerInvariant() switch
                    {
                        "hourly" => Frequency.Hourly,
                        "daily" => Frequency.Daily,
                        "weekly" => Frequency.Weekly,
                        "biweekly" => Frequency.Biweekly,
                        "monthly" => Frequency.Monthly,
                        _ => Frequency.Invalid,
                    };
                    break;

                case "payrate":
                    float? v_pr = Utils.GetAsFloat(kvp.Value);
                    if (v_pr.HasValue) payRate = v_pr.Value;
                    else Log.Err(() => "The provided 'payRate' is not a float. Setting to 0.");
                    break;

                case "password":
                    password = Utils.GetAsString(kvp.Value);
                    break;

                case "firstname":
                    firstName = Utils.GetAsString(kvp.Value);
                    break;

                case "lastname":
                    lastName = Utils.GetAsString(kvp.Value);
                    break;

                case "email":
                    email = Utils.GetAsString(kvp.Value);
                    break;

                case "phonenumber":
                    phoneNumber = Utils.GetAsString(kvp.Value);
                    break;

                case "position":
                    position = Utils.GetAsString(kvp.Value);
                    break;

                default:
                    Log.Warn(() => $"Invalid key '{kvp.Key}' detected with value '{kvp.Value}'. Skipping...");
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            Log.Err(() => "Username is required to create a Staff instance.");
            return null;
        }

        return new Staff(
            id,
            username,
            payFrequency,
            payRate,
            password,
            firstName,
            lastName,
            email,
            phoneNumber,
            position
        );
    }

    /// <summary>
    /// Creates an Attendance instance from a JSON string, resolving the Staff relationship from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="json">The JSON substring containing attendance details.</param>
    /// <param name="context">The database context used to lookup the Staff member.</param>
    internal static Attendance? CreateAttendanceInstance(string json, AppDbContext context)
    {
        Dictionary<string, object>? contents = Utils.ConvertFromJson(json);

        if (contents is null)
        {
            Log.Err(() => "Unable to parse the input JSON as an Attendance object.");
            return null;
        }

        ulong id = 0;
        Staff? staff = null;
        DateTime timeIn = DateTime.UtcNow;
        DateTime? timeOut = null;

        foreach (KeyValuePair<string, object> kvp in contents)
        {
            switch (kvp.Key.ToLowerInvariant())
            {
                case "id":
                    id = Utils.GetAsUlong(kvp.Value) ?? 0;
                    break;

                case "staffid":
                    ulong? staffId = Utils.GetAsUlong(kvp.Value);
                    if (staffId.HasValue)
                    {
                        staff = context.Staff.Find(staffId.Value);
                        if (staff is null)
                        {
                            Log.Err(() => $"Staff member with ID {staffId.Value} not found in database.");
                        }
                    }
                    break;

                case "timein":
                    timeIn = Utils.GetAsDateTime(kvp.Value) ?? DateTime.UtcNow;
                    break;

                case "timeout":
                    timeOut = Utils.GetAsDateTime(kvp.Value);
                    break;

                default:
                    Log.Warn(() => $"Invalid key '{kvp.Key}' detected in Attendance JSON. Skipping...");
                    break;
            }
        }

        if (staff is null)
        {
            Log.Err(() => "Staff member reference is required to create an Attendance instance.");
            return null;
        }

        return new Attendance(id, staff, timeIn, timeOut);
    }

    /// <summary>
    /// Creates a Payroll instance from a JSON string, resolving the Staff relationship from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="json">The JSON substring containing payroll details.</param>
    /// <param name="context">The database context used to lookup the Staff member.</param>
    internal static Payroll? CreatePayrollInstance(string json, AppDbContext context)
    {
        Dictionary<string, object>? contents = Utils.ConvertFromJson(json);

        if (contents is null)
        {
            Log.Err(() => "Unable to parse the input JSON as a Payroll object.");
            return null;
        }

        ulong id = 0;
        Staff? staff = null;
        DateTime periodStart = DateTime.UtcNow;
        DateTime periodEnd = DateTime.UtcNow;
        float totalHours = 0;
        float grossAmount = 0;
        float modifiers = 0;
        float netAmount = 0;
        DateTime? payDate = null;
        string? notes = null;

        foreach (KeyValuePair<string, object> kvp in contents)
        {
            switch (kvp.Key.ToLowerInvariant())
            {
                case "id":
                    id = Utils.GetAsUlong(kvp.Value) ?? 0;
                    break;

                case "staffid":
                    ulong? staffId = Utils.GetAsUlong(kvp.Value);
                    if (staffId.HasValue)
                    {
                        staff = context.Staff.Find(staffId.Value);
                        if (staff is null)
                        {
                            Log.Err(() => $"Staff member with ID {staffId.Value} not found in database.");
                        }
                    }
                    break;

                case "periodstart":
                    periodStart = Utils.GetAsDateTime(kvp.Value) ?? DateTime.UtcNow;
                    break;

                case "periodend":
                    periodEnd = Utils.GetAsDateTime(kvp.Value) ?? DateTime.UtcNow;
                    break;

                case "totalhours":
                    totalHours = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "grossamount":
                    grossAmount = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "modifiers":
                    modifiers = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "netamount":
                    netAmount = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "paydate":
                    payDate = Utils.GetAsDateTime(kvp.Value);
                    break;

                case "notes":
                    notes = Utils.GetAsString(kvp.Value);
                    break;

                default:
                    Log.Warn(() => $"Invalid key '{kvp.Key}' detected in Payroll JSON. Skipping...");
                    break;
            }
        }

        if (staff is null)
        {
            Log.Err(() => "Staff member reference is required to create a Payroll instance.");
            return null;
        }

        return new Payroll(
            id,
            staff,
            periodStart,
            periodEnd,
            totalHours,
            grossAmount,
            modifiers,
            netAmount,
            payDate,
            notes
        );
    }

    #endregion

    #region CRUD Staff Operations

    /// <summary>
    /// Retrieves all staff members from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static List<Staff> GetAllStaff()
    {
        using var context = new AppDbContext();
        return [.. context.Staff];
    }

    /// <summary>
    /// Retrieves a specific staff member by their unique ID.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static Staff? GetStaffById(ulong id)
    {
        using var context = new AppDbContext();
        return context.Staff.Find(id);
    }

    /// <summary>
    /// Adds a new staff member to the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static bool AddStaff(Staff staff)
    {
        try
        {
            using var context = new AppDbContext();
            context.Staff.Add(staff);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Log.Err(() => $"Error adding staff member: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Updates an existing staff member in the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static bool UpdateStaff(Staff staff)
    {
        try
        {
            using var context = new AppDbContext();
            context.Staff.Update(staff);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Log.Err(() => $"Error updating staff member: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Deletes a staff member by their unique ID.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static bool DeleteStaff(ulong id)
    {
        try
        {
            using var context = new AppDbContext();
            var staff = context.Staff.Find(id);
            if (staff is null) return false;
            context.Staff.Remove(staff);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Log.Err(() => $"Error deleting staff member: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Attendance Operations

    /// <summary>
    /// Retrieves all attendance records from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static List<Attendance> GetAllAttendance()
    {
        using var context = new AppDbContext();
        return [.. context.Attendance.Include(a => a.Staff)];
    }

    /// <summary>
    /// Retrieves attendance records for a specific staff member.
    /// <br/><br/>
    /// <i>Author(s): DefinitelyRus<br/>
    /// Editor(s): None<br/>
    /// Documented by: Antigravity</i>
    /// </summary>
    internal static List<Attendance> GetAttendanceForStaff(ulong staffId)
    {
        using var context = new AppDbContext();
        return [.. context.Attendance
            .Include(a => a.Staff)
            .Where(a => a.Staff.Id == staffId)];
    }

    /// <summary>
    /// Logs an attendance record.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static bool LogAttendance(Attendance attendance)
    {
        try
        {
            using var context = new AppDbContext();
            context.Entry(attendance.Staff).State = EntityState.Unchanged;
            context.Attendance.Add(attendance);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Log.Err(() => $"Error logging attendance record: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Payroll Operations

    /// <summary>
    /// Retrieves all payroll records from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static List<Payroll> GetAllPayroll()
    {
        using var context = new AppDbContext();
        return [.. context.Payroll.Include(p => p.Staff)];
    }

    /// <summary>
    /// Retrieves payroll records for a specific staff member.
    /// <br/><br/>
    /// <i>Author(s): DefinitelyRus<br/>
    /// Editor(s): Google Antigravity<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static List<Payroll> GetPayrollForStaff(ulong staffId)
    {
        using var context = new AppDbContext();
        return [.. context.Payroll
            .Include(p => p.Staff)
            .Where(p => p.Staff.Id == staffId)];
    }

    /// <summary>
    /// Adds a new payroll record to the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static bool AddPayroll(Payroll payroll)
    {
        try
        {
            using var context = new AppDbContext();
            context.Entry(payroll.Staff).State = EntityState.Unchanged;
            context.Payroll.Add(payroll);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Log.Err(() => $"Error adding payroll record: {ex.Message}");
            return false;
        }
    }

    #endregion
}