using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Tools;
using Microsoft.EntityFrameworkCore;

namespace EZBM.Core.Services;

/// <summary>
/// Provides services for managing sales transactions and sale entries.
/// </summary>
public static class SalesService
{
    #region Sale Requests

    /// <summary>
    /// Creates a new sale and updates inventory.
    /// </summary>
    /// <param name="request">The request parameters containing sale details.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult<ulong>> CreateSaleAsync(
        CreateSaleRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();

            Staff? staff = await context.Staff.FindAsync(request.StaffId);
            if (staff is null)
            {
                message = $"Staff with ID {request.StaffId} not found.";
                Log.Me(message);

                Utils.Result resultType = Utils.Result.Failed_NoResults;
                Utils.RequestResult<ulong> noStaffResult = new(
                    resultType, message, 0
                );

                return noStaffResult;
            }

            float discountPercentage = 0f;
            if (!string.IsNullOrEmpty(request.PromoCode))
            {
                string codeUpper = request.PromoCode.Trim().ToUpper();
                StoreSettings settings = SettingsService.LoadSettings();

                if (settings.PromoCodes != null && settings.PromoCodes.TryGetValue(codeUpper, out var promoInfo))
                {
                    if (DateTime.UtcNow > promoInfo.ExpirationDate.Date.AddDays(1).AddSeconds(-1))
                    {
                        message = $"Promo code {request.PromoCode} is invalid: the promotion has expired.";
                        Log.Me(message);
                        Utils.RequestResult<ulong> failResult = new(Utils.Result.Failed_InvalidQuery, message, 0);
                        return failResult;
                    }
                    discountPercentage = promoInfo.DiscountPercentage;
                }
                else if (codeUpper.Equals("FREEWEEK", StringComparison.OrdinalIgnoreCase))
                {
                    if (DateTime.UtcNow <= settings.StoreOpeningDate.AddDays(7))
                    {
                        discountPercentage = 100f;
                    }
                    else
                    {
                        message = "Promo code FREEWEEK is invalid: the opening week promotion has expired.";
                        Log.Me(message);
                        Utils.RequestResult<ulong> failResult = new(Utils.Result.Failed_InvalidQuery, message, 0);
                        return failResult;
                    }
                }
                else
                {
                    message = $"Promo code {request.PromoCode} is invalid or unrecognized.";
                    Log.Me(message);
                    Utils.RequestResult<ulong> failResult = new(Utils.Result.Failed_InvalidQuery, message, 0);
                    return failResult;
                }
            }

            float finalAmount = request.TotalAmount * (1f - discountPercentage / 100f);
            if (finalAmount < 0f) finalAmount = 0f;

            if (request.PaymentMethod == Transaction.PayMethod.Mixed)
            {
                if (request.SplitPayments is null || request.SplitPayments.Count == 0)
                {
                    message = "Cannot checkout: Mixed payment selected but no split payment items provided.";
                    Log.Me(message);
                    Utils.RequestResult<ulong> failResult = new(Utils.Result.Failed_InvalidQuery, message, 0);
                    return failResult;
                }
                float sum = request.SplitPayments.Sum(s => s.Amount);
                if (Math.Abs(sum - finalAmount) > 0.01f)
                {
                    message = $"Cannot checkout: Split payments sum (${sum}) does not match discounted total amount (${finalAmount}).";
                    Log.Me(message);
                    Utils.RequestResult<ulong> failResult = new(Utils.Result.Failed_InvalidQuery, message, 0);
                    return failResult;
                }
            }

            Customer? customer = null;
            if (request.CustomerId is not null && request.CustomerId > 0)
            {
                customer = await context.Customer.FindAsync(request.CustomerId.Value);
                if (customer is null)
                {
                    message = $"Customer with ID {request.CustomerId} not found.";
                    Log.Me(message);
                    Utils.RequestResult<ulong> failResult = new(Utils.Result.Failed_NoResults, message, 0);
                    return failResult;
                }
            }

            DateTime serverTime = DateTime.UtcNow;
            int invoiceNumber = Utils.GenerateInvoiceNumber(serverTime);

            Sale sale = new(
                id: Utils.GenerateEntityId(),
                invoiceNumber: invoiceNumber,
                amount: finalAmount,
                paymentMethod: request.PaymentMethod,
                staff: staff,
                timestamp: serverTime,
                notes: request.Notes
            )
            {
                Customer = customer
            };

