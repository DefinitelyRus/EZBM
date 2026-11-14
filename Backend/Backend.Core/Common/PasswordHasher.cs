using System.Security.Cryptography;

namespace Backend.Core.Common;

/// <summary>
/// Helper for password hashing and verification using PBKDF2.
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    /// <summary>
    /// Generates a cryptographic salt string.
    /// </summary>
    public static string GenerateSalt()
    {
        byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        return Convert.ToBase64String(saltBytes);
    }

    /// <summary>
    /// Computes a hash string for a password using the specified salt.
    /// </summary>
    public static string HashPassword(string password, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifies that a plaintext password matches the stored password hash.
    /// </summary>
    public static bool VerifyPassword(
        string password,
        string salt,
        string storedHash
    )
    {
        string computedHash = HashPassword(password, salt);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computedHash),
            Convert.FromBase64String(storedHash)
        );
    }
}
