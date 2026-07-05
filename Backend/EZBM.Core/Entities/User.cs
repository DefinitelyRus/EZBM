using System;
using System.Collections.Generic;

namespace EZBM.Core.Entities;

/// <summary>
/// Defines the types of card access levels supported.
/// </summary>
public enum AccessCardType
{
    None,
    Staff,
    OneTime,
    Member
}

/// <summary>
/// Represents the base user entity, sharing identity, personal, and RFID card/access details.
/// </summary>
public abstract class User : Entity
{
    #region Properties

    /// <summary>
    /// The first name of the user.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// The last name of the user.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// The contact phone number of the user.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// The access card type assigned to this user.
    /// </summary>
    public AccessCardType AccessType { get; set; } = AccessCardType.None;

    /// <summary>
    /// The unique identifier of the RFID card assigned to the user.
    /// </summary>
    public string? RfidCardId { get; set; }

    /// <summary>
    /// The active permissions allowed when the card is within its valid period.
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// The fallback permissions applied if the card has expired.
    /// </summary>
    public List<string> PermissionsAfterExpiry { get; set; } = new();

    /// <summary>
    /// The date and time when access card privileges expire.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// The list of custom roles assigned to this user.
    /// </summary>
    public List<Role> Roles { get; set; } = new();

    #endregion

    #region Public Methods

    /// <summary>
    /// Lazy permission check that evaluates the current expiration date.
    /// </summary>
    /// <returns>The active list of permissions.</returns>
    public List<string> GetActivePermissions()
    {
        if (ExpirationDate.HasValue && DateTime.UtcNow > ExpirationDate.Value)
        {
            return PermissionsAfterExpiry;
        }
        return Permissions;
    }

    /// <summary>
    /// Checks if a user has a specific permission using the roles matrix and flat overrides.
    /// Deny (-1) overrides all. Allow (1) overrides Default (0).
    /// </summary>
    /// <param name="action">The name of the action/permission to check.</param>
    /// <returns>True if allowed; otherwise false.</returns>
    public bool HasPermission(string action)
    {
        // 1. Check roles matrix
        int resolvedState = 0; // Default
        foreach (var role in Roles)
        {
            try
            {
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(role.PermissionsJson);
                if (dict != null && dict.TryGetValue(action, out int state))
                {
                    if (state == -1) // Explicit Deny overrides all
                    {
                        return false;
                    }
                    if (state == 1)
                    {
                        resolvedState = 1;
                    }
                }
            }
            catch { }
        }

        if (resolvedState == 1) return true;

        // 2. Fallback to flat overrides
        var activeFlat = GetActivePermissions();
        if (activeFlat.Contains($"-{action}")) return false; // deny override
        return activeFlat.Contains(action);
    }

    #endregion
}
