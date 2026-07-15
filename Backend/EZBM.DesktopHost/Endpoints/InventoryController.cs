using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using EZBM.DesktopHost.Tools;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints for managing inventory items and item transactions.
/// </summary>
public static class InventoryController
{
    #region Item Requests

    /// <summary>
    /// Retrieves a specific inventory item by identifier.
    /// </summary>
    /// <param name="request">The request containing the item ID.</param>
    /// <returns>An HTTP result with the item details if found.</returns>
    public static async Task<IResult> GetItem(
        [FromBody] GetItemRequest request)
    {
        Utils.RequestResult<Item> result = await InventoryService.GetItemAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Finds items matching the specified query filters.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>An HTTP result with the list of matching items.</returns>
    public static async Task<IResult> FindItems(
        [FromBody] FindItemRequest request)
    {
        Utils.RequestResult<List<Item>> result = await InventoryService.FindItemAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Creates a new inventory item.
    /// </summary>
    /// <param name="request">The request containing new item details.</param>
    /// <returns>An HTTP result indicating the status of the item creation.</returns>
    public static async Task<IResult> CreateItem(
        [FromBody] CreateItemRequest request)
    {
        Utils.RequestResult<Item> result = await InventoryService.CreateItemAsync(request);

        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Deletes a specific inventory item by its identifier.
    /// </summary>
    /// <param name="request">The request containing the item ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeleteItem(
        [FromBody] DeleteItemRequest request)
    {
        Utils.RequestResult result = await InventoryService.DeleteItemAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Retrieves all inventory items.
    /// </summary>
    /// <returns>An HTTP result containing the list of all items.</returns>
    public static async Task<IResult> GetAllItems()
    {
        Utils.RequestResult<List<Item>> result = await InventoryService.FindItemAsync(null!);
        return EndpointHelpers.ToIResult(result);
    }


    /// <summary>
    /// Adds tags to an existing inventory item.
    /// </summary>
    /// <param name="request">The request containing tags to add.</param>
    /// <returns>An HTTP result indicating the status of the operation.</returns>
    public static async Task<IResult> AddItemTags(
        [FromBody] AddItemTagsRequest request
    )
    {
        Utils.RequestResult result = await InventoryService.AddTagsToItemAsync(request);
        IResult httpResult = EndpointHelpers.ToIResult(result);
        return httpResult;
    }


    /// <summary>
    /// Replaces all tags on an existing inventory item.
    /// </summary>
    /// <param name="request">The request containing the new tags list.</param>
    /// <returns>An HTTP result indicating the status of the operation.</returns>
    public static async Task<IResult> ReplaceItemTags(
        [FromBody] ReplaceItemTagsRequest request
    )
    {
        Utils.RequestResult result = await InventoryService.ReplaceItemTagsAsync(request);
        IResult httpResult = EndpointHelpers.ToIResult(result);
        return httpResult;
    }


    /// <summary>
    /// Removes tags from an existing inventory item.
    /// </summary>
    /// <param name="request">The request containing tags to remove.</param>
    /// <returns>An HTTP result indicating the status of the operation.</returns>
    public static async Task<IResult> RemoveItemTags(
        [FromBody] RemoveItemTagsRequest request
    )
    {
        Utils.RequestResult result = await InventoryService.RemoveTagsFromItemAsync(request);
        IResult httpResult = EndpointHelpers.ToIResult(result);
        return httpResult;
    }


    #endregion

    #region Item Transaction Requests

    /// <summary>
    /// Creates a new stock transaction movement.
    /// </summary>
    /// <param name="request">The request parameters containing transaction details.</param>
    /// <returns>An HTTP result indicating the status of the transaction creation.</returns>
    public static async Task<IResult> CreateItemTransaction(
        [FromBody] CreateItemTransactionRequest request)
    {
        Utils.RequestResult result = await InventoryService.CreateItemTransactionAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Retrieves a specific item transaction by its identifier.
    /// </summary>
    /// <param name="request">The request containing the transaction ID.</param>
    /// <returns>An HTTP result with the transaction details if found.</returns>
    public static async Task<IResult> GetItemTransaction(
        [FromBody] GetItemTransactionRequest request)
    {
        Utils.RequestResult<ItemTransaction> result = await InventoryService.GetItemTransactionAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Finds item transactions matching the specified query filters.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>An HTTP result with the list of matching transactions.</returns>
    public static async Task<IResult> FindItemTransactions(
        [FromBody] FindItemTransactionRequest request)
    {
        Utils.RequestResult<List<ItemTransaction>> result = await InventoryService.FindItemTransactionAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Deletes a specific item transaction by its identifier.
    /// </summary>
    /// <param name="request">The request containing the transaction ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeleteItemTransaction(
        [FromBody] DeleteItemTransactionRequest request)
    {
        Utils.RequestResult result = await InventoryService.DeleteItemTransactionAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Looks up an item by its barcode.
    /// </summary>
    public static async Task<IResult> LookupBarcode(string code)
    {
        using AppDbContext context = new();
        Item? item = await context.Item.FirstOrDefaultAsync(i => i.Barcode == code);
        if (item == null)
        {
            return Results.Json(new { error = $"Item with barcode '{code}' not found." }, statusCode: StatusCodes.Status404NotFound);
        }
        return Results.Ok(item);
    }

    #endregion

}