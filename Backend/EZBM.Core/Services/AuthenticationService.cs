using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Tools;
using Microsoft.EntityFrameworkCore;

namespace EZBM.Core.Services;

public static class AuthenticationService
{
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

            if (staff.Password != request.Password)
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
}