using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Tools;
using Microsoft.EntityFrameworkCore;

namespace EZBM.Core.Services;

/// <summary>
/// Provides services for managing items and stock transactions in the inventory.
/// </summary>
public static class InventoryService
{

    #region Item Requests

    /// <summary>
    /// Retrieves a specific inventory item by identifier.
    /// </summary>
    /// <param name="request">The request containing the item ID.</param>
    /// <returns>A RequestResult containing the Item entity.</returns>
    public static async Task<Utils.RequestResult<Item>> GetItemAsync(GetItemRequest request)
    {
        string message;

        try
        {
            using AppDbContext context = new();
            Item? item = await context.Item.FindAsync(request.Id);

            if (item is null)
            {
                message = $"Item with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult<Item> failResult = new(Utils.Result.Failed_NoResults, message, null);
                return failResult;
            }

            message = $"Item '{item.Name}' with ID {item.Id} found successfully.";
            Log.Me(message);
            Utils.RequestResult<Item> successResult = new(Utils.Result.Success, message, item);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when getting item with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult<Item> errorResult = new(Utils.Result.Failed_UnhandledException, message, null);
            return errorResult;
        }
    }


    /// <summary>
    /// Finds items matching the specified query filters.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>A RequestResult containing the list of matching Items.</returns>
    public static async Task<Utils.RequestResult<List<Item>>> FindItemAsync(
        FindItemRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            IQueryable<Item> query = context.Item;

            if (request is not null)
            {
                if (request.Id is not null)
                {
                    string requestId = request.Id.Value.ToString();
                    query = query.Where(
                        item => item.Id.ToString().Contains(requestId)
                    );
                }

                if (!string.IsNullOrEmpty(request.Name))
                    query = query.Where(
                        item => item.Name.Contains(request.Name)
                    );

                if (!string.IsNullOrEmpty(request.Description))
                    query = query.Where(
                        item => item.Description != null &&
                        item.Description.Contains(request.Description)
                    );

                if (request.IsForSale is not null)
                    query = query.Where(
                        item => item.IsForSale == request.IsForSale
                    );

                if (request.MinCost is not null)
                    query = query.Where(
                        item => item.Cost >= request.MinCost
                    );

                if (request.MaxCost is not null)
                    query = query.Where(
                        item => item.Cost <= request.MaxCost
                    );

                if (request.MinSalePrice is not null)
                    query = query.Where(
                        item => item.SalePrice >= request.MinSalePrice
                    );

                if (request.MaxSalePrice is not null)
                    query = query.Where(
                        item => item.SalePrice <= request.MaxSalePrice
                    );

                if (request.MinQuantity is not null)
                    query = query.Where(
                        item => item.Quantity >= request.MinQuantity
                    );

                if (request.MaxQuantity is not null)
                    query = query.Where(
                        item => item.Quantity <= request.MaxQuantity
                    );

                if (request.UnitOfMeasurement is not null)
                    query = query.Where(
                        item => item.UnitOfMeasurement == request.UnitOfMeasurement
                    );

                if (request.MinExpirationDate is not null)
                    query = query.Where(
                        item => item.ExpirationDate >= request.MinExpirationDate
                    );

                if (!string.IsNullOrEmpty(request.Barcode))
                    query = query.Where(
                        item => item.Barcode != null &&
                        item.Barcode.Contains(request.Barcode)
                    );
            }

            query = Utils.ApplySortingAndPagination(query, request?.Limit, request?.Offset, request?.SortBy, request?.SortOrder);

            List<Item> results = await query.ToListAsync();

            if (request?.Tags is { Count: > 0 } && results.Count > 0)
            {
                results = [.. results.Where(
                    item => item.Tags.Any(t => request.Tags.Contains(t))
                )];
            }

            if (results.Count == 0)
            {
                message = "No items matching query found in database.";
                Log.Me(message);
                Utils.RequestResult<List<Item>> noResults = new(
                    Utils.Result.Success_NoResults, message, []);
                return noResults;
            }

            message = $"Found {results.Count} items matching query.";
            Log.Me(message);
            Utils.RequestResult<List<Item>> successResult = new(
                Utils.Result.Success, message, results);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when finding items: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult<List<Item>> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, []);
            return errorResult;
        }
    }


    /// <summary>
    /// Updates an existing inventory item's details.
    /// </summary>
    /// <param name="request">The request parameters containing update fields and ID.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> UpdateItemAsync(UpdateItemRequest request)
    {
        string message;

        try
        {
            using AppDbContext context = new();
            Item? item = await context.Item.FindAsync(request.Id);

            if (item is null)
            {
                message = $"Item with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult failResult = new(Utils.Result.Failed_NoResults, message);
                return failResult;
            }

            item.Name = request.Name ?? item.Name;
            item.Description = request.Description ?? item.Description;
            item.IsForSale = request.IsForSale;
            item.Cost = request.Cost ?? item.Cost;
            item.SalePrice = request.SalePrice ?? item.SalePrice;
            item.Quantity = request.Quantity;
            item.UnitOfMeasurement = request.UnitOfMeasurement;
            item.ExpirationDate = request.ExpirationDate ?? item.ExpirationDate;
            item.Tags = request.Tags ?? item.Tags;
            item.Brand = request.Brand ?? item.Brand;
            item.ImageUrl = request.ImageUrl ?? item.ImageUrl;
            item.Barcode = request.Barcode ?? item.Barcode;
            if (item is Product prod)
            {
                prod.TargetStock = request.TargetStock ?? prod.TargetStock;
                prod.LowStockThresholdPercentage = request.LowStockThresholdPercentage ?? prod.LowStockThresholdPercentage;
            }
            await context.SaveChangesAsync();

            message = $"Item '{item.Name}' with ID {item.Id} updated successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(Utils.Result.Success, message);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when updating item with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(Utils.Result.Failed_UnhandledException, message);
            return errorResult;
        }
    }


    /// <summary>
    /// Adds tags to an existing inventory item.
    /// </summary>
    /// <param name="request">The request parameters containing the item ID and the list of tags to add.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> AddTagsToItemAsync(
        AddItemTagsRequest request
    )
    {
        string message;

        try
        {
            using AppDbContext context = new();
            Item? item = await context.Item.FindAsync(request.Id);

            if (item is null)
            {
                message = $"Item with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult failResult = new(Utils.Result.Failed_NoResults, message);
                return failResult;
            }

            List<string> updatedTags = new(item.Tags);
            foreach (string tag in request.Tags)
            {
                if (!updatedTags.Contains(tag))
                {
                    updatedTags.Add(tag);
                }
            }

            item.Tags = updatedTags;
            await context.SaveChangesAsync();

            message = $"Tags successfully added to item '{item.Name}' with ID {item.Id}.";
            Log.Me(message);
            Utils.RequestResult successResult = new(Utils.Result.Success, message);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when adding tags to item with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(Utils.Result.Failed_UnhandledException, message);
            return errorResult;
        }
    }


    /// <summary>
    /// Replaces all tags on an existing inventory item.
    /// </summary>
    /// <param name="request">The request parameters containing the item ID and the new list of tags.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> ReplaceItemTagsAsync(
        ReplaceItemTagsRequest request
    )
    {
        string message;

        try
        {
            using AppDbContext context = new();
            Item? item = await context.Item.FindAsync(request.Id);

            if (item is null)
            {
                message = $"Item with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult failResult = new(Utils.Result.Failed_NoResults, message);
                return failResult;
            }

            item.Tags = request.Tags;
            await context.SaveChangesAsync();

            message = $"Tags successfully replaced for item '{item.Name}' with ID {item.Id}.";
            Log.Me(message);
            Utils.RequestResult successResult = new(Utils.Result.Success, message);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when replacing tags for item with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(Utils.Result.Failed_UnhandledException, message);
            return errorResult;
        }
    }


    /// <summary>
    /// Removes tags from an existing inventory item.
    /// </summary>
    /// <param name="request">The request parameters containing the item ID and the list of tags to remove.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> RemoveTagsFromItemAsync(
        RemoveItemTagsRequest request
    )
    {
        string message;

        try
        {
            using AppDbContext context = new();
            Item? item = await context.Item.FindAsync(request.Id);

            if (item is null)
            {
                message = $"Item with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult failResult = new(Utils.Result.Failed_NoResults, message);
                return failResult;
            }

            List<string> updatedTags = new(item.Tags);
            foreach (string tag in request.Tags)
            {
                updatedTags.Remove(tag);
            }

            item.Tags = updatedTags;
            await context.SaveChangesAsync();

            message = $"Tags successfully removed from item '{item.Name}' with ID {item.Id}.";
            Log.Me(message);
            Utils.RequestResult successResult = new(Utils.Result.Success, message);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when removing tags from item with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(Utils.Result.Failed_UnhandledException, message);
            return errorResult;
        }
    }



    /// <summary>
    /// Creates a new inventory item.
    /// </summary>
    /// <param name="request">The request parameters containing new item details.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult<Item>> CreateItemAsync(CreateItemRequest request)
    {
        try
        {
            Item item;
            ulong id = Utils.GenerateEntityId();
            if (request.ItemType != null && request.ItemType.Equals("Service", StringComparison.OrdinalIgnoreCase))
            {
                item = new Service(
                    id,
                    request.UnitOfMeasurement,
                    request.IsForSale,
                    request.SalePrice,
                    request.Name,
                    request.Description,
                    request.Tags,
                    request.Quantity,
                    request.ExpirationDate,
                    request.Cost,
                    request.ImageUrl,
                    request.Barcode
                );
            }
            else
            {
                item = new Product(
                    id,
                    request.UnitOfMeasurement,
                    request.IsForSale,
                    request.SalePrice,
                    request.Name,
                    request.Description,
                    request.Tags,
                    request.Quantity,
                    request.ExpirationDate,
                    request.Cost,
                    request.ImageUrl,
                    request.Barcode,
                    request.TargetStock ?? 0f,
                    request.LowStockThresholdPercentage ?? 0.20f,
                    request.Brand
                );
            }

            using AppDbContext context = new();
            context.Item.Add(item);
            await context.SaveChangesAsync();

            string message = $"Item '{item.Name}' with ID {item.Id} created successfully.";
            Log.Me(() => message);

            Utils.Result resultType = Utils.Result.Success;
            Utils.RequestResult<Item> successResult = new(
                resultType, message, item
            );

            return successResult;
        }

        catch (Exception ex)
        {
            string message = $"Error creating item: {ex.Message}.";
            Log.Me(() => message);

            Utils.Result resultType = Utils.Result.Failed_UnhandledException;
            Utils.RequestResult<Item> errorResult = new(
                resultType, message, null
            );

            return errorResult;
        }
    }


    /// <summary>
    /// Deletes a specific inventory item.
    /// </summary>
    /// <param name="request">The request containing the item ID to delete.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> DeleteItemAsync(DeleteItemRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            Item? item = await context.Item.FindAsync(request.Id);

            if (item is null)
            {
                message = $"Item with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult failResult = new(Utils.Result.Failed_NoResults, message);
                return failResult;
            }

            context.Item.Remove(item);
            await context.SaveChangesAsync();

            message = $"Item '{item.Name}' with ID {item.Id} deleted successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(Utils.Result.Success, message);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when deleting item with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(Utils.Result.Failed_UnhandledException, message);
            return errorResult;
        }
    }

    #endregion

    #region Item Transaction Requests

    /// <summary>
    /// Retrieves a specific item transaction.
    /// </summary>
    /// <param name="request">The request containing the transaction ID.</param>
    /// <returns>A RequestResult containing the ItemTransaction entity.</returns>
    public static async Task<Utils.RequestResult<ItemTransaction>> GetItemTransactionAsync(
        GetItemTransactionRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            ItemTransaction? itemTransaction = await context.ItemTransaction
                .Include(t => t.Item)
                .Include(t => t.Staff)
                .Include(t => t.SaleEntry)
                .FirstOrDefaultAsync(t => t.Id == request.Id);

            if (itemTransaction is null)
            {
                message = $"Item transaction with ID {request.Id} " +
                    $"not found in database.";
                Log.Me(message);
                Utils.RequestResult<ItemTransaction> failResult = new(
                    Utils.Result.Failed_NoResults, message, null);
                return failResult;
            }

            message = $"Item transaction with ID {request.Id} " +
                $"found successfully.";
            Log.Me(message);
            Utils.RequestResult<ItemTransaction> successResult = new(
                Utils.Result.Success, message, itemTransaction);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when getting item transaction " +
                $"with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult<ItemTransaction> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, null);
            return errorResult;
        }
    }


    /// <summary>
    /// Finds item transactions matching the specified query filters.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>A RequestResult containing the list of matching ItemTransactions.</returns>
    public static async Task<Utils.RequestResult<List<ItemTransaction>>> FindItemTransactionAsync(
        FindItemTransactionRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            IQueryable<ItemTransaction> query = context.ItemTransaction
                .Include(t => t.Item)
                .Include(t => t.Staff)
                .Include(t => t.SaleEntry);

            if (request is not null)
            {
                if (request.Id is not null)
                {
                    string requestId = request.Id.Value.ToString();
                    query = query.Where(
                        t => t.Id.ToString().Contains(requestId)
                    );
                }

                if (request.MinQuantity is not null)
                {
                    query = query.Where(
                        t => t.Quantity >= request.MinQuantity
                    );
                }

                if (request.MaxQuantity is not null)
                {
                    query = query.Where(
                        t => t.Quantity <= request.MaxQuantity
                    );
                }

                if (request.Type is not null)
                {
                    query = query.Where(
                        t => t.TransactionType == request.Type
                    );
                }

                if (request.MinTimestamp is not null)
                {
                    query = query.Where(
                        t => t.Timestamp >= request.MinTimestamp
                    );
                }

                if (request.MaxTimestamp is not null)
                {
                    query = query.Where(
                        t => t.Timestamp <= request.MaxTimestamp
                    );
                }

                if (!string.IsNullOrEmpty(request.Note))
                {
                    query = query.Where(
                        t => t.Note != null && t.Note.Contains(request.Note)
                    );
                }

                if (request.ItemQuery is not null)
                {
                    FindItemRequest itemQuery = request.ItemQuery;

                    if (itemQuery.Id is not null)
                    {
                        string itemIdStr = itemQuery.Id.Value.ToString();
                        query = query.Where(
                            t => t.Item.Id.ToString().Contains(itemIdStr)
                        );
                    }

                    if (!string.IsNullOrEmpty(itemQuery.Name))
                    {
                        query = query.Where(
                            t => t.Item.Name.Contains(itemQuery.Name)
                        );
                    }

                    if (!string.IsNullOrEmpty(itemQuery.Description))
                    {
                        query = query.Where(
                            t => t.Item.Description != null &&
                            t.Item.Description.Contains(itemQuery.Description)
                        );
                    }

                    if (itemQuery.IsForSale is not null)
                    {
                        query = query.Where(
                            t => t.Item.IsForSale == itemQuery.IsForSale
                        );
                    }

                    if (itemQuery.MinCost is not null)
                    {
                        query = query.Where(
                            t => t.Item.Cost >= itemQuery.MinCost
                        );
                    }

                    if (itemQuery.MaxCost is not null)
                    {
                        query = query.Where(
                            t => t.Item.Cost <= itemQuery.MaxCost
                        );
                    }

                    if (itemQuery.MinSalePrice is not null)
                    {
                        query = query.Where(
                            t => t.Item.SalePrice >= itemQuery.MinSalePrice
                        );
                    }

                    if (itemQuery.MaxSalePrice is not null)
                    {
                        query = query.Where(
                            t => t.Item.SalePrice <= itemQuery.MaxSalePrice
                        );
                    }

                    if (itemQuery.MinQuantity is not null)
                    {
                        query = query.Where(
                            t => t.Item.Quantity >= itemQuery.MinQuantity
                        );
                    }

                    if (itemQuery.MaxQuantity is not null)
                    {
                        query = query.Where(
                            t => t.Item.Quantity <= itemQuery.MaxQuantity
                        );
                    }

                    if (itemQuery.UnitOfMeasurement is not null)
                    {
                        query = query.Where(
                            t => t.Item.UnitOfMeasurement == itemQuery.UnitOfMeasurement
                        );
                    }

                    if (itemQuery.MinExpirationDate is not null)
                    {
                        query = query.Where(
                            t => t.Item.ExpirationDate >= itemQuery.MinExpirationDate
                        );
                    }

                    if (itemQuery.MaxExpirationDate is not null)
                    {
                        query = query.Where(
                            t => t.Item.ExpirationDate <= itemQuery.MaxExpirationDate
                        );
                    }
                }
            }

            query = Utils.ApplySortingAndPagination(query, request?.Limit, request?.Offset, request?.SortBy, request?.SortOrder);

            List<ItemTransaction> results = await query.ToListAsync();

            if (request?.ItemQuery?.Tags is { Count: > 0 } && results.Count > 0)
            {
                results = [.. results.Where(t =>
                    t.Item is not null &&
                    t.Item.Tags.Any(tag => request.ItemQuery.Tags.Contains(tag)))
                ];
            }

            if (results.Count == 0)
            {
                message = "No item transactions matching query found in database.";
                Log.Me(message);
                Utils.RequestResult<List<ItemTransaction>> noResults = new(
                    Utils.Result.Success_NoResults, message, []);
                return noResults;
            }

            message = $"Found {results.Count} item transaction(s) matching the query.";
            Log.Me(message);
            Utils.RequestResult<List<ItemTransaction>> successResult = new(
                Utils.Result.Success, message, results);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when getting item transactions: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult<List<ItemTransaction>> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, []);
            return errorResult;
        }
    }


    /// <summary>
    /// Creates a new stock/item transaction (stock movement log) and adjusts item quantity.
    /// </summary>
    /// <param name="request">The request parameters containing transaction details.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> CreateItemTransactionAsync(
        CreateItemTransactionRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();

            Item? item = await context.Item.FindAsync(request.ItemId);
            if (item is null)
            {
                message = $"Item with ID {request.ItemId} not found in database.";
                Log.Me(message);
                Utils.RequestResult noItemResult = new(
                    Utils.Result.Failed_NoResults, message);
                return noItemResult;
            }

            Staff? staff = await context.Staff.FindAsync(request.StaffId);
            if (staff is null)
            {
                message = $"Staff with ID {request.StaffId} not found in database.";
                Log.Me(message);
                Utils.RequestResult noStaffResult = new(
                    Utils.Result.Failed_NoResults, message);
                return noStaffResult;
            }

            SaleEntry? saleEntry = null;
            if (request.SaleEntryId is not null)
            {
                saleEntry = await context.SaleEntry.FindAsync(
                    request.SaleEntryId.Value);
                if (saleEntry is null)
                {
                    message = $"SaleEntry with ID {request.SaleEntryId} not found.";
                    Log.Me(message);
                    Utils.RequestResult noSaleEntryResult = new(
                        Utils.Result.Failed_NoResults, message);
                    return noSaleEntryResult;
                }
            }

            ItemTransaction transaction = new(
                item: item,
                transactionType: request.Type,
                saleEntry: saleEntry,
                quantity: request.Quantity,
                staff: staff,
                timestamp: request.Timestamp,
                note: request.Note
            );

            switch (request.Type)
            {
                case ItemTransaction.Type.NewStock:
                case ItemTransaction.Type.Correction_Sum:
                    item.Quantity += request.Quantity;
                    break;

                case ItemTransaction.Type.Consumed:
                case ItemTransaction.Type.Sale:
                case ItemTransaction.Type.Damaged_Lost_Expired:
                    item.Quantity -= request.Quantity;
                    break;

                case ItemTransaction.Type.Correction_Set:
                    item.Quantity = request.Quantity;
                    break;

                default:
                    break;
            }

            bool negativeResultWarning = item.Quantity < 0;
            bool negativeAddendWarning = false;
            if (request.Quantity < 0)
            {
                switch (request.Type)
                {
                    case ItemTransaction.Type.NewStock:
                    case ItemTransaction.Type.Consumed:
                    case ItemTransaction.Type.Sale:
                    case ItemTransaction.Type.Damaged_Lost_Expired:
                        negativeAddendWarning = true;
                        break;
                }
            }

            context.ItemTransaction.Add(transaction);
            await context.SaveChangesAsync();

            if (negativeAddendWarning)
            {
                message = $"Item transaction with ID {transaction.Id} created " +
                    $"with invalid quantity ({request.Quantity}).";
                Log.Me(message);
                Utils.RequestResult warningResult = new(
                    Utils.Result.Success_Warning, message);
                return warningResult;
            }

            if (negativeResultWarning)
            {
                message = $"Item transaction with ID {transaction.Id} created " +
                    $"with invalid total quantity ({item.Quantity}).";
                Log.Me(message);
                Utils.RequestResult warningResult = new(
                    Utils.Result.Success_Warning, message);
                return warningResult;
            }

            message = $"Item transaction with ID {transaction.Id} created successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(
                Utils.Result.Success, message);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when creating item transaction: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(
                Utils.Result.Failed_UnhandledException, message);
            return errorResult;
        }
    }


    /// <summary>
    /// Deletes a specific item transaction.
    /// </summary>
    /// <param name="request">The request containing the transaction ID to delete.</param>
    /// <returns>A RequestResult representing the outcome.</returns>
    public static async Task<Utils.RequestResult> DeleteItemTransactionAsync(DeleteItemTransactionRequest request)
    {
        string message;
        try
        {
            using AppDbContext context = new();
            ItemTransaction? transaction = await context.ItemTransaction.FindAsync(request.Id);

            if (transaction is null)
            {
                message = $"Item transaction with ID {request.Id} not found in database.";
                Log.Me(message);
                Utils.RequestResult failResult = new(Utils.Result.Failed_NoResults, message);
                return failResult;
            }

            context.ItemTransaction.Remove(transaction);
            await context.SaveChangesAsync();

            message = $"Item transaction with ID {transaction.Id} deleted successfully.";
            Log.Me(message);
            Utils.RequestResult successResult = new(Utils.Result.Success, message);
            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when deleting item transaction with ID {request.Id}: {ex.Message}.";
            Log.Me(message);
            Utils.RequestResult errorResult = new(Utils.Result.Failed_UnhandledException, message);
            return errorResult;
        }
    }

    #endregion

}