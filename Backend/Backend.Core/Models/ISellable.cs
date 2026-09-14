using Backend.Core.Common;

namespace Backend.Core.Models;

public interface ISellable
{
    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public string[] Tags { get; set; }

    public decimal Cost { get; set; }
    public decimal Price { get; set; }

    public string Sku { get; set; }
    public bool IsForSale { get; set; }
    public QuantityType QuantityType { get; set; }
}