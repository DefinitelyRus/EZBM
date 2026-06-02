namespace EZBM.Core.Entities;

/// <summary>
/// Represents an individual line item within a sale, recording the quantity and price at the time of purchase.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editors(s): None<br/>
/// Documented by: Google Gemini</i>
/// </summary>
public class SaleEntry(Sale sale, Item item, float quantity, float unitPrice, float subtotal) : Entity
{
    /// <summary>
    /// The amount of the item purchased.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float Quantity { get; private set; } = quantity;

    /// <summary>
    /// The price per unit of the item at the time of the sale.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float UnitPrice { get; private set; } = unitPrice;

    /// <summary>
    /// The total cost for this entry (Quantity * UnitPrice).
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float Subtotal { get; private set; } = subtotal;

    /// <summary>
    /// The parent sale transaction this entry belongs to.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public Sale Sale { get; private set; } = sale;

    /// <summary>
    /// The item being sold.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public Item Item { get; private set; } = item;
}
