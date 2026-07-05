using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Tools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EZBM.Core.Services;

/// <summary>
/// Provides secure password hashing and authentication services.
/// </summary>
public static class AuthenticationService
{
    #region Password Hashing

    /// <summary>
    /// Hashes a plain-text password using PBKDF2/SHA-256.
    /// </summary>
    public static string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        byte[] pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations: 10000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );
        byte[] hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(pbkdf2, 0, hashBytes, 16, 32);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifies a password against a PBKDF2/SHA-256 hash.
    /// </summary>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            byte[] pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations: 10000,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: 32
            );
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != pbkdf2[i]) return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Validates login credentials and returns a request result.
    /// </summary>
    public static async Task<Utils.RequestResult<string>> LoginAsync(LoginRequest request)
    {
        string message;

        try
        {
            using AppDbContext context = new();

            Staff? staff = await context.Staff.FirstOrDefaultAsync(
                s => s.Username == request.Username
            );

            if (staff is null)
            {
                message = $"Staff with username '{request.Username}' not found.";
                Log.Me(message);
                Utils.RequestResult<string> failResult = new(
                    Utils.Result.Failed_NoResults, message, null
                );

                return failResult;
            }

            if (!VerifyPassword(request.Password, staff.Password ?? string.Empty))
            {
                message = "Invalid password.";
                Log.Me(message);
                Utils.RequestResult<string> failResult = new(
                    Utils.Result.Failed_InvalidQuery, message, null
                );

                return failResult;
            }

            message = $"Staff '{staff.Username}' logged in successfully.";
            Log.Me(message);
            Utils.RequestResult<string> successResult = new(
                Utils.Result.Success, message, staff.Id.ToString()
            );

            return successResult;
        }

        catch (Exception ex)
        {
            message = $"Error when logging in: {ex.Message}";
            Log.Me(message);
            Utils.RequestResult<string> errorResult = new(
                Utils.Result.Failed_UnhandledException, message, null
            );

            return errorResult;
        }
    }

    #endregion
}