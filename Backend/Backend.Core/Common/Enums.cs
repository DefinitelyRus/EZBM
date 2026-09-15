namespace Backend.Core.Common;

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
    /// Initial stock quantity for an existing product before EZBM was used. <br/><br/>
    /// <b>Important:</b> Only one transaction of this type must exist per product.
    /// EZBM is programmed to warn about multiple <see cref="Initial">Initial</see>
    /// transaction types under the same product; <see cref="SetTo">SetTo</see> can
    /// be used to correct quantities if needed. <br/>
    /// By extension, do not modify the <see cref="Quantity">quantity</see> of
    /// an existing <see cref="Initial">Initial</see> (or any other) transaction; 
    /// create a new transaction instead. 
    /// </summary>
    /// <remarks>
    /// This is functionally the same as <see cref="SetTo">SetTo</see>.
    /// Its purpose is to distinguish between first-time tracking and quantity corrections. 
    /// </remarks>
    Initial, // SET

    /// <summary>
    /// Purchase a certain quantity of new stock.
    /// </summary>
    /// <remarks>
    /// This is functionally the same as <see cref="AdjustBy">AdjustBy</see>.
    /// Its purpose is to distinguish between new stock and quantity corrections.
    /// </remarks>
    Purchase, // ADD

    /// <summary>
    /// Return previously sold stock to the inventory.
    /// </summary>
    /// <remarks>
    /// This is functionally the same as <see cref="AdjustBy">AdjustBy</see>.
    /// Its purpose is to distinguish between returned stock and quantity corrections.
    /// </remarks>
    ReturnToStock, // ADD

    /// <summary>
    /// Sell a certain quantity of stock.
    /// </summary>
    /// <remarks>
    /// This is functionally the same as <see cref="AdjustBy">AdjustBy</see>, but negative.
    /// Its purpose is to distinguish between sold stock and quantity corrections. 
    /// </remarks>
    Sale, // SUBTRACT

    /// <summary>
    /// Return previously purchased stock to the supplier.
    /// </summary>
    /// <remarks>
    /// This is functionally the same as <see cref="AdjustBy">AdjustBy</see>, but negative.
    /// Its purpose is to distinguish between returned stock and quantity corrections.
    /// </remarks>
    ReturnToSupplier, // SUBTRACT

    /// <summary>
    /// Set a new stock quantity for an existing product.
    /// </summary>
    /// <remarks>
    /// This is functionally the same as <see cref="Initial">Initial</see>.
    /// Its purpose is to distinguish between first-time tracking and quantity corrections. 
    /// </remarks>
    SetTo, // SET

    /// <summary>
    /// Adjust a stock quantity by a specific amount.
    /// </summary>
    /// <remarks>
    /// This is functionally the same as <see cref="Purchase">Purchase</see>.
    /// Its purpose is to distinguish between new stock and quantity corrections. 
    /// </remarks>
    AdjustBy // ADD
}


public enum ProductTransactionMethod
{
    /// <summary>
    /// First-Expired-First-Out. <br/><br/>
    /// - If the transaction type is a <see cref="TransactionType.Sale">sale</see>,
    /// the oldest batch is deducted from first. <br/>
    /// - If it's a <see cref="TransactionType.Purchase">purchase</see>, a new batch
    /// is created. <br/>
    /// - For other <see cref="TransactionType">transaction types</see>, it will function
    /// the same as <see cref="TotalFirst">total-first</see>.
    /// </summary>
    Fefo,

    /// <summary>
    /// Update the product's total quantity immediately, but defer batch synchronization for later.
    /// </summary>
    /// <remarks>
    /// This is functionally the same as <see cref="TotalOnly">total-only</see> when
    /// the transaction is being recorded. However, unlike <see cref="TotalOnly">total-only</see>,
    /// if the <see cref="Product.TransactionMethod">product's transaction method</see>
    /// is set to total-first, the user will eventually be notified when there is
    /// a mismatch between the <see cref="Models.Product.TotalQuantity">product's
    /// total quantity</see> and the sum of its 
    /// <see cref="Models.ProductBatch.Quantity">batches' quantities</see>. 
    /// </remarks>
    TotalFirst,

    /// <summary>
    /// Update the product's total quantity but do not touch the batches. <br/><br/>
    /// This is useful for products that don't expire or have no reason to be tracked
    /// by batch, or for users who simply don't care about batch tracking.
    /// </summary>
    TotalOnly,

    /// <summary>
    /// Require the user to specify the exact batch(es) involved and the exact quantities
    /// said batches are adjusted by.
    /// </summary>
    Manual
}
