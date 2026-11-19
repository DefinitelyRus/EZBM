using System.Net;
using System.Net.Http.Json;
using Backend.DesktopHost.DTOs;

namespace Backend.ApiTests.Modules;

/// <summary>
/// Endpoint tests for system audit logging and business settings configuration.
/// </summary>
public static class AuditLogsAndSettingsTests
{
    #region Test Execution

    /// <summary>
    /// Runs all audit log and settings endpoint tests.
    /// </summary>
    public static async Task RunAllAsync(
        TestRunner runner,
        HttpClient client
    )
    {
        runner.PrintSectionHeader(
            "Audit Logs & Business Settings (/api/AuditLogs, /api/Settings)"
        );

        await runner.ExecuteTestAsync(
            "POST /api/AuditLogs and GET /api/AuditLogs",
            "Writes a security warning event and queries it back using filters.",
            async () =>
            {
                CreateAuditLogRequest createReq = new(
                    EventType: "LoginWarning",
                    Message: "Simulated repeated login attempt",
                    Severity: "Warning",
                    Source: "ApiTests",
                    UserId: 1
                );

                HttpResponseMessage createRes = await client.PostAsJsonAsync(
                    "/api/AuditLogs",
                    createReq
                );
                await TestRunner.AssertStatusCodeAsync(
                    createRes,
                    HttpStatusCode.Created
                );

                AuditEventLogDto? created = await createRes.Content
                    .ReadFromJsonAsync<AuditEventLogDto>();
                TestRunner.AssertTrue(
                    created is not null && created.Id > 0,
                    "Audit log entry was not assigned an ID."
                );

                HttpResponseMessage getRes = await client.GetAsync(
                    "/api/AuditLogs?eventType=LoginWarning&severity=Warning"
                );
                await TestRunner.AssertStatusCodeAsync(
                    getRes,
                    HttpStatusCode.OK
                );

                List<AuditEventLogDto>? logs = await getRes.Content
                    .ReadFromJsonAsync<List<AuditEventLogDto>>();
                TestRunner.AssertTrue(
                    logs is not null && logs.Any(l => l.Id == created!.Id),
                    "Created audit event was missing from filtered query."
                );

                return $"Recorded and retrieved audit event ID {created!.Id}.";
            }
        );

        await runner.ExecuteTestAsync(
            "GET /api/Settings",
            "Retrieves active and default database path settings.",
            async () =>
            {
                HttpResponseMessage response =
                    await client.GetAsync("/api/Settings");
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.OK
                );

                GetSettingsResponse? settings = await response.Content
                    .ReadFromJsonAsync<GetSettingsResponse>();
                TestRunner.AssertTrue(
                    settings is not null &&
                    !string.IsNullOrWhiteSpace(settings.ActiveDatabasePath) &&
                    !string.IsNullOrWhiteSpace(settings.DefaultDatabasePath),
                    "Settings response did not include active and default paths."
                );

                return "Settings endpoint returned both active and default database paths.";
            }
        );

        await runner.ExecuteTestAsync(
            "PUT /api/Settings/database-path (Valid & Invalid) and POST reset",
            "Updates the database path, verifies invalid extensions fail, and resets to default.",
            async () =>
            {
                string tempDb = Path.Combine(
                    Path.GetTempPath(),
                    "ezbm_custom_test.db"
                );
                UpdateDatabasePathRequest validReq = new(tempDb);
                HttpResponseMessage updateRes = await client.PutAsJsonAsync(
                    "/api/Settings/database-path",
                    validReq
                );
                await TestRunner.AssertStatusCodeAsync(
                    updateRes,
                    HttpStatusCode.OK
                );

                UpdateDatabasePathRequest invalidReq = new("invalid_file.txt");
                HttpResponseMessage invalidRes = await client.PutAsJsonAsync(
                    "/api/Settings/database-path",
                    invalidReq
                );
                await TestRunner.AssertStatusCodeAsync(
                    invalidRes,
                    HttpStatusCode.BadRequest
                );

                HttpResponseMessage resetRes = await client.PostAsync(
                    "/api/Settings/reset-database-path",
                    null
                );
                await TestRunner.AssertStatusCodeAsync(
                    resetRes,
                    HttpStatusCode.OK
                );

                return "Valid path update succeeded, .txt path was rejected with HTTP 400, and reset succeeded.";
            }
        );
    }

    #endregion
}