            context.Sale.Add(sale);

            if (request.PaymentMethod == Transaction.PayMethod.Mixed && request.SplitPayments is not null)
            {
                foreach (SplitPaymentRequest split in request.SplitPayments)
                {
                    Transaction childTx = new(
                        id: Utils.GenerateEntityId(),
                        transactionType: Transaction.Type.Income,
                        amount: split.Amount,
                        timestamp: serverTime,
                        staff: staff,
                        paymentMethod: split.PaymentMethod,
                        invoiceNumber: invoiceNumber,
                        invoicePrefix: "SALE-SPLIT",
                        notes: $"Split payment component of Invoice {sale.InvoiceId}"
                    );
                    childTx.ParentTransactionId = sale.Id;
                    childTx.Customer = customer;
                    context.Transaction.Add(childTx);
                }
            }

            foreach (SaleItemRequest itemReq in request.Items)
            {
                Item? item = await context.Item.FindAsync(itemReq.ItemId);
                if (item is null)
                {
                    message = $"Item with ID {itemReq.ItemId} not found.";
                    Log.Me(message);

                    Utils.Result resultType = Utils.Result.Failed_NoResults;
                    Utils.RequestResult<ulong> noItemResult = new(
                        resultType, message, 0
                    );

                    return noItemResult;
                }

                float subtotal = itemReq.Quantity * itemReq.UnitPrice;

                SaleEntry entry = new(
                    sale: sale,
                    item: item,
                    quantity: itemReq.Quantity,
                    unitPrice: itemReq.UnitPrice,
                    subtotal: subtotal
                );

                context.SaleEntry.Add(entry);

                if (item is Product && item.Quantity != -1f)
                {
                    item.Quantity -= itemReq.Quantity;
                }

                if (item is Service && (item.Name.Contains("Upgrade", StringComparison.OrdinalIgnoreCase) || item.Name.Contains("Grooming", StringComparison.OrdinalIgnoreCase)))
                {
                    StoreSettings storeSettings = SettingsService.LoadSettings();
                    float commissionRate = staff.CommissionRate ?? 1.0f;
                    if (storeSettings.MembershipCommissions.TryGetValue(item.Name, out float baseCommission))
                    {
                        float finalCommission = baseCommission * commissionRate;
                        StaffAdjustment adjustment = new(
                            id: Utils.GenerateEntityId(),
                            staffId: staff.Id,
                            adjustmentType: "Commission",
                            amount: finalCommission,
                            deductFromCurrentPayroll: false,
                            isPaid: false,
                            timestamp: serverTime,
                            notes: $"Commission earned from {item.Name} sold in Invoice {sale.InvoiceId}"
                        );
                        context.StaffAdjustment.Add(adjustment);
                    }
                }

                ItemTransaction itemTransaction = new(
                    item: item,
                    transactionType: ItemTransaction.Type.Sale,
                    saleEntry: entry,
                    quantity: itemReq.Quantity,
                    staff: staff,
                    timestamp: serverTime,
                    note: $"Checkout for Invoice {sale.InvoiceId}"
                );

                context.ItemTransaction.Add(itemTransaction);
            }

            await context.SaveChangesAsync();

            message = $"Sale registered successfully with Invoice {sale.InvoiceId}.";
            Log.Me(message);

