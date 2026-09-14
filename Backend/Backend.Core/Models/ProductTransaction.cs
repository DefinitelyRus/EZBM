using Backend.Core.Common;

namespace Backend.Core.Models;

public class ProductTransaction : Entity
{
    /// <summary>
    /// The <see cref="Product">product</see> this transaction affects.
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// The <see cref="ProductBatch">product batch</see> this transaction affects. 
    /// </summary>
    public ProductBatch? Batch { get; set; }

    /// <summary>
    /// How this transaction affects the <see cref="Product">product</see> and/or
    /// <see cref="ProductBatch">product batch</see>.
    /// </summary>
    public ProductTransactionMethod Method { get; set; }

    /// <summary>
    /// The quantity to add, remove, or set.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// How this transaction affects the <see cref="ProductBatch.Quantity">batch quantity</see>
    /// and/or <see cref="Product.TotalQuantity">total product quantity</see>.
    /// </summary>
    public TransactionType TransactionType { get; set; }

    // TODO: Implement in Phase 3
    //public long StaffId { get; set; }
    //public Staff Staff { get; set; }

    /// <summary>
    /// The date and time of when this product transaction happened.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="DateTime.UtcNow"/> if not specified.
    /// </remarks>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}