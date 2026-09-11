namespace Backend.Core.Models;

public enum QuantityType
{
    Pieces,     // Individual units (e.g., apples, bottles)
    Weight,     // Mass-based (e.g., mg, g, kg, lb, oz)
    Volume,     // Volume-based (e.g., ml, L, gal)
    Length,     // Length-based (e.g., m, cm, mm, ft)
    Area,       // Area-based (e.g., sq m, sq ft)
    Time,       // Time-based (e.g., seconds, minutes, hours, days, months, years)
    Data,       // Data-based (e.g., bits, bytes, KB, MB, GB, TB, PB, EB, ZB, YB)
    Percentage, // Percentage-based (e.g., 1%, 10%, 100%)
    Frequency,  // Frequency-based (e.g., Hz, kHz, MHz, GHz)
    Energy,     // Energy-based (e.g., J, kJ, MJ, GJ)
    Rating,     // Rating-based (e.g., 1-5 stars, 1-100 scale)
    Unlimited,  // For non-depleting items
    Unknown,    // Fallback in case of missing info
    Other       // None of the above
}

public enum TransactionType
{
    /// <summary>
    /// Initial stock quantity for an existing product before EZBM was used. 
    /// </summary>
    Initial,
    /// <summary>
    /// Purchase a certain quantity of new stock.
    /// </summary>
    Purchase,
    /// <summary>
    /// Sell a certain quantity of stock.
    /// </summary>
    Sale,
    /// <summary>
    /// Set a new stock quantity for an existing product.
    /// </summary>
    SetTo,
    /// <summary>
    /// Adjust a stock quantity by a specific amount.
    /// </summary>
    AdjustBy
}