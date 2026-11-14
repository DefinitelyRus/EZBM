namespace Backend.DesktopHost.DTOs;

/// <summary>
/// Request record for user authentication.
/// </summary>
public record LoginRequest(
    string Username,
    string Password
);

/// <summary>
/// Response record returned upon successful authentication.
/// </summary>
public record LoginResponse(
    string Token,
    string Username,
    string Role,
    DateTime ExpiresAt
);

/// <summary>
/// Request record for changing credentials.
/// </summary>
public record ChangePasswordRequest(
    string Username,
    string CurrentPassword,
    string NewPassword
);
