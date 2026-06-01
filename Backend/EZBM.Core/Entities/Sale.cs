namespace EZBM.Core.Entities;

/// <summary>
/// Represents a completed sales transaction, including payment details and invoice information.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editors(s): None<br/>
/// Documented by: Google Gemini</i>
/// </summary>
public class Sale : Entity
{
    /// <summary>
    /// Defines the supported payment methods for a sale.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public enum PayMethod { Cash, EWallet, Savings, Credit, Other }

    /// <summary>
    /// The unique generated invoice string for the sale.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string InvoiceNumber { get; private set; }

    /// <summary>
    /// The total monetary value of the sale.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float TotalAmount { get; private set; }

    /// <summary>
    /// The method used by the customer to pay for the sale.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public PayMethod PaymentMethod { get; private set; }

    /// <summary>
    /// The date and time when the sale occurred.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Additional remarks or information regarding the sale.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Initializes a new instance of the Sale class.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="id">The unique identifier for this entity.</param>
    /// <param name="invoiceNumber">The numeric sequence for the invoice.</param>
    /// <param name="totalAmount">The total cost of the sale.</param>
    /// <param name="paymentMethod">The method of payment.</param>
    /// <param name="timestamp">The time of the transaction.</param>
    /// <param name="notes">Optional notes about the sale.</param>
    public Sale(
        int id,
        int invoiceNumber,
        float totalAmount,
        PayMethod paymentMethod,
        DateTime timestamp,
        string? notes = null)
    {
        Id = id;

        string datePart = Timestamp.Date.ToString("yyyyMMdd");
        string sequencePart = invoiceNumber.ToString().PadLeft(6, '0');
        InvoiceNumber = $"INVOICE-{datePart}-{sequencePart}";

        TotalAmount = totalAmount;
        PaymentMethod = paymentMethod;
        Timestamp = timestamp;
        Notes = notes;
    }
}
