namespace EZBM.Core.Entities;

// Instances of this class must be immutable.
public class SaleEntry(Sale sale, Item item, float quantity, float unitPrice, float subtotal)
{
    public float Quantity { get; private set; } = quantity;
    public float UnitPrice { get; private set; } = unitPrice;
    public float Subtotal { get; private set; } = subtotal;

    public Sale Sale { get; private set; } = sale;
    public Item Item { get; private set; } = item;
}
