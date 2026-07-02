using System;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents a completed sales transaction.
/// <br/><br/>
/// <b>Note:</b> This class has no additional members compared to <see cref="Transaction"/>.
/// It is made purely to distinguish from other <see cref="Transaction"/> subtypes.
/// </summary>
/// <param name="id">The unique identifier for this entity.</param>
/// <param name="invoiceNumber">The numeric sequence for the invoice.</param>
/// <param name="amount">The total cost of the sale.</param>
/// <param name="paymentMethod">The method of payment.</param>
/// <param name="staff">The staff member responsible for the sale.</param>
/// <param name="timestamp">The time of the transaction.</param>
/// <param name="notes">Optional notes about the sale.</param>
public class Sale : Transaction
{
    #region Constructors

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    protected Sale() : base() { }

    /// <summary>
    /// Initializes a new instance of the Sale class.
    /// </summary>
    public Sale(
        ulong id,
        int invoiceNumber,
        float amount,
        PayMethod paymentMethod,
        Staff staff,
        DateTime timestamp,
        string? notes = null) : base(
            id,
            Type.Income,
            amount,
            timestamp,
            staff,
            paymentMethod,
            invoiceNumber,
            "SALE",
            notes)
    {
    }

    #endregion
}
