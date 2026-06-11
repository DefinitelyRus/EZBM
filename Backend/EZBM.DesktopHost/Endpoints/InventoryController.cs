using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.DesktopHost.Tools;
using Microsoft.AspNetCore.Mvc;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints for managing inventory items and item transactions.
/// <br/><br/>
/// <i>Documented by: Google Antigravity</i>
/// </summary>
public static class InventoryController
{

    #region Item Requests

    /// <summary>
    /// Retrieves a specific inventory item by identifier.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request containing the item ID.</param>
    /// <returns>An HTTP result with the item details if found.</returns>
    public static async Task<IResult> GetItem([FromBody] GetItemRequest request)
    {
        Utils.RequestResult<Item> result = await InventoryService.GetItemAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Finds items matching the specified query filters.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>An HTTP result with the list of matching items.</returns>
    public static async Task<IResult> FindItems([FromBody] FindItemRequest request)
    {
        Utils.RequestResult<List<Item>> result = await InventoryService.FindItemAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Creates a new inventory item.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request containing new item details.</param>
    /// <returns>An HTTP result indicating the status of the item creation.</returns>
    public static async Task<IResult> CreateItem([FromBody] CreateItemRequest request)
    {
        Utils.RequestResult<Item> result = await InventoryService.CreateItemAsync(request);

        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Deletes a specific inventory item by its identifier.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request containing the item ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeleteItem([FromBody] DeleteItemRequest request)
    {
        Utils.RequestResult result = await InventoryService.DeleteItemAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Retrieves all inventory items.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <returns>An HTTP result containing the list of all items.</returns>
    public static async Task<IResult> GetAllItems()
    {
        Utils.RequestResult<List<Item>> result = await InventoryService.FindItemAsync(null!);
        return EndpointHelpers.ToIResult(result);
    }

    #endregion

    #region Item Transaction Requests

    /// <summary>
    /// Creates a new stock transaction movement.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request parameters containing transaction details.</param>
    /// <returns>An HTTP result indicating the status of the transaction creation.</returns>
    public static async Task<IResult> CreateItemTransaction([FromBody] CreateItemTransactionRequest request)
    {
        Utils.RequestResult result = await InventoryService.CreateItemTransactionAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Retrieves a specific item transaction by its identifier.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request containing the transaction ID.</param>
    /// <returns>An HTTP result with the transaction details if found.</returns>
    public static async Task<IResult> GetItemTransaction([FromBody] GetItemTransactionRequest request)
    {
        Utils.RequestResult<ItemTransaction> result = await InventoryService.GetItemTransactionAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Finds item transactions matching the specified query filters.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <returns>An HTTP result with the list of matching transactions.</returns>
    public static async Task<IResult> FindItemTransactions([FromBody] FindItemTransactionRequest request)
    {
        Utils.RequestResult<List<ItemTransaction>> result = await InventoryService.FindItemTransactionAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Deletes a specific item transaction by its identifier.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request containing the transaction ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeleteItemTransaction([FromBody] DeleteItemTransactionRequest request)
    {
        Utils.RequestResult result = await InventoryService.DeleteItemTransactionAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    #endregion

}