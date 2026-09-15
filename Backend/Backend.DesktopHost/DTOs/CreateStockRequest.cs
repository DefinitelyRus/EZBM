namespace Backend.DesktopHost.DTOs;

public class CreateStockRequest
{
    /// <summary>
    /// The ID of the product to apply the transaction to.
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// How much quantity to add to the product.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// The cost per unit for this batch.
    /// </summary>
    /// <remarks>
    /// This value is used only when the <see cref="Core.Models.Product.TransactionMethod">
    /// product's transaction method</see> is set to
    /// <see cref="Core.Common.ProductTransactionMethod.Fefo">FEFO</see>,
    /// <see cref="Core.Common.ProductTransactionMethod.Manual">manual</see>, or
    /// <see cref="Core.Common.ProductTransactionMethod.TotalFirst">total-first</see>.
    /// </remarks>
    public decimal? Cost { get; set; }

    /// <summary>
    /// When this batch expires.
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Additional notes that describe the created batch.
    /// </summary>
    public string[]? BatchNotes { get; set; }

    /// <summary>
    /// Additional notes that describe the created stock transaction.
    /// </summary>
    public string[]? Notes { get; set; }
}