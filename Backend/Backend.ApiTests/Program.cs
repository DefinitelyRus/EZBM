using Backend.ApiTests.Modules;

namespace Backend.ApiTests;

/// <summary>
/// Console entry point for running the EZBM API endpoint test suite.
/// </summary>
public static class ApiTestProgram
{
    /// <summary>
    /// Executes all API endpoint tests and returns 0 when all pass.
    /// </summary>
    public static async Task<int> Main(string[] args)
    {
        Console.WriteLine("Starting EZBM API Endpoint Test Suite...");
        Console.WriteLine("Spinning up isolated in-process test server and SQLite database.");
        Console.WriteLine();

        using TestWebApplicationFactory factory = new();
        using HttpClient client = factory.CreateClient();
        TestRunner runner = new();

        await AuthTests.RunAllAsync(runner, client);
        await ProductsAndStockTests.RunAllAsync(runner, client);
        await PosAndPaymentTests.RunAllAsync(runner, client, factory);
        await AttendanceAndPayrollTests.RunAllAsync(runner, client);
        await AuditLogsAndSettingsTests.RunAllAsync(runner, client);

        runner.PrintSummary();

        return runner.FailedTests == 0 ? 0 : 1;
    }
}
