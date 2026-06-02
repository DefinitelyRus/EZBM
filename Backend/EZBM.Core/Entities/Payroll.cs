namespace EZBM.Core.Entities;

using System;

/// <summary>
/// Represents a payroll record for a staff member, detailing earnings and payment status for a specific period.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editors(s): None<br/>
/// Documented by: Google Gemini</i>
/// </summary>
public class Payroll : Transaction
{
    /// <summary>
    /// The start date of the pay period.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime PeriodStart { get; private set; }

    /// <summary>
    /// The end date of the pay period.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime PeriodEnd { get; private set; }

    /// <summary>
    /// The total number of hours worked during the period.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float TotalHours { get; private set; }

    /// <summary>
    /// The total earnings before any deductions or additions.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float GrossAmount { get; private set; }

    /// <summary>
    /// Adjustments made to the gross amount, such as bonuses or deductions.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float Modifiers { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Payroll class.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="id">The unique identifier for this entity.</param>
    /// <param name="staff">The staff member associated with this record.</param>
    /// <param name="periodStart">The start of the pay period.</param>
    /// <param name="periodEnd">The end of the pay period.</param>
    /// <param name="totalHours">Total hours worked.</param>
    /// <param name="grossAmount">Earnings before modifiers.</param>
    /// <param name="modifiers">Deductions or bonuses.</param>
    /// <param name="netAmount">Final pay amount (mapped to transaction amount).</param>
    /// <param name="payDate">The date of payment (mapped to transaction timestamp).</param>
    /// <param name="notes">Optional remarks about the payroll.</param>
    public Payroll(
        int id,
        Staff staff,
        DateTime periodStart,
        DateTime periodEnd,
        float totalHours,
        float grossAmount,
        float modifiers,
        float netAmount,
        DateTime? payDate = null,
        string? notes = null) : base(id, Transaction.Type.Expense, netAmount, payDate ?? DateTime.UtcNow, staff, null, null, notes)
    {
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        TotalHours = totalHours;
        GrossAmount = grossAmount;
        Modifiers = modifiers;
    }
}
