using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.DesktopClient.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EZBM.DesktopClient.Pages;

/// <summary>
/// Page model for Point-of-Sale checkout interface.
/// </summary>
public class POSModel : PageModel
{
    private readonly ICashRegisterService _cashRegisterService;

    /// <summary>
    /// Represents an item in the client checkout cart.
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

    /// <summary>
    /// Initializes a new instance of the POSModel page model.
    /// </summary>
    public POSModel(ICashRegisterService cashRegisterService)
    {
        _cashRegisterService = cashRegisterService;
    }

    #region Handlers

    /// <summary>
    /// Handles GET request to load catalog and employee context.
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
    /// </summary>
    public async Task<IActionResult> OnPostCheckoutAsync(
        Transaction.PayMethod paymentMethod,
        float totalAmount,
        string? notes,
        string cartJson,
        string? splitPaymentsJson,
        string? promoCode
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

            List<SplitPaymentRequest>? splitPayments = null;
            if (paymentMethod == Transaction.PayMethod.Mixed && !string.IsNullOrEmpty(splitPaymentsJson))
            {
                splitPayments = JsonSerializer.Deserialize<List<SplitPaymentRequest>>(
                    splitPaymentsJson,
                    serializeOptions
                );
            }

            CreateSaleRequest checkoutRequest = new(
                StaffId: ActiveStaff.Id,
                PaymentMethod: paymentMethod,
                TotalAmount: totalAmount,
                Notes: notes,
                Items: saleItemsList,
                SplitPayments: splitPayments,
                PromoCode: promoCode
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

    /// <summary>
    /// Handles manual override to open the cash register drawer via an RFID swipe code.
    /// </summary>
    public async Task<IActionResult> OnPostOpenRegisterAsync(string rfidCardId)
    {
        if (string.IsNullOrEmpty(rfidCardId))
        {
            ErrorMessage = "RFID card code is required to override register.";
            return RedirectToPage("/POS");
        }

        try
        {
            using AppDbContext db = new();
            var user = await db.User.FirstOrDefaultAsync(u => u.RfidCardId == rfidCardId);
            if (user is null || user.AccessType != AccessCardType.Staff)
            {
                ErrorMessage = "Access Denied: Invalid RFID Card or card is not a registered Staff profile.";
                return RedirectToPage("/POS");
            }

            // Trigger physical drawer open
            _cashRegisterService.OpenDrawer();

            // Save to audit logs
            var log = new ActionLog(
                id: Utils.GenerateEntityId(),
                actionType: "RegisterOverride",
                operatorUsername: user.FirstName ?? user.RfidCardId ?? "Unknown Staff",
                details: $"Manual cash register drawer override triggered by RFID swipe of Staff: '{user.FirstName} {user.LastName}' (RFID: {user.RfidCardId}).",
                timestamp: DateTime.UtcNow
            );
            db.ActionLog.Add(log);
            await db.SaveChangesAsync();

            SuccessMessage = $"Cash Register override successful. Drawer opened by {user.FirstName}!";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Register override failed: {ex.Message}";
        }

        return RedirectToPage("/POS");
    }

    #endregion
}
