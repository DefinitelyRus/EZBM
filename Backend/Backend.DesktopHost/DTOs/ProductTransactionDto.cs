namespace Backend.DesktopHost.DTOs;

/// <summary>
/// Data transfer object for product transaction records.
/// </summary>
public record ProductTransactionDto(
    long Id,
    long ProductId,
    string ProductName,
    long? BatchId,
    decimal Quantity,
    string TransactionType,
    DateTime Timestamp
);
