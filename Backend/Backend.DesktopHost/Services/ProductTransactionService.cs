using Backend.Core.Common;
using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;

namespace Backend.DesktopHost.Services;

public class ProductTransactionService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<ProductTransaction> CreateStockAsync(CreateStockRequest request)
    {
        Product? product = await _context.Products.FindAsync(request.ProductId)
            ?? throw new ArgumentException($"Product with ID '{request.ProductId}' does not exist.");

        product.TotalQuantity += request.Quantity;

        ProductBatch newBatch = new()
        {
            Product = product,
            Quantity = request.Quantity,
            Cost = request.Cost ?? product.Cost,
            ExpirationDate = request.ExpiryDate
        };

        await _context.ProductBatches.AddAsync(newBatch);

        ProductTransaction transaction = new()
        {
            Product = product,
            Batch = newBatch,
            Quantity = request.Quantity,
            TransactionType = TransactionType.Purchase,
            Timestamp = DateTime.UtcNow
        };

        await _context.ProductTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }


    public async Task<IEnumerable<ProductTransaction>> UpdateStockAsync(UpdateStockRequest request)
    {
        Product? product = await _context.Products.FindAsync(request.ProductId)
            ?? throw new ArgumentException($"Product with ID '{request.ProductId}' does not exist.");

        ProductTransactionMethod method =
            request.TransactionMethod ??            // Use request override
            product.TransactionMethod ??            // Use product override
            ProductTransactionMethod.TotalFirst;    // Use system default
                                                    // TODO: Replace with system default.

        TransactionType type = request.TransactionType;

        List<ProductBatch> allBatches = [.._context.ProductBatches
            .Where(b => b.Product.Id == request.ProductId)];


        List<ProductBatch> affectedBatches = request.BatchChanges is not null
            ? [.. allBatches.Where(b => request.BatchChanges.ContainsKey(b.BatchNumber))]
            : [];

        List<ProductTransaction> transactions = [];

        switch (method)
        {
            case ProductTransactionMethod.Fefo:
                if (type == TransactionType.Purchase)
                    throw new ArgumentException($"Stocking method `{method}` does not allow adding new stock. Please use `CreateStockAsync` instead.");

                if (type != TransactionType.Sale) goto TotalFirst;

                List<ProductBatch> sortedBatch = [.. allBatches.OrderBy(b => b.ExpirationDate)];
                decimal remaining = request.QuantityChange;
                int batchIndex = 0;

                while (remaining > 0)
                {
                    decimal diff = Math.Clamp(
                            remaining - sortedBatch[batchIndex].Quantity,
                            0,
                            decimal.MaxValue
                        );

                    remaining -= diff;
                    sortedBatch[batchIndex].Quantity -= diff;
                    batchIndex++;

                    // Create new product transaction per batch
                    ProductTransaction transaction = new()
                    {
                        Product = product,
                        Batch = sortedBatch[batchIndex],
                        Quantity = diff,
                        TransactionType = type,
                        OverrideMethod = method,
                        Timestamp = DateTime.UtcNow
                    };

                    // Record transaction
                    await _context.ProductTransactions.AddAsync(transaction);
                    transactions.Add(transaction);
                }

                break;

            case ProductTransactionMethod.TotalFirst:
            TotalFirst:
                // TODO: Update records in TotalFirst method.
                break;

            case ProductTransactionMethod.TotalOnly:
                // TODO: Update records in TotalOnly method.
                break;

            case ProductTransactionMethod.Manual:
                // TODO: Update records in Manual method.
                break;

            default:
                throw new ArgumentException($"Stocking method {method} is invalid.");
        }

        return transactions;
    }
}





// public async Task OLD_ProcessTransactionAsync(CreateTransactionRequest request)
// {
//     #region Validation

//     if (request.TotalQuantityChange == 0)
//         throw new ArgumentException($"`TotalQuantityChange` must be non-zero.");

//     if (request.BatchAdjustments != null)
//     {
//         // Check if total and batch sum are equal
//         if (request.BatchAdjustments.Count > 0)
//         {
//             decimal sum = request.BatchAdjustments.Values.Sum();

//             if (sum != request.TotalQuantityChange)
//                 throw new ArgumentException($"`TotalQuantityChange` ({request.TotalQuantityChange}) does not match the sum of `BatchAdjustments` ({sum}).");
//         }

//         // Check if the listed batches exist
//         List<long> invalidIds = [];
//         foreach (long batchId in request.BatchAdjustments.Keys)
//         {
//             ProductBatch? batch = await _context.ProductBatches.FindAsync(batchId);

//             if (batch == null) invalidIds.Add(batchId);
//         }

//         if (invalidIds.Count > 0)
//             throw new ArgumentException($"Batches with the following IDs do not exist: {string.Join(", ", invalidIds)}");
//     }

//     #endregion

//     Product? product =
//         await _context.Products.FindAsync(request.ProductId) ??
//         throw new ArgumentException($"Product with ID '{request.ProductId}' does not exist.");

//     // Get all product batches with a matching ID.
//     ProductBatch[]? matchingBatches =
//         request.BatchAdjustments != null
//             ? [.. _context.ProductBatches
//                 .Where(b => request.BatchAdjustments.ContainsKey(b.Id))]
//             : null;

//     // Determine which method to use.
//     ProductTransactionMethod method =
//         request.OverrideMethod                  // Use request's override
//         ?? product.TransactionMethod            // Use product's override
//         ?? ProductTransactionMethod.TotalOnly;  // Use system default.
//                                                 // TODO: Replace with system default.

//     switch (method)
//     {
//         // Adjust the batch by expirty date.
//         // Oldest first for sales, newest first for purchases.
//         case ProductTransactionMethod.Fefo:
//             List<ProductBatch> sortedBatches = [.. _context.ProductBatches
//                 .Where(b => b.Product.Id == request.ProductId)
//                 .OrderBy(b => b.ExpirationDate)];

//             switch (request.TransactionType)
//             {
//                 case TransactionType.Purchase:
//                     // TODO: Create new batch
//                     _context.ProductBatches
//                         .Add(new ProductBatch());
//                     break;

//                 case TransactionType.Sale:
//                     // TODO: Update existing batches in order
//                     break;

//                 default:
//                     goto TotalFirst;
//             }

//             break;

//         // Apply the total quantity, then the batch adjustments if specified.
//         // This is separated from Manual as this allows for the absence of batch
//         // adjustments, whereas Manual requires them.
//         case ProductTransactionMethod.TotalFirst:
//         TotalFirst:

//             break;

//         // Apply total quantity, then the batch adjustments.
//         case ProductTransactionMethod.Manual:

//             break;

//         // Apply the total quantity only.
//         // This is separated from Manual and TotalFirst because the client could
//         // supply the batch adjustments even when they shouldn't be tracked, which
//         // means they'll actively need to be filtered out regardless.
//         case ProductTransactionMethod.TotalOnly:

//             break;

//         default:
//             throw new ArgumentException($"Invalid transaction method: {product.TransactionMethod}");
//     }

// }