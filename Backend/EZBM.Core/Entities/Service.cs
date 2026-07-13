using System;
using System.Collections.Generic;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents an unlimited digital offering or service.
/// </summary>
public class Service : Item
{
    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    protected Service() : base() { }

    /// <summary>
    /// Initializes a new instance of the Service class.
    /// </summary>
    public Service(
        ulong id,
        Unit unitOfMeasurement,
        bool isForSale,
        float? price = null,
        string? name = null,
        string? description = null,
        List<string>? tags = null,
        float quantity = 9999f,
        DateTime? expirationDate = null,
        float? cost = null,
        string? imageUrl = null,
        string? barcode = null)
        : base(id, unitOfMeasurement, isForSale, price, name, description, tags, quantity, expirationDate, cost, imageUrl, barcode, null)
    {
    }
}
