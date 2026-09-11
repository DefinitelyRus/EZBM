namespace Backend.Core.Models;

public abstract class Entity
{
    /// <summary>
    /// A unique identifier for this entity.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// When this entity was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this entity was last updated.
    /// </summary>
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Additional notes for this entity.
    /// </summary>
    public List<string>? Notes { get; set; }
}