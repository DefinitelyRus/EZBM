using System;
using System.Collections.Generic;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents a system role with granular permissions mapping.
/// </summary>
public class Role : Entity
{
    /// <summary>
    /// The display name of the role.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A JSON string storing action-to-permission mapping dictionary.
    /// </summary>
    public string PermissionsJson { get; set; } = "{}";

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    protected Role() : base() { }

    /// <summary>
    /// Initializes a new instance of the Role class.
    /// </summary>
    public Role(ulong id, string name, string permissionsJson)
    {
        Id = id;
        Name = name;
        PermissionsJson = permissionsJson;
    }
}
