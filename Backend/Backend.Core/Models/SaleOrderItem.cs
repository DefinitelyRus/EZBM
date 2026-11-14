namespace Backend.Core.Models;

/// <summary>
/// Represents an individual item within a sales order.
/// </summary>
public class SaleOrderItem : Entity
{
    /// <summary>
    /// Identifier of the related sale order.
    /// </summary>
    public long SaleOrderId { get; set; }

    /// <summary>
    /// Identifier of the sold product or service item.
    /// </summary>
    public long ItemId { get; set; }

    /// <summary>
    /// Type of the sold item ('Product' or 'Service').
    /// </summary>
    public string ItemType { get; set; } = "Product";

    /// <summary>
    /// Name of the item at the time of sale.
    /// </summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// Quantity of items sold.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Unit price of the item at the time of sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Calculated subtotal for this line item.
    /// </summary>
    public decimal Subtotal => Quantity * UnitPrice;
}
