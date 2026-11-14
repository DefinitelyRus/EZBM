using Backend.Core.Common;

namespace Backend.Core.Models;

/// <summary>
/// Represents a payment transaction made by a customer.
/// </summary>
public class PaymentRecord : Entity
{
    /// <summary>
    /// The total amount charged for the sale.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The amount of money tendered by the customer.
    /// </summary>
    public decimal AmountTendered { get; set; }

    /// <summary>
    /// The change returned to the customer.
    /// </summary>
    public decimal ChangeAmount { get; set; }

    /// <summary>
    /// The payment method used.
    /// </summary>
    public PaymentMethod Method { get; set; }

    /// <summary>
    /// Optional transaction or external reference note.
    /// </summary>
    public string? ReferenceNote { get; set; }

    /// <summary>
    /// The timestamp when the payment occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
