using System.Net;
using System.Net.Http.Json;
using Backend.DesktopHost.DTOs;

namespace Backend.ApiTests.Modules;

/// <summary>
/// Endpoint tests for employee attendance logging and payroll calculations.
/// </summary>
public static class AttendanceAndPayrollTests
{
    #region Test Execution

    /// <summary>
    /// Runs all attendance and payroll endpoint tests.
    /// </summary>
    public static async Task RunAllAsync(
        TestRunner runner,
        HttpClient client
    )
    {
        runner.PrintSectionHeader(
            "Attendance & Payroll (/api/Attendance, /api/Payroll)"
        );

        await runner.ExecuteTestAsync(
            "POST /api/Attendance/clock (Clock In & Clock Out)",
            "Logs a clock-in followed by a clock-out for user ID 1.",
            async () =>
            {
                ClockActionRequest clockIn = new(1, "In", "Morning shift start");
                HttpResponseMessage inRes = await client.PostAsJsonAsync(
                    "/api/Attendance/clock",
                    clockIn
                );
                await TestRunner.AssertStatusCodeAsync(
                    inRes,
                    HttpStatusCode.Created
                );

                AttendanceResponse? inPayload = await inRes.Content
                    .ReadFromJsonAsync<AttendanceResponse>();
                TestRunner.AssertEqual("In", inPayload!.ActionType, "ActionType");

                await Task.Delay(50);

                ClockActionRequest clockOut = new(1, "Out", "Shift end");
                HttpResponseMessage outRes = await client.PostAsJsonAsync(
                    "/api/Attendance/clock",
                    clockOut
                );
                await TestRunner.AssertStatusCodeAsync(
                    outRes,
                    HttpStatusCode.Created
                );

                AttendanceResponse? outPayload = await outRes.Content
                    .ReadFromJsonAsync<AttendanceResponse>();
                TestRunner.AssertEqual(
                    "Out",
                    outPayload!.ActionType,
                    "ActionType"
                );

                return "Clock-in and clock-out events were recorded with HTTP 201.";
            }
        );

        await runner.ExecuteTestAsync(
            "GET /api/Attendance/history",
            "Retrieves attendance history for user ID 1 and confirms both entries exist.",
            async () =>
            {
                HttpResponseMessage response = await client.GetAsync(
                    "/api/Attendance/history?userId=1"
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.OK
                );

                List<AttendanceResponse>? history = await response.Content
                    .ReadFromJsonAsync<List<AttendanceResponse>>();
                TestRunner.AssertTrue(
                    history is not null && history.Count >= 2,
                    "Expected at least 2 attendance records in history."
                );

                return $"Attendance history returned {history!.Count} record(s) for user ID 1.";
            }
        );

        await runner.ExecuteTestAsync(
            "GET /api/Payroll/summary",
            "Calculates shift hours and gross pay at 100.00/hour for user ID 1.",
            async () =>
            {
                HttpResponseMessage response = await client.GetAsync(
                    "/api/Payroll/summary?userId=1&hourlyRate=100"
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.OK
                );

                PayrollSummaryResponse? summary = await response.Content
                    .ReadFromJsonAsync<PayrollSummaryResponse>();
                TestRunner.AssertTrue(
                    summary is not null,
                    "Payroll summary response was empty."
                );
                TestRunner.AssertEqual(1L, summary!.UserId, "UserId");
                TestRunner.AssertEqual(
                    0,
                    summary.UnpairedClockIns,
                    "UnpairedClockIns"
                );
                TestRunner.AssertTrue(
                    summary.Shifts.Count >= 1 && summary.Shifts[0].IsCompleted,
                    "Expected at least one completed shift."
                );

                return "Payroll summary paired the shift cleanly with 0 unpaired clock-ins.";
            }
        );
    }

    #endregion
}