            Utils.Result successType = Utils.Result.Success;
            Utils.RequestResult<ulong> successResult = new(
                successType, message, sale.Id
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when creating sale transaction: {ex.Message}";
            Log.Me(message);

            Utils.Result resultType =
                Utils.Result.Failed_UnhandledException;

            Utils.RequestResult<ulong> errorResult = new(
                resultType, message, 0
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Retrieves a specific sale record by identifier.
    /// </summary>
    /// <param name="request">The request containing the sale ID.</param>
    /// <returns>A RequestResult containing the Sale entity.</returns>
    public static async Task<Utils.RequestResult<Sale>> GetSaleAsync(
        GetSaleRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Sale? sale = await context.Sale
                .Include(s => s.Staff)
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(
                    s => s.Id == request.Id
                );

            if (sale is null)
            {
                message = $"Sale with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult<Sale> failResult = new(
                    Utils.Result.Failed_NoResults, message, null
                );

                return failResult;
            }

            message = $"Sale with ID {request.Id} found successfully.";
            Log.Me(message);
            Utils.RequestResult<Sale> successResult = new(
                Utils.Result.Success, message, sale
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when getting sale with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult<Sale> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, null
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Finds sale records matching query filters.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>A RequestResult containing the list of matching Sales.</returns>
    public static async Task<Utils.RequestResult<List<Sale>>> FindSalesAsync(
        FindSaleRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            IQueryable<Sale> query = context.Sale
                .Include(s => s.Staff)
                .Include(s => s.Customer);

            if (request is not null)
            {
                if (request.Id is not null)
                {
                    string requestIdStr = request.Id.Value.ToString();
                    query = query.Where(
                        s => s.Id.ToString().Contains(requestIdStr)
                    );
                }

                if (request.InvoiceNumber is not null)
                    query = query.Where(
                        s => s.InvoiceNumber == request.InvoiceNumber
                    );

                if (request.StaffId is not null)
                    query = query.Where(
                        s => s.Staff.Id == request.StaffId
                    );

                if (request.MinTimestamp is not null)
                    query = query.Where(
                        s => s.Timestamp >= request.MinTimestamp
                    );

                if (request.MaxTimestamp is not null)
                    query = query.Where(
                        s => s.Timestamp <= request.MaxTimestamp
                    );

                if (request.PaymentMethod is not null)
                    query = query.Where(
                        s => s.PaymentMethod == request.PaymentMethod
                    );

                if (request.MinAmount is not null)
                    query = query.Where(
                        s => s.Amount >= request.MinAmount
                    );

                if (request.MaxAmount is not null)
                    query = query.Where(
                        s => s.Amount <= request.MaxAmount
                    );

                if (request.CustomerId is not null)
                    query = query.Where(
                        s => s.Customer != null && s.Customer.Id == request.CustomerId
                    );
            }

            query = Utils.ApplySortingAndPagination(query, request?.Limit, request?.Offset, request?.SortBy, request?.SortOrder);

            List<Sale> results = await query.ToListAsync();

            if (results.Count == 0)
            {
                message = "No sales records found matching the query.";
                Log.Me(message);
                Utils.RequestResult<List<Sale>> noResults = new(
                    Utils.Result.Success_NoResults, message, []
                );
                return noResults;
            }

            message = $"Found {results.Count} sales record(s) matching query.";
            Log.Me(message);
            Utils.RequestResult<List<Sale>> successResult = new(
                Utils.Result.Success, message, results
            );
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when finding sales records: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult<List<Sale>> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, []
            );
            return errorResult;
        }
    }

    /// <summary>
    /// Deletes a specific sale record.
    /// </summary>
    /// <param name="request">The request containing the sale ID to delete.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> DeleteSaleAsync(
        DeleteSaleRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Sale? sale = await context.Sale.FindAsync(request.Id);

            if (sale is null)
            {
                message = $"Sale with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult noResults = new(
                    Utils.Result.Failed_NoResults, message
                );

                return noResults;
            }

            context.Sale.Remove(sale);
            await context.SaveChangesAsync();

            message = $"Sale with ID {sale.Id} deleted successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when deleting sale with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    #endregion

    #region Sale Entry Requests

    /// <summary>
    /// Creates a manual sale entry.
    /// </summary>
    /// <param name="request">The request containing sale entry details.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> CreateSaleEntryAsync(
        CreateSaleEntryRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();

            Sale? sale = await context.Sale.FindAsync(request.SaleId);
            if (sale is null)
            {
                message = $"Sale with ID {request.SaleId} not found.";
                Log.Me(message);
                Utils.RequestResult noSaleResult = new(
                    Utils.Result.Failed_NoResults, message
                );

                return noSaleResult;
            }

            Item? item = await context.Item.FindAsync(request.ItemId);
            if (item is null)
            {
                message = $"Item with ID {request.ItemId} not found.";
                Log.Me(message);
                Utils.RequestResult noItemResult = new(
                    Utils.Result.Failed_NoResults, message
                );

                return noItemResult;
            }

