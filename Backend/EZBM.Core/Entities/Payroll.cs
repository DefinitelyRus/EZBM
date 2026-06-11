namespace EZBM.Core.Entities;

using System;

/// <summary>
/// Represents a payroll record for a staff member, detailing earnings and payment status for a specific period.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editor(s): Google Antigravity<br/>
/// Documented by: Google Gemini, Google Antigravity</i>
/// </summary>
/// <remarks>
/// Initializes a new instance of the Payroll class.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editor(s): Google Antigravity<br/>
/// Documented by: Google Gemini, Google Antigravity</i>
/// </remarks>
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
    /// Parameterless constructor for EF Core.
    /// </summary>
    protected Payroll() : base() { }

    /// <summary>
    /// Initializes a new instance of the Payroll class.
    /// </summary>
    public Payroll(
        ulong id,
        Staff staff,
        DateTime periodStart,
        DateTime periodEnd,
        float totalHours,
        float grossAmount,
        float modifiers,
        float netAmount,
        DateTime? payDate = null,
        string? notes = null) : base(
            id: id,
            transactionType: Type.Expense,
            amount: netAmount,
            timestamp: payDate ?? DateTime.UtcNow,
            staff: staff,
            paymentMethod: null,
            invoiceNumber: null,
            invoicePrefix: "PAYROLL",
            notes: notes)
    {
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        TotalHours = totalHours;
        GrossAmount = grossAmount;
        Modifiers = modifiers;
    }

}

