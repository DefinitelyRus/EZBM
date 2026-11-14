using System.Collections.Concurrent;
using System.Security.Cryptography;
using Backend.Core.Common;
using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.DesktopHost.Services;

/// <summary>
/// Service managing authentication, password verification, and session tokens.
/// </summary>
public class AuthService(AppDbContext context)
{
    private readonly AppDbContext _context = context;
    private static readonly ConcurrentDictionary<string, SessionInfo>
        _activeSessions = new();

    private record SessionInfo(
        long UserId,
        string Username,
        string Role,
        DateTime ExpiresAt
    );

    #region Authentication Methods

    /// <summary>
    /// Authenticates user credentials and creates an active session.
    /// </summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw Esc.New<ArgumentException>(
                "Username and password are required."
            );
        }

        User? user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null)
        {
            throw Esc.New<UnauthorizedAccessException>(
                "Invalid username or password."
            );
        }

        bool isValid = PasswordHasher.VerifyPassword(
            request.Password,
            user.Salt,
            user.PasswordHash
        );

        if (!isValid)
        {
            throw Esc.New<UnauthorizedAccessException>(
                "Invalid username or password."
            );
        }

        string token = Convert.ToHexString(
            RandomNumberGenerator.GetBytes(32)
        );
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);

        SessionInfo session = new(
            user.Id,
            user.Username,
            user.Role,
            expiresAt
        );
        _activeSessions[token] = session;

        LoginResponse response = new(
            Token: token,
            Username: user.Username,
            Role: user.Role,
            ExpiresAt: expiresAt
        );

        return response;
    }

    /// <summary>
    /// Validates an active session token.
    /// </summary>
    public bool VerifySession(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;

        if (_activeSessions.TryGetValue(token, out SessionInfo? session))
        {
            if (DateTime.UtcNow <= session.ExpiresAt) return true;

            _activeSessions.TryRemove(token, out _);
        }

        return false;
    }

    /// <summary>
    /// Updates the user's password after verifying current credentials.
    /// </summary>
    public async Task ChangePasswordAsync(ChangePasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw Esc.New<ArgumentException>(
                "New password cannot be empty."
            );
        }

        User? user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null)
        {
            throw Esc.New<ArgumentException>(
                "User does not exist.",
                request.Username
            );
        }

        bool isValid = PasswordHasher.VerifyPassword(
            request.CurrentPassword,
            user.Salt,
            user.PasswordHash
        );

        if (!isValid)
        {
            throw Esc.New<UnauthorizedAccessException>(
                "Current password is incorrect."
            );
        }

        string newSalt = PasswordHasher.GenerateSalt();
        string newHash = PasswordHasher.HashPassword(
            request.NewPassword,
            newSalt
        );

        user.Salt = newSalt;
        user.PasswordHash = newHash;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds default admin user credentials if no users exist.
    /// </summary>
    public async Task EnsureAdminUserExistsAsync()
    {
        bool hasUsers = await _context.Users.AnyAsync();
        if (hasUsers) return;

        string salt = PasswordHasher.GenerateSalt();
        string hash = PasswordHasher.HashPassword("admin123", salt);

        User defaultAdmin = new()
        {
            Username = "admin",
            PasswordHash = hash,
            Salt = salt,
            Role = "Admin"
        };

        await _context.Users.AddAsync(defaultAdmin);
        await _context.SaveChangesAsync();
    }

    #endregion
}
