using EZBM.Core.Tools;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents a record of stock movement or adjustment for a specific item.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editor(s): None<br/>
/// Documented by: Google Gemini, Antigravity</i>
/// </summary>
public class ItemTransaction : Entity
{
    /// <summary>
    /// Defines the reason or nature of the item transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public enum Type { NewStock, Sale, Consumed, Damaged_Lost_Expired, Correction_Sum, Correction_Set }

    /// <summary>
    /// The category of this stock movement.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public Type TransactionType { get; private set; }

    /// <summary>
    /// The amount of the item involved in the transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float Quantity { get; private set; }

    /// <summary>
    /// The staff member who performed or authorized the stock movement.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public Staff Staff { get; private set; }

    /// <summary>
    /// The date and time when the transaction occurred.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Additional context or reasons for the transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? Note { get; private set; }

    /// <summary>
    /// The item associated with this transaction.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public Item Item { get; private set; }

    /// <summary>
    /// The specific sale entry if this transaction was triggered by a sale.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public SaleEntry? SaleEntry { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ItemTransaction class.
    /// <br/><br/>
    /// <i>Author(s): DefinitelyRus<br/>
    /// Editor(s): Google Antigravity<br/>
    /// Documented by: Google Gemini, Antigravity</i>
    /// </summary>
    /// <param name="item">The item being tracked.</param>
    /// <param name="transactionType">The type of stock movement.</param>
    /// <param name="saleEntry">Optional reference to a sale record.</param>
    /// <param name="quantity">The amount of stock changed.</param>
    /// <param name="staff">The staff member responsible.</param>
    /// <param name="timestamp">The time of the event.</param>
    /// <param name="note">Optional remarks.</param>
    public ItemTransaction(
        Item item,
        Type transactionType,
        SaleEntry? saleEntry,
        float quantity,
        Staff staff,
        DateTime timestamp,
        string? note = null)
    {
        Id = Utils.GenerateEntityId();
        Item = item;
        TransactionType = transactionType;
        SaleEntry = saleEntry;
        Quantity = quantity;
        Staff = staff;
        Timestamp = timestamp;
        Note = note;
    }
}
