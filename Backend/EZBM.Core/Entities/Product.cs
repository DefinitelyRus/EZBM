using System;
using System.Collections.Generic;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents a physical product in inventory with stock levels and warning thresholds.
/// </summary>
public class Product : Item
{
    /// <summary>
    /// The target stock quantity for the product.
    /// </summary>
    public float TargetStock { get; set; }

    /// <summary>
    /// The warning threshold percentage for low stock alert.
    /// </summary>
    public float LowStockThresholdPercentage { get; set; } = 0.20f;

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    protected Product() : base() { }

    /// <summary>
    /// Initializes a new instance of the Product class.
    /// </summary>
    public Product(
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
        string? imageUrl = null,
        string? barcode = null,
        float targetStock = 0,
        float lowStockThresholdPercentage = 0.20f)
        : base(id, unitOfMeasurement, isForSale, price, name, description, tags, quantity, expirationDate, cost, imageUrl, barcode)
    {
        TargetStock = targetStock;
        LowStockThresholdPercentage = lowStockThresholdPercentage;
    }
}
