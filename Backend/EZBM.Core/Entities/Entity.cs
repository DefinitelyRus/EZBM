namespace EZBM.Core.Entities;

/// <summary>
/// The base class for all domain entities, providing common identification and auditing properties.
/// </summary>
public abstract class Entity
{
    #region Properties

    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// The date and time when the entity was first created.
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// The date and time when the entity was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    #endregion
}