using System.Threading;

namespace EZBM.Core.Data;

/// <summary>
/// Provides access to the ambient logged-in user context in asynchronous call stacks.
/// </summary>
public static class CurrentUserContext
{
    private static readonly AsyncLocal<string?> _currentUsername = new();

    /// <summary>
    /// Gets or sets the username of the current request operator.
    /// </summary>
    public static string? Username
    {
        get => _currentUsername.Value;
        set => _currentUsername.Value = value;
    }
}
