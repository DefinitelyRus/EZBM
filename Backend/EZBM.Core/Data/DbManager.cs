using EZBM.Core.Tools;
using System.Linq;

namespace EZBM.Core.Data;

/// <summary>
/// Manages the database lifecycle, including initialization and error handling.
/// </summary>
public static class DbManager
{

    /// <summary>
    /// The name of the database file.
    /// </summary>
    public static string DbFileName { get; set; } = "business_data.db";

    /// <summary>
    /// The full system path to the database file.
    /// </summary>
    public static string DbFilePath => Path.Combine(Utils.UserSavePath, DbFileName);

    /// <summary>
    /// Occurs when the database fails to initialize.
    /// </summary>
    public static event EventHandler<DbInitializationFailedEventArgs>? InitializationFailed;


    /// <summary>
    /// Triggers the InitializationFailed event.
    /// </summary>
    /// <param name="e">The event data.</param>
    private static void OnInitializationFailed(DbInitializationFailedEventArgs e)
    {
        InitializationFailed?.Invoke(null, e);
    }


    /// <summary>
    /// Ensures the database is created and ready for use.
    /// </summary>
    public static void Initialize()
    {
        try
        {
            using AppDbContext context = new();
            context.Database.EnsureCreated();
        }
        catch (Exception e)
        {
            Log.Err($"Error initializing database: {e.Message}");

            // Broadcast a signal that the database failed to initialize.
            // (To prompt the user to take action)
            // The UI may take several forms, so this must be front-end agnostic.

            DbInitializationFailedEventArgs dbException = new(e, "Database is corrupted or inaccessible.");
            OnInitializationFailed(dbException);
        }
    }


    /// <summary>
    /// Deletes the existing database file and creates a fresh one.
    /// </summary>
    public static void Reset()
    {
        try
        {
            // Kill active SQLite database connections
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Delete the database file
            if (File.Exists(DbFilePath)) File.Delete(DbFilePath);

            // Create a new database
            using AppDbContext context = new();
            context.Database.EnsureCreated();
        }
        catch (Exception e)
        {
            Log.Err(() => $"Critical error during database reset: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Configures the database filename and optionally triggers randomized seeder population based on command line arguments.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void ConfigureFromArgs(
        string[] args
    )
    {
        if (args is null) return;

        bool useTest = args.Contains("-t") || args.Contains("--use_test_data");
        bool generateTest = args.Contains("-g") || args.Contains("--generate_test_data");

        if (useTest || generateTest)
        {
            DbFileName = "test_data.db";
        }

        if (generateTest)
        {
            Reset();
            DataSeeder.Seed(true);
        }
    }
}



/// <summary>
/// Provides data for the database initialization failure event.
/// </summary>
/// <param name="e">The exception that caused the failure.</param>
/// <param name="message">A descriptive error message.</param>
public class DbInitializationFailedEventArgs(Exception e, string message) : EventArgs
{
    /// <summary>
    /// The exception captured during initialization.
    /// </summary>
    public Exception Exception { get; } = e;

    /// <summary>
    /// The error message describing the failure.
    /// </summary>
    public string Message { get; } = message;
}