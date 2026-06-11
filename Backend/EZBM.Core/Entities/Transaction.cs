using EZBM.Core.Tools;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents a financial transaction within the system, tracking income, expenses, and corrections.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editor(s): Google Antigravity<br/>
/// Documented by: Google Gemini, Google Antigravity</i>
/// </summary>
public class Transaction : Entity
{
    #region Enums

    /// <summary>
    /// Defines the nature of the financial transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public enum Type { Income, Expense, Correction }

    /// <summary>
    /// Defines the supported payment methods for a transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public enum PayMethod { Cash, EWallet, Savings, Credit, Other }

    #endregion

    #region Properties

    /// <summary>
    /// The category of this financial record.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public Type TransactionType { get; private set; }

    /// <summary>
    /// The monetary value of the transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float Amount { get; private set; }

    /// <summary>
    /// The date and time when the transaction was recorded.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// The staff member associated with the transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public Staff Staff { get; private set; }

    /// <summary>
    /// The method used for payment, if applicable.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public PayMethod? PaymentMethod { get; private set; }

    /// <summary>
    /// The numeric sequence for the invoice.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public int InvoiceNumber { get; private set; }

    /// <summary>
    /// The unique generated invoice string, if applicable.
    /// <br/><br/>
    /// <b>Note:</b> This property does not have a direct database equivalent.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <remarks>
    /// </remarks>
    public string? InvoiceId { get; protected set; }

    /// <summary>
    /// Additional context or remarks about the transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? Notes { get; private set; }

    #endregion

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
#pragma warning disable CS8618
    protected Transaction() { }
#pragma warning restore CS8618

    /// <summary>
    /// Initializes a new instance of the Transaction class.
    /// <br/><br/>
    /// <i>Author(s): DefinitelyRus<br/>
    /// Editor(s): Google Antigravity<br/>
    /// Documented by: Google Gemini, Google Antigravity</i>
    /// </summary>
    /// <param name="id">The unique identifier for this entity.</param>
    /// <param name="transactionType">The type of financial movement.</param>
    /// <param name="amount">The monetary amount involved.</param>
    /// <param name="timestamp">The time of the transaction.</param>
    /// <param name="staff">The staff member associated with the transaction.</param>
    /// <param name="paymentMethod">The method of payment.</param>
    /// <param name="invoiceNumber">The numeric sequence for the invoice.</param>
    /// <param name="invoicePrefix">The prefix for the generated invoice ID.</param>
    /// <param name="notes">Optional remarks.</param>
    public Transaction(
        ulong id,
        Type transactionType,
        float amount,
        DateTime timestamp,
        Staff staff,
        PayMethod? paymentMethod = null,
        int? invoiceNumber = null,
        string? invoicePrefix = "GENERIC",
        string? notes = null)
    {
        Id = id;
        TransactionType = transactionType;
        Amount = amount;
        Timestamp = timestamp;
        Staff = staff;
        PaymentMethod = paymentMethod;
        Notes = notes;

        InvoiceNumber = invoiceNumber ?? Utils.GenerateInvoiceNumber(timestamp);
        string datePart = timestamp.Date.ToString("yyyyMMdd");
        string sequencePart = InvoiceNumber.ToString().PadLeft(6, '0');
        InvoiceId = $"{invoicePrefix}-{datePart}-{sequencePart}";
    }
}
