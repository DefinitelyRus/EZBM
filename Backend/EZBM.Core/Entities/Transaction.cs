namespace EZBM.Core.Entities;

public class Transaction : Entity
{
    public enum Type { Income, Expense, Correction }

    public Type TransactionType { get; private set; }

    public float Amount { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? Notes { get; private set; }

    public Entity Reference { get; private set; }

    public Transaction(int id, Type transactionType, float amount, DateTime timestamp, Entity reference, string? notes = null)
    {
        Id = id;
        TransactionType = transactionType;
        Amount = amount;
        Timestamp = timestamp;
        Reference = reference;
        Notes = notes;
    }
}
