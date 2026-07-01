using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EZBM.DesktopClient.Pages;

/// <summary>
/// Page model for the inventory management screen.
/// </summary>
public class InventoryModel : PageModel
{

    #region Properties

    /// <summary>
    /// List of all inventory items retrieved from the database.
    /// </summary>
    public List<Item> Items { get; set; } = new();

    /// <summary>
    /// Item being edited, if action is edit.
    /// </summary>
    public Item? EditingItem { get; set; }

    /// <summary>
    /// Error message to display.
    /// </summary>
    [TempData]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Success message to display.
    /// </summary>
    [TempData]
    public string? SuccessMessage { get; set; }

    #endregion

    #region Handlers

    /// <summary>
    /// Handles GET request for listing items and setting up edit mode.
    /// </summary>
    public async Task OnGetAsync(
        string? action,
        ulong? id
    )
    {
        Utils.RequestResult<List<Item>> result = await InventoryService.FindItemAsync(null!);
        Items = result.Data ?? new List<Item>();

        if (action == "edit" && id.HasValue)
        {
            Utils.RequestResult<Item> getItemResult = await InventoryService.GetItemAsync(
                new GetItemRequest(id.Value)
            );

            if (getItemResult.Type == Utils.Result.Success)
                EditingItem = getItemResult.Data;
        }
    }


    /// <summary>
    /// Handles creating a new inventory item.
    /// </summary>
    public async Task<IActionResult> OnPostCreateAsync(
        string name,
        string? description,
        bool isForSale,
        float? cost,
        float? salePrice,
        float quantity,
        Item.Unit unitOfMeasurement,
        DateTime? expirationDate,
        List<Item.Tag>? tags
    )
    {
        if (cost.HasValue && cost.Value < 0f)
        {
            ErrorMessage = "Cost price cannot be negative.";
            return RedirectToPage("/Inventory");
        }

        if (salePrice.HasValue && salePrice.Value < 0f)
        {
            ErrorMessage = "Sale price cannot be negative.";
            return RedirectToPage("/Inventory");
        }

        if (quantity < 0f)
        {
            ErrorMessage = "Stock quantity cannot be negative.";
            return RedirectToPage("/Inventory");
        }

        CreateItemRequest request = new(
            Name: name,
            Description: description,
            IsForSale: isForSale,
            Cost: cost,
            SalePrice: salePrice,
            Quantity: quantity,
            UnitOfMeasurement: unitOfMeasurement,
            ExpirationDate: expirationDate,
            Tags: tags ?? new List<Item.Tag>(),
            ImageUrl: null
        );

        Utils.RequestResult<Item> result = await InventoryService.CreateItemAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = $"Item '{name}' created successfully.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Inventory");
    }


    /// <summary>
    /// Handles updating an existing inventory item.
    /// </summary>
    public async Task<IActionResult> OnPostUpdateAsync(
        ulong id,
        string name,
        string? description,
        bool isForSale,
        float? cost,
        float? salePrice,
        float quantity,
        Item.Unit unitOfMeasurement,
        DateTime? expirationDate,
        List<Item.Tag>? tags
    )
    {
        if (cost.HasValue && cost.Value < 0f)
        {
            ErrorMessage = "Cost price cannot be negative.";
            return RedirectToPage("/Inventory");
        }

        if (salePrice.HasValue && salePrice.Value < 0f)
        {
            ErrorMessage = "Sale price cannot be negative.";
            return RedirectToPage("/Inventory");
        }

        if (quantity < 0f)
        {
            ErrorMessage = "Stock quantity cannot be negative.";
            return RedirectToPage("/Inventory");
        }

        UpdateItemRequest request = new(
            Id: id,
            Name: name,
            Description: description,
            IsForSale: isForSale,
            Cost: cost,
            SalePrice: salePrice,
            Quantity: quantity,
            UnitOfMeasurement: unitOfMeasurement,
            ExpirationDate: expirationDate,
            Tags: tags ?? new List<Item.Tag>()
        );

        Utils.RequestResult result = await InventoryService.UpdateItemAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = $"Item with ID {id} updated successfully.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Inventory");
    }


    /// <summary>
    /// Handles deleting an inventory item.
    /// </summary>
    public async Task<IActionResult> OnPostDeleteAsync(
        ulong id
    )
    {
        DeleteItemRequest request = new(Id: id);
        Utils.RequestResult result = await InventoryService.DeleteItemAsync(request);

        if (result.Type == Utils.Result.Success)
            SuccessMessage = $"Item with ID {id} deleted successfully.";
        else
            ErrorMessage = result.Message;

        return RedirectToPage("/Inventory");
    }

    #endregion

}
