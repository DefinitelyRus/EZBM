namespace Backend.Core.Models;

/// <summary>
/// Represents a point-of-sale customer order.
/// </summary>
public class SaleOrder : Entity
{
    /// <summary>
    /// Identifier of the user or cashier who processed the sale.
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Total price of all items in the sale.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Timestamp when the sale order was finalized.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Associated payment record.
    /// </summary>
    public PaymentRecord? Payment { get; set; }

    /// <summary>
    /// Line items associated with this order.
    /// </summary>
    public List<SaleOrderItem> Items { get; set; } = [];
}
