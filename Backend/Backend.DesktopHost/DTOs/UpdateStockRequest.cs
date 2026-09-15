using Backend.Core.Common;

namespace Backend.DesktopHost.DTOs;

public class UpdateStockRequest
{
    /// <summary>
    /// The ID of the product to apply the transaction to.
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// The net quantity change to apply to the product.
    /// </summary>
    /// <remarks>
    /// If the <see cref="ProductTransactionMethod">transaction method</see> is
    /// <see cref="ProductTransactionMethod.Manual">manual</see> or
    /// <see cref="ProductTransactionMethod.TotalFirst">total-first</see>,
    /// this value must match the sum of all values in
    /// <see cref="BatchChanges">batch changes</see>.
    /// </remarks>
    public decimal QuantityChange { get; set; }

    /// <summary>
    /// The quantity changes to apply to each listed batch.<br/><br/>
    /// <b>Key:</b> Batch Number <br/>
    /// <b>Value:</b> Quantity
    /// </summary>
    /// <remarks>
    /// Used only for <see cref="ProductTransactionMethod.Manual">manual</see> or
    /// <see cref="ProductTransactionMethod.TotalFirst">total-first</see> transaction methods.
    /// <br/><br/>
    /// The sum of all batch quantities must match the
    /// <see cref="QuantityChange">total quantity</see> change.
    /// </remarks>
    public Dictionary<long, decimal>? BatchChanges { get; set; }

    /// <summary>
    /// The type of transaction to apply to the product.
    /// </summary>
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// How the <see cref="Core.Models.Product">product</see> or
    /// <see cref="Core.Models.ProductBatch">product batch</see> quantities will
    /// be modified for this transaction.
    /// </summary>
    /// <remarks>
    /// If null, use the <see cref="Core.Models.Product.TransactionMethod">product's
    /// transaction method</see>. If that's also null, use the system default
    /// <see cref="ProductTransactionMethod">transaction method</see> instead.
    /// </remarks>
    public ProductTransactionMethod? TransactionMethod { get; set; }

    /// <summary>
    /// Notes that describe the product transaction.
    /// </summary>
    public string[]? Notes { get; set; }
}