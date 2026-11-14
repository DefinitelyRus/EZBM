namespace Backend.Core.Models;

/// <summary>
/// User entity for system authentication and audit tracking.
/// </summary>
public class User : Entity
{
    /// <summary>
    /// Unique username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// PBKDF2 computed password hash.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Salt used to compute the password hash.
    /// </summary>
    public string Salt { get; set; } = string.Empty;

    /// <summary>
    /// User role (e.g., Admin, Employee).
    /// </summary>
    public string Role { get; set; } = "Admin";
}
