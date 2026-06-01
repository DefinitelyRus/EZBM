namespace EZBM.Core.Entities;

// Instances of this class must be immutable.
public class ItemTransaction : Entity
{
    public enum Type { NewStock, Sale, Damaged_Lost_Expired, Correction }

    public Type TransactionType { get; private set; }

    public float Quantity { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? Notes { get; private set; }

    public Item Item { get; private set; }
    public SaleEntry? SaleEntry { get; private set; }

    public ItemTransaction(int id, Item item, Type transactionType, SaleEntry? saleEntry, float quantity, DateTime timestamp, string? notes = null)
    {
        Id = id;
        Item = item;
        TransactionType = transactionType;
        SaleEntry = saleEntry;
        Quantity = quantity;
        Timestamp = timestamp;
        Notes = notes;
    }
}
