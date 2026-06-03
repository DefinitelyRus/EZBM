namespace EZBM.Core.Entities;

/// <summary>
/// The base class for all domain entities, providing common identification and auditing properties.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editors(s): None<br/>
/// Documented by: Google Gemini</i>
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// The unique identifier for the entity.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// The date and time when the entity was first created.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// The date and time when the entity was last modified.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
}