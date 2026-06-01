namespace EZBM.Core.Entities;

/// <summary>
/// Represents a payroll record for a staff member, detailing earnings and payment status for a specific period.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editors(s): None<br/>
/// Documented by: Google Gemini</i>
/// </summary>
public class Payroll : Entity
{
    /// <summary>
    /// The unique identifier of the staff member associated with this payroll.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public int StaffId { get; set; }

    /// <summary>
    /// Indicates whether the payroll has been processed and paid.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// The date when the payment was issued.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime PayDate { get; set; }

    /// <summary>
    /// The start date of the pay period.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// The end date of the pay period.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// The total number of hours worked during the period.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float TotalHours { get; set; }

    /// <summary>
    /// The total earnings before any deductions or additions.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float GrossAmount { get; set; }

    /// <summary>
    /// Adjustments made to the gross amount, such as bonuses or deductions.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float Modifiers { get; set; }

    /// <summary>
    /// The final amount to be paid after modifiers are applied.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float NetAmount { get; set; }

    /// <summary>
    /// Initializes a new instance of the Payroll class.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="id">The unique identifier for this entity.</param>
    /// <param name="staffId">The ID of the staff member.</param>
    /// <param name="periodStart">The start of the pay period.</param>
    /// <param name="periodEnd">The end of the pay period.</param>
    /// <param name="totalHours">Total hours worked.</param>
    /// <param name="grossAmount">Earnings before modifiers.</param>
    /// <param name="modifiers">Deductions or bonuses.</param>
    /// <param name="netAmount">Final pay amount.</param>
    /// <param name="isPaid">Payment status.</param>
    /// <param name="payDate">The date of payment.</param>
    public Payroll(
        int id,
        int staffId,
        DateTime periodStart,
        DateTime periodEnd,
        float totalHours,
        float grossAmount,
        float modifiers,
        float netAmount,
        bool isPaid = false,
        DateTime? payDate = null)
    {
        Id = id;
        StaffId = staffId;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        TotalHours = totalHours;
        GrossAmount = grossAmount;
        Modifiers = modifiers;
        NetAmount = netAmount;
        IsPaid = isPaid;
        PayDate = payDate ?? DateTime.UtcNow;
    }
}
