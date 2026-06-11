using System.ComponentModel.DataAnnotations;
using EZBM.Core.Entities;
using static EZBM.Core.Entities.Item;
using static EZBM.Core.Entities.ItemTransaction;

namespace EZBM.Core.Tools;

#region Items

public record GetItemRequest(
    [Required] ulong Id
);

public record FindItemRequest(
    ulong? Id,
    string? Name,
    string? Description,
    bool? IsForSale,
    float? MinCost,
    float? MaxCost,
    float? MinSalePrice,
    float? MaxSalePrice,
    float? MinQuantity,
    float? MaxQuantity,
    Unit? UnitOfMeasurement,
    DateTime? MinExpirationDate,
    DateTime? MaxExpirationDate,
    List<Tag>? Tags
);

public record UpdateItemRequest(
    [Required] ulong Id,
    string? Name,
    string? Description,
    [Required] bool IsForSale,
    float? Cost,
    float? SalePrice,
    [Required] float Quantity,
    [Required] Unit UnitOfMeasurement,
    DateTime? ExpirationDate,
    List<Tag>? Tags
);

public record CreateItemRequest(
    string? Name,
    string? Description,
    [Required] bool IsForSale,
    float? Cost,
    float? SalePrice,
    [Required] float Quantity,
    [Required] Unit UnitOfMeasurement,
    DateTime? ExpirationDate,
    List<Tag>? Tags,
    string? ImageUrl
);

public record DeleteItemRequest(
    [Required] ulong Id
);

#endregion

#region Item Transactions

public record GetItemTransactionRequest(
    [Required] ulong Id
);

public record CreateItemTransactionRequest(
    [Required] ulong ItemId,
    [Required] ItemTransaction.Type Type,
    ulong? SaleEntryId,
    [Required] float Quantity,
    [Required] ulong StaffId,
    [Required] DateTime Timestamp,
    string? Note
);

public record FindItemTransactionRequest(
    ulong? Id,
    FindItemRequest? ItemQuery,
    float? MinQuantity,
    float? MaxQuantity,
    ItemTransaction.Type? Type,
    DateTime? MinTimestamp,
    DateTime? MaxTimestamp,
    string? Note
);

[Obsolete("ItemTransactions are immutable and must not be updated.")]
public record UpdateItemTransactionRequest(
    [Required] ulong Id,
    [Required] ulong ItemId,
    [Required] float Quantity,
    [Required] ItemTransaction.Type Type,
    [Required] DateTime Timestamp,
    string? Note
);

public record DeleteItemTransactionRequest(
    [Required] ulong Id
);

#endregion

#region Sales

public record SaleItemRequest(
    [Required] ulong ItemId,
    [Required] float Quantity,
    [Required] float UnitPrice
);

public record CreateSaleRequest(
    [Required] ulong StaffId,
    [Required] Transaction.PayMethod PaymentMethod,
    [Required] float TotalAmount,
    string? Notes,
    [Required] List<SaleItemRequest> Items
);

public record FindSaleRequest(
    ulong? Id,
    int? InvoiceNumber,
    ulong? StaffId,
    DateTime? MinTimestamp,
    DateTime? MaxTimestamp,
    Transaction.PayMethod? PaymentMethod,
    float? MinAmount,
    float? MaxAmount
);

public record GetSaleRequest(
    [Required] ulong Id
);

public record DeleteSaleRequest(
    [Required] ulong Id
);

public record CreateSaleEntryRequest(
    [Required] ulong SaleId,
    [Required] ulong ItemId,
    [Required] float Quantity,
    [Required] float UnitPrice,
    [Required] float Subtotal
);

public record GetSaleEntryRequest(
    [Required] ulong Id
);

public record FindSaleEntryRequest(
    ulong? Id,
    ulong? SaleId,
    ulong? ItemId,
    float? MinQuantity,
    float? MaxQuantity,
    float? MinUnitPrice,
    float? MaxUnitPrice
);

public record DeleteSaleEntryRequest(
    [Required] ulong Id
);

#endregion

#region Staff

public record CreateStaffRequest(
    [Required] string Username,
    string? Password,
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber,
    string? Position,
    [Required] Staff.Frequency PayFrequency,
    [Required] float PayRate
);

public record GetStaffRequest(
    [Required] ulong Id
);

public record FindStaffRequest(
    ulong? Id,
    string? Username,
    string? FirstName,
    string? LastName,
    string? Position,
    Staff.Frequency? PayFrequency
);

public record UpdateStaffRequest(
    [Required] ulong Id,
    string? Username,
    string? Password,
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber,
    string? Position,
    Staff.Frequency? PayFrequency,
    float? PayRate
);

public record DeleteStaffRequest(
    [Required] ulong Id
);

#endregion

#region Payroll

public record CreatePayrollRequest(
    [Required] ulong StaffId,
    [Required] DateTime PeriodStart,
    [Required] DateTime PeriodEnd,
    [Required] float TotalHours,
    [Required] float GrossAmount,
    [Required] float Modifiers,
    [Required] float NetAmount,
    DateTime? PayDate,
    string? Notes
);

public record GetPayrollRequest(
    [Required] ulong Id
);

public record FindPayrollRequest(
    ulong? Id,
    ulong? StaffId,
    DateTime? MinPeriodStart,
    DateTime? MaxPeriodStart,
    DateTime? MinPeriodEnd,
    DateTime? MaxPeriodEnd,
    float? MinNetAmount,
    float? MaxNetAmount
);

public record DeletePayrollRequest(
    [Required] ulong Id
);

#endregion

#region Attendance

public record CreateAttendanceRequest(
    [Required] ulong StaffId,
    [Required] DateTime TimeIn,
    DateTime? TimeOut
);

public record GetAttendanceRequest(
    [Required] ulong Id
);

public record FindAttendanceRequest(
    ulong? Id,
    ulong? StaffId,
    DateTime? MinTimeIn,
    DateTime? MaxTimeIn,
    DateTime? MinTimeOut,
    DateTime? MaxTimeOut
);

public record UpdateAttendanceRequest(
    [Required] ulong Id,
    ulong? StaffId,
    DateTime? TimeIn,
    DateTime? TimeOut
);

public record DeleteAttendanceRequest(
    [Required] ulong Id
);

public record LogAttendanceRequest(
    [Required] ulong StaffId,
    [Required] string ActionType
);

#endregion

#region Authentication

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

#endregion