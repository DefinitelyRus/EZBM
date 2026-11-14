using Backend.Core.Common;
using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.DesktopHost.Services;

/// <summary>
/// Service handling Point of Sale checkout and stock reduction operations.
/// </summary>
public class PosService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    #region Checkout Operations

    /// <summary>
    /// Processes cart checkout and logs transactions atomically.
    /// </summary>
    public async Task<CheckoutResponse> CheckoutAsync(
        CheckoutRequest request
    )
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            throw Esc.New<ArgumentException>(
                "Cart contains no items to checkout."
            );
        }

        IDbContextTransaction dbTransaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            List<SaleOrderItem> orderItems = [];
            List<SaleOrderItemDto> orderItemDtos = [];
            decimal totalOrderAmount = 0m;

            foreach (CartItemRequest cartItem in request.Items)
            {
                if (cartItem.Quantity <= 0)
                {
                    throw Esc.New<ArgumentException>(
                        "Item quantity must be greater than zero.",
                        cartItem.Quantity
                    );
                }

                bool isService = cartItem.ItemType
                    .Equals("Service", StringComparison.OrdinalIgnoreCase);

                string itemName;
                decimal unitPrice;

                if (isService)
                {
                    Service service =
                        await _context.Services.FindAsync(cartItem.ItemId)
                        ?? throw Esc.New<ArgumentException>(
                            "Service not found.",
                            cartItem.ItemId
                        );

                    itemName = service.Name;
                    unitPrice = service.Price;
                }
                else
                {
                    Product product =
                        await _context.Products.FindAsync(cartItem.ItemId)
                        ?? throw Esc.New<ArgumentException>(
                            "Product not found.",
                            cartItem.ItemId
                        );

                    if (product.TotalQuantity < cartItem.Quantity)
                    {
                        string errorMsg =
                            $"Insufficient stock for '{product.Name}'. " +
                            $"Available: {product.TotalQuantity}, " +
                            $"Requested: {cartItem.Quantity}.";
                        throw Esc.New<InvalidOperationException>(errorMsg);
                    }

                    product.TotalQuantity -= cartItem.Quantity;

                    ProductTransaction transaction = new()
                    {
                        Product = product,
                        Quantity = cartItem.Quantity,
                        TransactionType = TransactionType.Sale,
                        Timestamp = DateTime.UtcNow
                    };
                    await _context.ProductTransactions.AddAsync(transaction);

                    decimal remainingToDeduct = cartItem.Quantity;
                    List<ProductBatch> batches = await _context.ProductBatches
                        .Where(b => b.Product.Id == product.Id)
                        .Where(b => b.Quantity > 0)
                        .OrderBy(b => b.ExpirationDate)
                        .ThenBy(b => b.Id)
                        .ToListAsync();

                    foreach (ProductBatch batch in batches)
                    {
                        if (remainingToDeduct <= 0) break;

                        decimal deductFromBatch =
                            Math.Min(batch.Quantity, remainingToDeduct);
                        batch.Quantity -= deductFromBatch;
                        remainingToDeduct -= deductFromBatch;
                    }

                    itemName = product.Name;
                    unitPrice = product.Price;
                }

                decimal itemSubtotal = cartItem.Quantity * unitPrice;
                totalOrderAmount += itemSubtotal;

                SaleOrderItem orderItem = new()
                {
                    ItemId = cartItem.ItemId,
                    ItemType = isService ? "Service" : "Product",
                    ItemName = itemName,
                    Quantity = cartItem.Quantity,
                    UnitPrice = unitPrice
                };
                orderItems.Add(orderItem);

                SaleOrderItemDto itemDto = new(
                    ItemId: cartItem.ItemId,
                    ItemType: isService ? "Service" : "Product",
                    ItemName: itemName,
                    Quantity: cartItem.Quantity,
                    UnitPrice: unitPrice,
                    Subtotal: itemSubtotal
                );
                orderItemDtos.Add(itemDto);
            }

            if (request.AmountTendered < totalOrderAmount)
            {
                throw Esc.New<ArgumentException>(
                    "Tendered amount is insufficient for total cost.",
                    request.AmountTendered
                );
            }

            decimal changeAmount = request.AmountTendered - totalOrderAmount;

            PaymentRecord payment = new()
            {
                Amount = totalOrderAmount,
                AmountTendered = request.AmountTendered,
                ChangeAmount = changeAmount,
                Method = request.PaymentMethod,
                ReferenceNote = request.ReferenceNote,
                Timestamp = DateTime.UtcNow
            };
            await _context.PaymentRecords.AddAsync(payment);

            SaleOrder saleOrder = new()
            {
                UserId = request.UserId,
                TotalAmount = totalOrderAmount,
                Timestamp = DateTime.UtcNow,
                Payment = payment,
                Items = orderItems
            };
            await _context.SaleOrders.AddAsync(saleOrder);

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            CheckoutResponse response = new(
                OrderId: saleOrder.Id,
                TotalAmount: saleOrder.TotalAmount,
                AmountTendered: request.AmountTendered,
                ChangeAmount: changeAmount,
                PaymentMethod: request.PaymentMethod,
                Timestamp: saleOrder.Timestamp,
                Items: orderItemDtos
            );

            return response;
        }

        catch (Exception)
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    #endregion
}
