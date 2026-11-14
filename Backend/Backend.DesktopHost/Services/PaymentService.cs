using Backend.Core.Common;
using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;

namespace Backend.DesktopHost.Services;

/// <summary>
/// Service for processing and persisting customer payment transactions.
/// </summary>
public class PaymentService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    #region Payment Operations

    /// <summary>
    /// Processes and records a payment transaction.
    /// </summary>
    public async Task<ProcessPaymentResponse> ProcessPaymentAsync(
        ProcessPaymentRequest request
    )
    {
        if (request.Amount < 0)
        {
            throw Esc.New<ArgumentException>(
                "Payment amount cannot be negative.",
                request.Amount
            );
        }

        if (request.AmountTendered < request.Amount)
        {
            throw Esc.New<ArgumentException>(
                "Amount tendered is insufficient to cover the total charge.",
                request.AmountTendered
            );
        }

        decimal changeAmount = request.AmountTendered - request.Amount;

        PaymentRecord record = new()
        {
            Amount = request.Amount,
            AmountTendered = request.AmountTendered,
            ChangeAmount = changeAmount,
            Method = request.Method,
            ReferenceNote = request.ReferenceNote,
            Timestamp = DateTime.UtcNow
        };

        await _context.PaymentRecords.AddAsync(record);
        await _context.SaveChangesAsync();

        ProcessPaymentResponse response = new(
            Id: record.Id,
            Amount: record.Amount,
            AmountTendered: record.AmountTendered,
            ChangeAmount: record.ChangeAmount,
            Method: record.Method,
            ReferenceNote: record.ReferenceNote,
            Timestamp: record.Timestamp
        );

        return response;
    }

    #endregion
}
