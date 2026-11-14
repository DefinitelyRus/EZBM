using Backend.Core.Common;

namespace Backend.DesktopHost.DTOs;

/// <summary>
/// Request record for processing customer payments.
/// </summary>
public record ProcessPaymentRequest(
    decimal Amount,
    decimal AmountTendered,
    PaymentMethod Method,
    string? ReferenceNote
);

/// <summary>
/// Response record containing payment confirmation and change details.
/// </summary>
public record ProcessPaymentResponse(
    long Id,
    decimal Amount,
    decimal AmountTendered,
    decimal ChangeAmount,
    PaymentMethod Method,
    string? ReferenceNote,
    DateTime Timestamp
);
