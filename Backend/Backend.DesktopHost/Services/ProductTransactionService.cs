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
        // ─── Checks And Assignments ──────────────────────────────────────────
        Product? product = await _context.Products.FindAsync(request.ProductId)
            ?? throw Esc.New<ArgumentException>($"Product does not exist.", request.ProductId);

        product.TotalQuantity += request.Quantity;

        ProductBatch newBatch = new()
        {
            Product = product,
            Quantity = request.Quantity,
            Cost = request.Cost ?? product.Cost,
            ExpirationDate = request.ExpiryDate
        };

        ProductTransaction transaction = new()
        {
            Product = product,
            Batch = newBatch,
            Quantity = request.Quantity,
            TransactionType = TransactionType.Purchase,
            Timestamp = DateTime.UtcNow
        };

        // ─── Apply Changes To Database ───────────────────────────────────────
        await _context.ProductBatches.AddAsync(newBatch);
        await _context.ProductTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }


    public async Task<IEnumerable<ProductTransaction>> UpdateStockAsync(UpdateStockRequest request)
    {
        // ─── Checks And Assignments ──────────────────────────────────────────
        if (request.QuantityChange == 0) throw Esc.New<ArgumentException>($"Request quantity change must not be zero.", request.QuantityChange);

        Product? product = await _context.Products.FindAsync(request.ProductId)
            ?? throw Esc.New<ArgumentException>($"Product does not exist.", request.ProductId);

        ProductTransactionMethod method =
            request.TransactionMethod ??            // Use request override
            product.TransactionMethod ??            // Use product override
            ProductTransactionMethod.TotalFirst;    // Use system default
                                                    // TODO: Replace with system default.

        TransactionType type = request.TransactionType;

        List<ProductBatch> allBatches = [.._context.ProductBatches
            .Where(b => b.Product.Id == request.ProductId)];

        List<ProductTransaction> transactions = [];

        // Distribute to helper methods
        switch (method)
        {
            // ─── FEFO ────────────────────────────────────────────────────────
            case ProductTransactionMethod.Fefo:
                if (type == TransactionType.Purchase)
                    throw Esc.New<ArgumentException>($"Stocking method `{nameof(TransactionType.Purchase)}` does not allow adding new stock. Please use `CreateStockAsync` instead.");

                if (type != TransactionType.Sale) goto TotalFirst;

                try
                {
                    transactions = [.. await ApplyChangesFefo(request, product, allBatches, method)];
                }
                catch (ArgumentException e) { throw e.AddData(request.TransactionMethod); }
                catch (Exception) { throw; }

                break;

            // ─── Total-First / Total-Only ────────────────────────────────────
            case ProductTransactionMethod.TotalFirst:
            case ProductTransactionMethod.TotalOnly:
            TotalFirst:
                ProductTransaction transaction;
                try
                {
                    transaction = await ApplyChangeTotal(request, product, method);
                }
                catch (ArgumentException e) { throw e.AddData(request.TransactionMethod); }
                catch (Exception) { throw; }


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
                throw Esc.New<ArgumentException>($"Invalid stocking method.", method);
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
        // ─── Assignments ─────────────────────────────────────────────────────
        decimal remaining = request.QuantityChange;
        int batchIndex = 0;
        List<ProductBatch> sortedBatch = [.. allBatches.OrderBy(b => b.ExpirationDate)];
        List<ProductTransaction> transactions = [];

        // ─── Apply New Batch Quantities ──────────────────────────────────────
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

        // Throw if not all requested quantities consumed
        if (remaining != 0) throw Esc.New<InvalidOperationException>($"Remaining change quantity is non-zero after applying quantity changes to all affected batches.", remaining);

        // ─── Apply New Total Quantity ────────────────────────────────────────
        try
        {
            product.TotalQuantity = ApplyChangeByType(
                request.TransactionType,
                product.TotalQuantity,
                request.QuantityChange
                );
        }
        catch (ArgumentException e) { throw e.AddData(request.TransactionMethod); }
        catch (Exception) { throw; }

        return transactions;
    }

    private static async Task<ProductTransaction> ApplyChangeTotal(
        UpdateStockRequest request,
        Product product,
        ProductTransactionMethod method
        )
    {
        // ─── Apply New Total Quantity ────────────────────────────────────────
        try
        {
            product.TotalQuantity = ApplyChangeByType(
                request.TransactionType,
                product.TotalQuantity,
                request.QuantityChange
                );
        }
        catch (ArgumentException e) { throw e.AddData(request.TransactionMethod); }
        catch (Exception) { throw; }

        // ─── Create New Product Transaction Per Batch ────────────────────────
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
        // ─── Checks And Assignments ──────────────────────────────────────────
        List<ProductBatch> affectedBatches = request.BatchChanges is not null
            ? [.. allBatches.Where(b => request.BatchChanges.ContainsKey(b.BatchNumber))]
            : [];

        Dictionary<long, decimal>? changes = request.BatchChanges;
        if (affectedBatches.Count == 0 || changes == null)
            throw Esc.New<ArgumentException>($"Stocking method {nameof(ProductTransactionMethod.Manual)} requires at least 1 batch change.");

        decimal sum = changes.Values.Sum();
        if (sum != request.QuantityChange)
            throw Esc.New<ArgumentException>($"Quantity change does not match batch sum.")
            .AddData(request.QuantityChange)
            .AddData(sum);

        List<ProductTransaction> transactions = [];

        // ─── Apply New Batch Quantities ──────────────────────────────────────
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
            catch (ArgumentException e) { throw e.AddData(request.TransactionMethod); }
            catch (Exception) { throw; }

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

        // ─── Apply New Total Quantity ────────────────────────────────────────
        try
        {
            product.TotalQuantity = ApplyChangeByType(
                request.TransactionType,
                product.TotalQuantity,
                request.QuantityChange
                );
        }
        catch (ArgumentException e) { throw e.AddData(request.TransactionMethod); }
        catch (Exception) { throw; }

        return transactions;
    }

    private static decimal ApplyChangeByType(
        TransactionType type,
        decimal original,
        decimal change
        )
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
                    throw Esc.New<ArgumentException>($"Quantity value ({change}) must be a positive number.")
                        .AddData(change)
                        .AddData(type);

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
                    throw Esc.New<ArgumentException>($"Quantity value ({change}) must be a positive number.")
                        .AddData(change)
                        .AddData(type);

                decimal diff = Math.Min(newQuantity, change);
                newQuantity -= diff;
                break;

            default:
                throw Esc.New<ArgumentException>($"Invalid transaction type.", type);
        }

        return newQuantity;
    }

    #endregion

}