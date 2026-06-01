namespace EZBM.Core.Entities;

/// <summary>
/// Represents a financial transaction within the system, tracking income, expenses, and corrections.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editors(s): None<br/>
/// Documented by: Google Gemini</i>
/// </summary>
public class Transaction : Entity
{
    /// <summary>
    /// Defines the nature of the financial transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public enum Type { Income, Expense, Correction }

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
    /// Additional context or remarks about the transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// The entity related to this transaction, such as a Sale or Payroll.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public Entity Reference { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Transaction class.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="id">The unique identifier for this entity.</param>
    /// <param name="transactionType">The type of financial movement.</param>
    /// <param name="amount">The monetary amount involved.</param>
    /// <param name="timestamp">The time of the transaction.</param>
    /// <param name="reference">The associated domain entity.</param>
    /// <param name="notes">Optional remarks.</param>
    public Transaction(
        int id,
        Type transactionType,
        float amount,
        DateTime timestamp,
        Entity reference,
        string? notes = null)
    {
        Id = id;
        TransactionType = transactionType;
        Amount = amount;
        Timestamp = timestamp;
        Reference = reference;
        Notes = notes;
    }
}