            SaleEntry entry = new(
                sale: sale,
                item: item,
                quantity: request.Quantity,
                unitPrice: request.UnitPrice,
                subtotal: request.Subtotal
            );

            context.SaleEntry.Add(entry);
            await context.SaveChangesAsync();

            message = $"Sale entry created successfully for Sale ID {sale.Id}.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when creating sale entry: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Retrieves a specific sale entry record.
    /// </summary>
    /// <param name="request">The request containing the sale entry ID.</param>
    /// <returns>A RequestResult containing the SaleEntry entity.</returns>
    public static async Task<Utils.RequestResult<SaleEntry>> GetSaleEntryAsync(
        GetSaleEntryRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            SaleEntry? entry = await context.SaleEntry
                .Include(se => se.Sale)
                .Include(se => se.Item)
                .FirstOrDefaultAsync(
                    se => se.Id == request.Id
                );

            if (entry is null)
            {
                message = $"Sale entry with ID {request.Id} not found.";
                Log.Me(message);
                Utils.RequestResult<SaleEntry> failResult = new(
                    Utils.Result.Failed_NoResults, message, null
                );

                return failResult;
            }

            message = $"Sale entry with ID {request.Id} found successfully.";
            Log.Me(message);
            Utils.RequestResult<SaleEntry> successResult = new(
                Utils.Result.Success, message, entry
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when getting sale entry with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult<SaleEntry> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, null
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Finds sale entries matching query filters.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>A RequestResult containing the list of matching SaleEntries.</returns>
    public static async Task<Utils.RequestResult<List<SaleEntry>>> FindSaleEntriesAsync(
        FindSaleEntryRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            IQueryable<SaleEntry> query = context.SaleEntry
                .Include(se => se.Sale)
                .Include(se => se.Item);

            if (request is not null)
            {
                if (request.Id is not null)
                {
                    string requestIdStr = request.Id.Value.ToString();
                    query = query.Where(
                        se => se.Id.ToString().Contains(requestIdStr)
                    );
                }

                if (request.SaleId is not null)
                    query = query.Where(
                        se => se.Sale.Id == request.SaleId
                    );

                if (request.ItemId is not null)
                    query = query.Where(
                        se => se.Item.Id == request.ItemId
                    );

                if (request.MinQuantity is not null)
                    query = query.Where(
                        se => se.Quantity >= request.MinQuantity
                    );

                if (request.MaxQuantity is not null)
                    query = query.Where(
                        se => se.Quantity <= request.MaxQuantity
                    );

                if (request.MinUnitPrice is not null)
                    query = query.Where(
                        se => se.UnitPrice >= request.MinUnitPrice
                    );

                if (request.MaxUnitPrice is not null)
                    query = query.Where(
                        se => se.UnitPrice <= request.MaxUnitPrice
                    );
            }

            query = Utils.ApplySortingAndPagination(query, request?.Limit, request?.Offset, request?.SortBy, request?.SortOrder);

            List<SaleEntry> results = await query.ToListAsync();

            if (results.Count == 0)
            {
                message = "No sale entries matching query found.";
                Log.Me(message);
                Utils.RequestResult<List<SaleEntry>> noResults = new(
                    Utils.Result.Success_NoResults, message, []
                );

                return noResults;
            }

            message = $"Found {results.Count} sale entry/entries matching query.";
            Log.Me(message);
            Utils.RequestResult<List<SaleEntry>> successResult = new(
                Utils.Result.Success, message, results
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when finding sale entries: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult<List<SaleEntry>> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, []
            );

            return errorResult;
        }
    }

    /// <summary>
    /// Deletes a specific sale entry record.
    /// </summary>
    /// <param name="request">The request containing the sale entry ID to delete.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> DeleteSaleEntryAsync(
        DeleteSaleEntryRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            SaleEntry? entry = await context.SaleEntry.FindAsync(request.Id);

            if (entry is null)
            {
                message = $"Sale entry with ID {request.Id} not found.";
                Log.Me(message);
                Utils.RequestResult noResults = new(
                    Utils.Result.Failed_NoResults, message
                );

                return noResults;
            }

            context.SaleEntry.Remove(entry);
            await context.SaveChangesAsync();

            message = $"Sale entry with ID {entry.Id} deleted successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when deleting sale entry with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message
            );

            return errorResult;
        }
    }

    #endregion
}
