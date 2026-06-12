using System.Text.Json;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.DesktopClient.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EZBM.DesktopClient.Pages;

/// <summary>
/// Page model for Point-of-Sale checkout interface.
/// <br/><br/>
/// <i>Documented by: Google Antigravity</i>
/// </summary>
public class POSModel : PageModel
{

    /// <summary>
    /// Represents an item in the client checkout cart.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    public record CartItemDto(
        ulong ItemId,
        float Quantity,
        float UnitPrice
    );

    #region Properties

    /// <summary>
    /// List of all items available for sale.
    /// </summary>
    public List<Item> CatalogItems { get; set; } = new();

    /// <summary>
    /// Current logged in staff member.
    /// </summary>
    public Staff? ActiveStaff { get; set; }

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
    /// Handles GET request to load catalog and employee context.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    public async Task OnGetAsync()
    {
        ActiveStaff = await StateHelper.GetActiveStaffAsync(HttpContext);

        // Fetch only items that are marked as for sale
        FindItemRequest searchRequest = new(
            Id: null,
            Name: null,
            Description: null,
            IsForSale: true,
            MinCost: null,
            MaxCost: null,
            MinSalePrice: null,
            MaxSalePrice: null,
            MinQuantity: null,
            MaxQuantity: null,
            UnitOfMeasurement: null,
            MinExpirationDate: null,
            MaxExpirationDate: null,
            Tags: null
        );

        Utils.RequestResult<List<Item>> result = await InventoryService.FindItemAsync(searchRequest);
        CatalogItems = result.Data ?? new List<Item>();
    }


    /// <summary>
    /// Handles checkout submission from the cart form.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    public async Task<IActionResult> OnPostCheckoutAsync(
        Transaction.PayMethod paymentMethod,
        float totalAmount,
        string? notes,
        string cartJson
    )
    {
        ActiveStaff = await StateHelper.GetActiveStaffAsync(HttpContext);

        if (ActiveStaff is null)
        {
            ErrorMessage = "Cannot checkout: No active employee is logged in. Please log in first.";
            return RedirectToPage("/POS");
        }

        if (string.IsNullOrWhiteSpace(cartJson))
        {
            ErrorMessage = "Cannot checkout: Shopping cart is empty.";
            return RedirectToPage("/POS");
        }

        try
        {
            JsonSerializerOptions serializeOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };

            List<CartItemDto>? cartItems = JsonSerializer.Deserialize<List<CartItemDto>>(
                cartJson,
                serializeOptions
            );

            if (cartItems is null || cartItems.Count == 0)
            {
                ErrorMessage = "Cannot checkout: Failed to parse shopping cart items.";
                return RedirectToPage("/POS");
            }

            if (totalAmount < 0f)
            {
                ErrorMessage = "Cannot checkout: Total amount cannot be negative.";
                return RedirectToPage("/POS");
            }

            foreach (CartItemDto item in cartItems)
            {
                if (item.Quantity <= 0f)
                {
                    ErrorMessage = "Cannot checkout: Item quantity must be greater than zero.";
                    return RedirectToPage("/POS");
                }

                if (item.UnitPrice < 0f)
                {
                    ErrorMessage = "Cannot checkout: Item unit price cannot be negative.";
                    return RedirectToPage("/POS");
                }
            }

            List<SaleItemRequest> saleItemsList = cartItems
                .Select(i => new SaleItemRequest(
                    ItemId: i.ItemId,
                    Quantity: i.Quantity,
                    UnitPrice: i.UnitPrice
                ))
                .ToList();

            CreateSaleRequest checkoutRequest = new(
                StaffId: ActiveStaff.Id,
                PaymentMethod: paymentMethod,
                TotalAmount: totalAmount,
                Notes: notes,
                Items: saleItemsList
            );

            Utils.RequestResult<ulong> saleResult = await SalesService.CreateSaleAsync(checkoutRequest);

            if (saleResult.Type == Utils.Result.Success)
            {
                SuccessMessage = $"Sale registered successfully! Invoice ID is: {saleResult.Data}. Cart reset.";
            }
            else
            {
                ErrorMessage = $"Checkout failed: {saleResult.Message}";
            }
        }

        catch (Exception ex)
        {
            ErrorMessage = $"System checkout exception: {ex.Message}";
        }

        return RedirectToPage("/POS");
    }

    #endregion

}
