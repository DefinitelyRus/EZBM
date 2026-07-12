using System;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents a payroll adjustment such as a bonus or advance pay.
/// </summary>
public class StaffAdjustment : Entity
{
    /// <summary>
    /// The unique identifier of the staff member.
    /// </summary>
    public ulong StaffId { get; set; }

    /// <summary>
    /// The type of adjustment (e.g. "Bonus" or "AdvancePay").
    /// </summary>
    public string AdjustmentType { get; set; } = "Bonus";

    /// <summary>
    /// The amount of the adjustment.
    /// </summary>
    public float Amount { get; set; }

    /// <summary>
    /// Whether this adjustment should be automatically deducted from the current payroll.
    /// </summary>
    public bool DeductFromCurrentPayroll { get; set; }

    /// <summary>
    /// Whether this adjustment has been paid out or settled.
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// The timestamp of the adjustment.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Additional comments or reasons.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// The parent payroll invoice ID if this adjustment has been processed.
    /// </summary>
    public ulong? PayrollId { get; set; }

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    protected StaffAdjustment() : base() { }

    /// <summary>
    /// Initializes a new instance of the StaffAdjustment class.
    /// </summary>
    public StaffAdjustment(
        ulong id,
        ulong staffId,
        string adjustmentType,
        float amount,
        bool deductFromCurrentPayroll,
        bool isPaid,
        DateTime timestamp,
        string? notes = null)
    {
        Id = id;
        StaffId = staffId;
        AdjustmentType = adjustmentType;
        Amount = amount;
        DeductFromCurrentPayroll = deductFromCurrentPayroll;
        IsPaid = isPaid;
        Timestamp = timestamp;
        Notes = notes;
    }
}
