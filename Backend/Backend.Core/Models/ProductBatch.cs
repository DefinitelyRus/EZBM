using Backend.Core.Common;

namespace Backend.Core.Models;

public class ProductBatch : Entity
{
    /// <summary>
    /// The product being batched.
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// How much each unit of this product batch cost.
    /// </summary>
    /// <remarks>
    /// This value should only be used if the <see cref="TransactionMethod">transaction method</see>
    /// is <see cref="ProductTransactionMethod.Fefo">first-expired first-out (FEFO)</see>
    /// or <see cref="ProductTransactionMethod.Manual">manual</see>.
    /// </remarks>
    public decimal Cost { get; set; }

    /// <summary>
    /// A unique, human-readable identifier for this batch.
    /// </summary>
    public long BatchNumber { get; set; }

    /// <summary>
    /// The remaining quantity of products in this batch.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Product.QuantityType"/> for display or calculations. 
    public decimal Quantity { get; set; }

    /// <summary>
    /// The expiration date of this batch.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }
}