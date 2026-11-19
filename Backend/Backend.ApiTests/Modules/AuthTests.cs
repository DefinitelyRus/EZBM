using System.Net;
using System.Net.Http.Json;
using Backend.DesktopHost.DTOs;

namespace Backend.ApiTests.Modules;

/// <summary>
/// Endpoint tests for authentication, session tokens, and password updates.
/// </summary>
public static class AuthTests
{
    #region Test Execution

    /// <summary>
    /// Runs all authentication endpoint tests.
    /// </summary>
    public static async Task RunAllAsync(
        TestRunner runner,
        HttpClient client
    )
    {
        runner.PrintSectionHeader("Authentication Endpoints (/api/Auth)");

        string activeToken = string.Empty;

        await runner.ExecuteTestAsync(
            "POST /api/Auth/login (Default Admin)",
            "Logs in with the seeded default admin credentials and expects a session token.",
            async () =>
            {
                LoginRequest request = new("admin", "admin123");
                HttpResponseMessage response = await client.PostAsJsonAsync(
                    "/api/Auth/login",
                    request
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.OK
                );

                LoginResponse? payload =
                    await response.Content.ReadFromJsonAsync<LoginResponse>();
                TestRunner.AssertTrue(
                    payload is not null,
                    "Login response body was empty."
                );
                TestRunner.AssertTrue(
                    !string.IsNullOrWhiteSpace(payload!.Token),
                    "Returned session token was empty."
                );
                TestRunner.AssertEqual("admin", payload.Username, "Username");
                TestRunner.AssertEqual("Admin", payload.Role, "Role");

                activeToken = payload.Token;
                return "Default admin logged in and received a valid session token.";
            }
        );

        await runner.ExecuteTestAsync(
            "POST /api/Auth/login (Invalid Password)",
            "Attempts to log in with a wrong password and expects HTTP 401 Unauthorized.",
            async () =>
            {
                LoginRequest request = new("admin", "wrong-password");
                HttpResponseMessage response = await client.PostAsJsonAsync(
                    "/api/Auth/login",
                    request
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.Unauthorized
                );

                return "Invalid credentials were correctly rejected with HTTP 401.";
            }
        );

        await runner.ExecuteTestAsync(
            "GET /api/Auth/verify (Valid Token)",
            "Checks that the active session token is recognized as valid.",
            async () =>
            {
                HttpResponseMessage response = await client.GetAsync(
                    $"/api/Auth/verify?token={Uri.EscapeDataString(activeToken)}"
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.OK
                );

                return "Active session token was confirmed valid.";
            }
        );

        await runner.ExecuteTestAsync(
            "GET /api/Auth/verify (Invalid Token)",
            "Checks that a fake token is rejected with HTTP 401 Unauthorized.",
            async () =>
            {
                HttpResponseMessage response = await client.GetAsync(
                    "/api/Auth/verify?token=invalid-token-value"
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.Unauthorized
                );

                return "Fake session token was correctly rejected with HTTP 401.";
            }
        );

        await runner.ExecuteTestAsync(
            "POST /api/Auth/change-password",
            "Changes the admin password, verifies old tokens are revoked, and restores the password.",
            async () =>
            {
                ChangePasswordRequest changeReq = new(
                    "admin",
                    "admin123",
                    "newpass456"
                );
                HttpResponseMessage changeRes = await client.PostAsJsonAsync(
                    "/api/Auth/change-password",
                    changeReq
                );
                await TestRunner.AssertStatusCodeAsync(
                    changeRes,
                    HttpStatusCode.OK
                );

                HttpResponseMessage verifyOldRes = await client.GetAsync(
                    $"/api/Auth/verify?token={Uri.EscapeDataString(activeToken)}"
                );
                await TestRunner.AssertStatusCodeAsync(
                    verifyOldRes,
                    HttpStatusCode.Unauthorized
                );

                LoginRequest newLoginReq = new("admin", "newpass456");
                HttpResponseMessage newLoginRes = await client.PostAsJsonAsync(
                    "/api/Auth/login",
                    newLoginReq
                );
                await TestRunner.AssertStatusCodeAsync(
                    newLoginRes,
                    HttpStatusCode.OK
                );

                ChangePasswordRequest restoreReq = new(
                    "admin",
                    "newpass456",
                    "admin123"
                );
                HttpResponseMessage restoreRes = await client.PostAsJsonAsync(
                    "/api/Auth/change-password",
                    restoreReq
                );
                await TestRunner.AssertStatusCodeAsync(
                    restoreRes,
                    HttpStatusCode.OK
                );

                return "Password updated, prior session invalidated, and new password verified.";
            }
        );
    }

    #endregion
}
