using System;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents an audit log of important system, operator, or device actions.
/// </summary>
public class ActionLog : Entity
{
    /// <summary>
    /// The classification of action performed (e.g., Login, ClockAction, RegisterOverride, Edit, Delete, DoorLog).
    /// </summary>
    public string ActionType { get; set; }

    /// <summary>
    /// The username of the operator (staff or customer card) who initiated the event.
    /// </summary>
    public string OperatorUsername { get; set; }

    /// <summary>
    /// Descriptive details of the action and its contextual state parameters.
    /// </summary>
    public string Details { get; set; }

    /// <summary>
    /// The exact timestamp of the action log entry.
    /// </summary>
    public DateTime Timestamp { get; set; }


    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
#pragma warning disable CS8618
    protected ActionLog() { }
#pragma warning restore CS8618

    /// <summary>
    /// Initializes a new instance of the ActionLog class.
    /// </summary>
    public ActionLog(
        ulong id,
        string actionType,
        string operatorUsername,
        string details,
        DateTime timestamp)
    {
        Id = id;
        ActionType = actionType;
        OperatorUsername = operatorUsername;
        Details = details;
        Timestamp = timestamp;
    }
}
