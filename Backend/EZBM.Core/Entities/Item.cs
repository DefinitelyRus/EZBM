namespace EZBM.Core.Entities;

/// <summary>
/// Represents a product or resource in the inventory, tracking its stock, pricing, and properties.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editor(s): Google Antigravity<br/>
/// Documented by: Google Gemini, Google Antigravity</i>
/// </summary>
public class Item : Entity
{
    /// <summary>
    /// Defines the units used to measure the quantity of the item.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public enum Unit { Count, Milligrams, Grams, Kilograms, Ounces, Pounds, Milliliters, Liters, Gallons }

    /// <summary>
    /// Categories used to classify the item for filtering or reporting.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public enum Tag { Food, Hygiene, Consumable, Reusable }

    /// <summary>
    /// The display name of the item.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A detailed description of the item.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// A URL pointing to an image representing the item.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Indicates if the item is available for purchase by customers.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public bool IsForSale { get; set; }

    /// <summary>
    /// The cost price paid to acquire the item.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float? Cost { get; set; }

    /// <summary>
    /// The price at which the item is sold to customers.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float? SalePrice { get; set; }

    /// <summary>
    /// The current amount of stock available.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float Quantity { get; set; }

    /// <summary>
    /// The unit of measurement applied to the quantity.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public Unit UnitOfMeasurement { get; set; }

    /// <summary>
    /// The date when the item expires, if applicable.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// A list of tags associated with the item.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public List<Tag> Tags { get; set; } = [];

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
#pragma warning disable CS8618
    protected Item() { }
#pragma warning restore CS8618

    /// <summary>
    /// Initializes a new instance of the Item class.
    /// <br/><br/>
    /// <i>Author(s): DefinitelyRus<br/>
    /// Editor(s): Google Antigravity<br/>
    /// Documented by: Google Gemini, Google Antigravity</i>
    /// </summary>
    /// <param name="id">The unique identifier for this entity.</param>
    /// <param name="unitOfMeasurement">The unit used for quantity.</param>
    /// <param name="isForSale">Whether the item can be sold.</param>
    /// <param name="price">The selling price.</param>
    /// <param name="name">The name of the item.</param>
    /// <param name="description">The item's description.</param>
    /// <param name="tags">A list of classification tags.</param>
    /// <param name="quantity">Initial stock quantity.</param>
    /// <param name="expirationDate">Optional expiration date.</param>
    /// <param name="cost">The acquisition cost.</param>
    /// <param name="imageUrl">The URL for the item's image.</param>
    public Item(
        ulong id,
        Unit unitOfMeasurement,
        bool isForSale,
        float? price = null,
        string? name = null,
        string? description = null,
        List<Tag>? tags = null,
        float quantity = 0,
        DateTime? expirationDate = null,
        float? cost = null,
        string? imageUrl = null)
    {
        Id = id;
        Name = name ?? $"Item #{Id}";
        Description = description;
        Tags = tags ?? [];
        Quantity = quantity;
        UnitOfMeasurement = unitOfMeasurement;
        ExpirationDate = expirationDate;
        Cost = cost;
        IsForSale = isForSale;
        ImageUrl = imageUrl;
        SalePrice = price;
    }
}
