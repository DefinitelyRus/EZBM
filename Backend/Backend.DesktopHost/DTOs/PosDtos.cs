using Backend.Core.Common;
using Backend.DesktopHost.DTOs;

namespace Backend.DesktopHost.DTOs;

/// <summary>
/// Line item in a POS cart checkout request.
/// </summary>
public record CartItemRequest(
    long ProductId,
    decimal Quantity
);

/// <summary>
/// Request record for completing a POS cart checkout.
/// </summary>
public record CheckoutRequest(
    List<CartItemRequest> Items,
    decimal AmountTendered,
    PaymentMethod PaymentMethod,
    string? ReferenceNote,
    long? UserId
);

/// <summary>
/// Line item summary in a completed sale response.
/// </summary>
public record SaleOrderItemDto(
    long ProductId,
    string ProductName,
    decimal Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

/// <summary>
/// Response record returned after successful POS sale checkout.
/// </summary>
public record CheckoutResponse(
    long OrderId,
    decimal TotalAmount,
    decimal AmountTendered,
    decimal ChangeAmount,
    PaymentMethod PaymentMethod,
    DateTime Timestamp,
    List<SaleOrderItemDto> Items
);
