namespace Backend.Core.Models;

/// <summary>
/// Persisted authentication session record for desktop users.
/// </summary>
public class UserSession : Entity
{
    /// <summary>
    /// Identifier of the authenticated user.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Cryptographic session token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when this session expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
