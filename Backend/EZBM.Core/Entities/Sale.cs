namespace EZBM.Core.Entities;

// Instances of this class must be immutable.
public class Sale : Entity
{
    public enum PayMethod { Cash, EWallet, Savings, Credit, Other }

    public string InvoiceNumber { get; private set; }

    public float TotalAmount { get; private set; }
    public PayMethod PaymentMethod { get; private set; }

    public DateTime Timestamp { get; private set; }
    public string? Notes { get; set; }

    public Sale(int id, int invoiceNumber, float totalAmount, PayMethod paymentMethod, DateTime timestamp, string? notes = null)
    {
        Id = id;
        InvoiceNumber = $"INVOICE-{Timestamp.Date:yyyyMMdd}-{invoiceNumber.ToString().PadLeft(6, '0')}";
        TotalAmount = totalAmount;
        PaymentMethod = paymentMethod;
        Timestamp = timestamp;
        Notes = notes;
    }
}
