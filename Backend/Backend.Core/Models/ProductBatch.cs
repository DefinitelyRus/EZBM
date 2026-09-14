using Backend.Core.Common;

namespace Backend.Core.Models;

public class ProductBatch : Entity
{
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public long BatchNumber { get; set; }
    public decimal Quantity { get; set; }
    // Uses `Product.QuantityType`.

    // Properties that matter in batches
    public DateTime? ExpirationDate { get; set; }
}