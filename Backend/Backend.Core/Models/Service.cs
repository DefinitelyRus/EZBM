using Backend.Core.Common;

namespace Backend.Core.Models;

// Currently has no additional properties over Entity and ISellable
public class Service : Entity, ISellable
{
    #region Inherited properties

    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];

    public decimal Cost { get; set; }
    public decimal Price { get; set; }

    public string Sku { get; set; } = string.Empty;
    public bool IsForSale { get; set; }
    public QuantityType QuantityType { get; set; }

    #endregion


}