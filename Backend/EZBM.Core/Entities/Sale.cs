namespace EZBM.Core.Entities;

using System;

/// <summary>
/// Represents a completed sales transaction.
/// <br/><br/>
/// <b>Note:</b> This class has no additional members compared to <see cref="Transaction"/>.
/// It is made purely to distinguish from other <see cref="Transaction"/> subtypes.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editor(s): Google Antigravity<br/>
/// Documented by: Google Gemini, Google Antigravity</i>
/// </summary>
/// <param name="id">The unique identifier for this entity.</param>
/// <param name="invoiceNumber">The numeric sequence for the invoice.</param>
/// <param name="amount">The total cost of the sale.</param>
/// <param name="paymentMethod">The method of payment.</param>
/// <param name="staff">The staff member responsible for the sale.</param>
/// <param name="timestamp">The time of the transaction.</param>
/// <param name="notes">Optional notes about the sale.</param>
public class Sale(
    ulong id,
    int invoiceNumber,
    float amount,
    Transaction.PayMethod paymentMethod,
    Staff staff,
    DateTime timestamp,
    string? notes = null) : Transaction(id, Type.Income, amount, timestamp, staff, paymentMethod, invoiceNumber, "SALE", notes)
{
    // Yes, this class does nothing.
}
