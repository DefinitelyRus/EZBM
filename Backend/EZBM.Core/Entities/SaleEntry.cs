using EZBM.Core.Tools;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents an individual line item within a sale, recording the quantity and price at the time of purchase.
/// </summary>
public class SaleEntry : Entity
{
    #region Properties

    /// <summary>
    /// The amount of the item purchased.
    /// </summary>
    public float Quantity { get; private set; }

    /// <summary>
    /// The price per unit of the item at the time of the sale.
    /// </summary>
    public float UnitPrice { get; private set; }

    /// <summary>
    /// The total cost for this entry (Quantity * UnitPrice).
    /// </summary>
    public float Subtotal { get; private set; }

    /// <summary>
    /// The parent sale transaction this entry belongs to.
    /// </summary>
    public Sale Sale { get; private set; }

    /// <summary>
    /// The item being sold.
    /// </summary>
    public Item Item { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
#pragma warning disable CS8618
    protected SaleEntry() { }
#pragma warning restore CS8618

    /// <summary>
    /// Creates a new instance of the <see cref="SaleEntry"/> class.
    /// </summary>
    /// <param name="sale">The sale transaction this entry belongs to.</param>
    /// <param name="item">The item being sold.</param>
    /// <param name="quantity">The amount of the item purchased.</param>
    /// <param name="unitPrice">The price per unit of the item at the time of the sale.</param>
    /// <param name="subtotal"></param>
    public SaleEntry(
        Sale sale,
        Item item,
        float quantity,
        float unitPrice,
        float subtotal)
    {
        Id = Utils.GenerateEntityId();
        Sale = sale;
        Item = item;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Subtotal = subtotal;
    }

    #endregion
}
