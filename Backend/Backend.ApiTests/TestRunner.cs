using System.Net;

namespace Backend.ApiTests;

/// <summary>
/// Coordinates test execution, assertions, and plain-language console reporting.
/// </summary>
public class TestRunner
{
    private int _totalTests;
    private int _passedTests;
    private int _failedTests;

    /// <summary>
    /// Gets the total number of failed tests.
    /// </summary>
    public int FailedTests => _failedTests;

    #region Execution & Reporting

    /// <summary>
    /// Prints a section header for a group of related endpoint tests.
    /// </summary>
    public void PrintSectionHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"================================================================================");
        Console.WriteLine($"  {title}");
        Console.WriteLine($"================================================================================");
        Console.ResetColor();
        Console.WriteLine();
    }

    /// <summary>
    /// Runs a single API test and prints a plain-word summary of the outcome.
    /// </summary>
    public async Task ExecuteTestAsync(
        string testName,
        string description,
        Func<Task<string>> testAction
    )
    {
        _totalTests++;
        Console.WriteLine($"[TEST {_totalTests}] {testName}");
        Console.WriteLine($"  What it checks: {description}");

        try
        {
            string successDetails = await testAction();
            _passedTests++;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  Result: PASSED - {successDetails}");
            Console.ResetColor();
        }

        catch (Exception ex)
        {
            _failedTests++;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  Result: FAILED - {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Prints the final pass/fail summary across all executed tests.
    /// </summary>
    public void PrintSummary()
    {
        Console.ForegroundColor =
            _failedTests == 0 ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"================================================================================");
        Console.WriteLine($"  TEST SUITE SUMMARY");
        Console.WriteLine($"  Total: {_totalTests} | Passed: {_passedTests} | Failed: {_failedTests}");
        Console.WriteLine($"================================================================================");
        Console.ResetColor();
    }

    #endregion

    #region Assertions

    /// <summary>
    /// Verifies that an HTTP response has the expected status code.
    /// </summary>
    public static async Task AssertStatusCodeAsync(
        HttpResponseMessage response,
        HttpStatusCode expected
    )
    {
        if (response.StatusCode == expected) return;

        string body = await response.Content.ReadAsStringAsync();
        string message =
            $"Expected HTTP {(int)expected} ({expected}), " +
            $"but received HTTP {(int)response.StatusCode} " +
            $"({response.StatusCode}). Server response: {body}";
        throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Verifies that a boolean condition is true.
    /// </summary>
    public static void AssertTrue(bool condition, string failureReason)
    {
        if (condition) return;

        throw new InvalidOperationException(failureReason);
    }

    /// <summary>
    /// Verifies that two values are equal.
    /// </summary>
    public static void AssertEqual<T>(
        T expected,
        T actual,
        string fieldName
    )
    {
        if (EqualityComparer<T>.Default.Equals(expected, actual)) return;

        string message =
            $"Expected {fieldName} to be '{expected}', " +
            $"but got '{actual}'.";
        throw new InvalidOperationException(message);
    }

    #endregion
}
