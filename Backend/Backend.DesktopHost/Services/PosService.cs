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
    /// Processes cart checkout atomically, reducing stock and logging transactions.
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

                Product product =
                    await _context.Products.FindAsync(cartItem.ProductId)
                    ?? throw Esc.New<ArgumentException>(
                        "Product not found.",
                        cartItem.ProductId
                    );

                if (product.TotalQuantity < cartItem.Quantity)
                {
                    string errorMsg =
                        $"Insufficient stock for product '{product.Name}'. " +
                        $"Available: {product.TotalQuantity}, " +
                        $"Requested: {cartItem.Quantity}.";
                    throw Esc.New<InvalidOperationException>(errorMsg);
                }

                // Reduce total product quantity
                product.TotalQuantity -= cartItem.Quantity;

                // Create stock reduction transaction
                ProductTransaction transaction = new()
                {
                    Product = product,
                    Quantity = cartItem.Quantity,
                    TransactionType = TransactionType.Sale,
                    Timestamp = DateTime.UtcNow
                };
                await _context.ProductTransactions.AddAsync(transaction);

                // Reduce from available batches (FIFO/FEFO order)
                decimal remainingToDeduct = cartItem.Quantity;
                List<ProductBatch> batches = await _context.ProductBatches
                    .Where(b => b.Product.Id == product.Id && b.Quantity > 0)
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

                decimal itemSubtotal = cartItem.Quantity * product.Price;
                totalOrderAmount += itemSubtotal;

                SaleOrderItem orderItem = new()
                {
                    Product = product,
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price
                };
                orderItems.Add(orderItem);

                SaleOrderItemDto itemDto = new(
                    ProductId: product.Id,
                    ProductName: product.Name,
                    Quantity: cartItem.Quantity,
                    UnitPrice: product.Price,
                    Subtotal: itemSubtotal
                );
                orderItemDtos.Add(itemDto);
            }

            if (request.AmountTendered < totalOrderAmount)
            {
                throw Esc.New<ArgumentException>(
                    "Amount tendered is insufficient to cover total order cost.",
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
