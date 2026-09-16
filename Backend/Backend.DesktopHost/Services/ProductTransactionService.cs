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
        Product? product = await _context.Products.FindAsync(request.ProductId) ?? throw new ArgumentException($"Product with ID '{request.ProductId}' does not exist.");

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
        if (request.QuantityChange == 0) throw new ArgumentException($"Request quantity change must not be zero.");

        Product? product = await _context.Products.FindAsync(request.ProductId) ?? throw new ArgumentException($"Product with ID '{request.ProductId}' does not exist.");

        ProductTransactionMethod method =
            request.TransactionMethod ??            // Use request override
            product.TransactionMethod ??            // Use product override
            ProductTransactionMethod.TotalFirst;    // Use system default
                                                    // TODO: Replace with system default.

        TransactionType type = request.TransactionType;

        List<ProductBatch> allBatches = [.._context.ProductBatches
            .Where(b => b.Product.Id == request.ProductId)];

        List<ProductTransaction> transactions = [];

        switch (method)
        {
            // ─── FEFO ────────────────────────────────────────────────────────
            case ProductTransactionMethod.Fefo:
                if (type == TransactionType.Purchase)
                    throw new ArgumentException($"Stocking method `{TransactionType.Purchase}` does not allow adding new stock. Please use `CreateStockAsync` instead.");

                if (type != TransactionType.Sale) goto TotalFirst;

                try
                {
                    transactions = [.. await ApplyChangesFefo(request, product, allBatches, method)];
                }
                catch (Exception e) { throw new Exception($"{e.Message} TransactionMethod={request.TransactionMethod}"); }
                // TODO: Complete exception handling
                
                break;

            // ─── Total-First / Total-Only ────────────────────────────────────
            case ProductTransactionMethod.TotalFirst:
            case ProductTransactionMethod.TotalOnly:
            TotalFirst:
                ProductTransaction transaction = await ApplyChangeTotal(request, product, method);
                // TODO: Pass exceptions from ApplyChangeTotal


                await _context.ProductTransactions.AddAsync(transaction);
                transactions.Add(transaction);

                break;

            // ─── Manual ──────────────────────────────────────────────────────
            case ProductTransactionMethod.Manual:
                transactions = [.. await ApplyChangesManual(request, product, allBatches, method)];
                // TODO: Pass exceptions from ApplyChangesManual

                break;

            // ─── Invalid ─────────────────────────────────────────────────────
            default:
                throw new ArgumentException($"Invalid stocking method: {method} | ");
        }

        await _context.ProductTransactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
        return transactions;
    }

    #region Helpers

    private static async Task<IEnumerable<ProductTransaction>> ApplyChangesFefo(
        UpdateStockRequest request,
        Product product,
        IEnumerable<ProductBatch> allBatches,
        ProductTransactionMethod method
        )
    {
        decimal remaining = request.QuantityChange;
        int batchIndex = 0;
        List<ProductBatch> sortedBatch = [.. allBatches.OrderBy(b => b.ExpirationDate)];
        List<ProductTransaction> transactions = [];

        while (remaining > 0 && batchIndex < sortedBatch.Count)
        {
            decimal diff = Math.Min(remaining, sortedBatch[batchIndex].Quantity);

            remaining -= diff;
            sortedBatch[batchIndex].Quantity -= diff;

            ProductTransaction transaction = new()
            {
                Product = product,
                Batch = sortedBatch[batchIndex],
                Quantity = diff,
                TransactionType = request.TransactionType,
                OverrideMethod = method,
                Timestamp = DateTime.UtcNow // TODO: Add as property in UpdateStockRequest
            };

            transactions.Add(transaction);
            // TODO: Find a way to cancel all pending database changes when an exception is thrown. 
            // The current system relies on thrown exceptions to cascade all the
            // way up to the controller to prevent cancelled saved changes. This
            // is fine for now, but is prone to data-impacting bugs.

            batchIndex++;
        }

        if (remaining != 0) throw new InvalidOperationException($"Remaining change quantity is non-zero after applying quantity changes to all affected batches. | remaining={remaining}");
        // TODO: Create a custom exception message generator.

        try
        {
            product.TotalQuantity = ApplyChangeByType(
                request.TransactionType,
                product.TotalQuantity,
                request.QuantityChange
                );
        }
        catch (ArgumentException e) { throw new ArgumentException($"{e.Message} TransactionMethod={request.TransactionMethod}"); }
        catch (Exception e) { throw new Exception($"{e.Message} | "); }

        return transactions;
    }

    private static async Task<ProductTransaction> ApplyChangeTotal(
        UpdateStockRequest request,
        Product product,
        ProductTransactionMethod method
        )
    {
        try
        {
            product.TotalQuantity = ApplyChangeByType(
                request.TransactionType,
                product.TotalQuantity,
                request.QuantityChange
                );
        }
        catch (ArgumentException e) { throw new ArgumentException($"{e.Message} TransactionMethod={request.TransactionMethod}"); }
        catch (Exception e) { throw new Exception($"{e.Message} | "); }

        // Create new product transaction per batch
        ProductTransaction transaction = new()
        {
            Product = product,
            Batch = null,
            Quantity = request.QuantityChange,
            TransactionType = request.TransactionType,
            OverrideMethod = method,
            Timestamp = DateTime.UtcNow // TODO: Replace with property from UpdateStockRequest
        };

        return transaction;
    }

    private static async Task<IEnumerable<ProductTransaction>> ApplyChangesManual(
        UpdateStockRequest request,
        Product product,
        IEnumerable<ProductBatch> allBatches,
        ProductTransactionMethod method
        )
    {
        List<ProductBatch> affectedBatches = request.BatchChanges is not null
            ? [.. allBatches.Where(b => request.BatchChanges.ContainsKey(b.BatchNumber))]
            : [];

        Dictionary<long, decimal>? changes = request.BatchChanges;
        if (affectedBatches.Count == 0 || changes == null) throw new ArgumentException($"Stocking method {ProductTransactionMethod.Manual} requires at least 1 batch change.");

        decimal sum = changes.Values.Sum();
        if (sum != request.QuantityChange) throw new ArgumentException($"Quantity change ({request.QuantityChange}) does not match batch sum ({sum}).");

        List<ProductTransaction> transactions = [];

        foreach (KeyValuePair<long, decimal> change in changes)
        {
            ProductBatch batch = affectedBatches.Find(b => b.BatchNumber == change.Key)!;

            try
            {
                batch.Quantity = ApplyChangeByType(
                    request.TransactionType,
                    batch.Quantity,
                    change.Value
                    );
            }
            catch (ArgumentException e) { throw new ArgumentException($"{e.Message} TransactionMethod={request.TransactionMethod}"); }
            catch (Exception e) { throw new Exception($"{e.Message} | "); }

            transactions.Add(new()
            {
                Product = product,
                Batch = batch,
                Quantity = change.Value,
                TransactionType = request.TransactionType,
                OverrideMethod = method,
                Timestamp = DateTime.UtcNow // TODO: Replace with property from UpdateStockRequest
            });
        }

        product.TotalQuantity = request.QuantityChange;

        return transactions;
    }

    private static decimal ApplyChangeByType(TransactionType type, decimal original, decimal change)
    {
        decimal newQuantity = original;

        switch (type)
        {
            // ─── Set ─────────────────────────────────────────────────
            case TransactionType.Initial:
            case TransactionType.SetTo:
                newQuantity = change;
                break;

            // ─── Add ─────────────────────────────────────────────────
            case TransactionType.Purchase:
            case TransactionType.ReturnToStock:
                if (change <= 0)
                    throw new ArgumentException($"Quantity value ({change}) must be a positive number. | TransactionType={type}");

                newQuantity += change;
                break;

            // Allow changes
            case TransactionType.AdjustBy:
                newQuantity += change;
                break;

            // ─── Subtract ────────────────────────────────────────────
            case TransactionType.Sale:
            case TransactionType.ReturnToSupplier:
                if (change <= 0)
                    throw new ArgumentException($"Quantity value ({change}) must be a positive number. | TransactionType={type}");

                decimal diff = Math.Min(newQuantity, change);
                newQuantity -= diff;
                break;

            default:
                throw new ArgumentException($"Invalid transaction type: {type} | ");
        }

        return newQuantity;
    }

    #endregion

}