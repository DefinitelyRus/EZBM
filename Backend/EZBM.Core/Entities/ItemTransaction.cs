using EZBM.Core.Tools;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents a record of stock movement or adjustment for a specific item.
/// </summary>
public class ItemTransaction : Entity
{
    /// <summary>
    /// Defines the reason or nature of the item transaction.
    /// </summary>
    public enum Type { NewStock, Sale, Consumed, Damaged_Lost_Expired, Correction_Sum, Correction_Set }

    /// <summary>
    /// The category of this stock movement.
    /// </summary>
    public Type TransactionType { get; private set; }

    /// <summary>
    /// The amount of the item involved in the transaction.
    /// </summary>
    public float Quantity { get; private set; }

    /// <summary>
    /// The staff member who performed or authorized the stock movement.
    /// </summary>
    public Staff Staff { get; private set; }

    /// <summary>
    /// The date and time when the transaction occurred.
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Additional context or reasons for the transaction.
    /// </summary>
    public string? Note { get; private set; }

    /// <summary>
    /// The item associated with this transaction.
    /// </summary>
    public Item Item { get; private set; }

    /// <summary>
    /// The specific sale entry if this transaction was triggered by a sale.
    /// </summary>
    public SaleEntry? SaleEntry { get; private set; }

    /// <summary>
    /// The transactions associated with this stock movement.
    /// </summary>
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
#pragma warning disable CS8618
    protected ItemTransaction() { }
#pragma warning restore CS8618

    /// <summary>
    /// Initializes a new instance of the ItemTransaction class.
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
