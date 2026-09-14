using Backend.Core.Common;

namespace Backend.Core.Models;

// Currently only adds Barcode over Entity and ISellable
public class Product : Entity, ISellable
{
    #region Inherited properties

    /// <summary>
    /// The name of the product.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A short, human-readable description of the product in plain-text.
    /// </summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// A longer description of the product.
    /// </summary>
    /// <remarks>
    /// Supports markdown.
    /// </remarks>
    public string LongDescription { get; set; } = string.Empty;

    /// <summary>
    /// A list of keywords associated with the product.
    /// </summary>
    public string[] Tags { get; set; } = [];

    /// <summary>
    /// How much each unit of this product cost.
    /// </summary>
    /// <remarks>
    /// This value should only be used if the <see cref="TransactionMethod">transaction method</see>
    /// is <see cref="ProductTransactionMethod.TotalFirst">total-first</see>
    /// or <see cref="ProductTransactionMethod.TotalOnly">total-only</see>.
    /// </remarks>
    public decimal Cost { get; set; }

    /// <summary>
    /// The price of the product.
    /// </summary>
    public decimal Price { get; set; }

    public string Sku { get; set; } = string.Empty;
    public bool IsForSale { get; set; }
    public decimal TotalQuantity { get; set; }
    public QuantityType QuantityType { get; set; }
    public ProductTransactionMethod TransactionMethod { get; set; }

    #endregion

    public string? Barcode { get; set; }

}