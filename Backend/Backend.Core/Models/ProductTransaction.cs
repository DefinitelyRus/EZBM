using Backend.Core.Common;

namespace Backend.Core.Models;

public class ProductTransaction : Entity
{
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Quantity { get; set; }
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// The product batch(es) associated with this product transaction.
    /// <br/><br/>
    /// Not all products are batched, so this property is nullable.
    /// <br/><br/>
    /// If a transaction affects a batched product, it will update the quantity of the specified batches and/or create new batches if the transaction type is 'Purchase'.
    /// </summary>
    public List<ProductBatch>? Batches { get; set; }

    // TODO: Implement in Phase 3
    //public long StaffId { get; set; }
    //public Staff Staff { get; set; }

    /// <summary>
    /// When the product transaction took place.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="DateTime.UtcNow"/> if not specified.
    /// </remarks>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}