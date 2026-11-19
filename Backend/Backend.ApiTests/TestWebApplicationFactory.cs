using System.Text.Json;
using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backend.ApiTests;

/// <summary>
/// Isolated test web application factory using a temporary SQLite database.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _tempDirectory;
    private readonly string _databasePath;

    /// <summary>
    /// Initializes a new instance with an isolated temporary directory.
    /// </summary>
    public TestWebApplicationFactory()
    {
        string folderName = $"ezbm_api_tests_{Guid.NewGuid():N}";
        _tempDirectory = Path.Combine(Path.GetTempPath(), folderName);
        Directory.CreateDirectory(_tempDirectory);
        _databasePath = Path.Combine(_tempDirectory, "test_data.db");
    }

    #region Host Configuration

    /// <summary>
    /// Configures the test web host with isolated database and settings.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
            );

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            string connectionString = $"Data Source={_databasePath}";
            services.AddDbContext<AppDbContext>(
                options => options.UseSqlite(connectionString)
            );

            ServiceDescriptor? settingsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IBusinessSettingsService)
            );

            if (settingsDescriptor is not null)
            {
                services.Remove(settingsDescriptor);
            }

            TestBusinessSettingsService testSettings = new(_tempDirectory);
            services.AddSingleton<IBusinessSettingsService>(testSettings);
        });
    }

    /// <summary>
    /// Releases temporary database and settings files after tests finish.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing) return;

        SqliteConnection.ClearAllPools();

        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        catch
        {
            // Ignore transient file lock cleanup errors on exit.
        }
    }

    #endregion
}

/// <summary>
/// Test implementation of <see cref="IBusinessSettingsService"/> using a temp path.
/// </summary>
public class TestBusinessSettingsService(
    string tempDirectory
) : IBusinessSettingsService
{
    private readonly string _settingsFilePath =
        Path.Combine(tempDirectory, "settings.json");

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    #region Settings Operations

    /// <summary>
    /// Loads settings from the temporary test settings file.
    /// </summary>
    public BusinessSettings LoadSettings()
    {
        if (!File.Exists(_settingsFilePath))
        {
            return new BusinessSettings();
        }

        string rawJson = File.ReadAllText(_settingsFilePath);
        BusinessSettings? settings =
            JsonSerializer.Deserialize<BusinessSettings>(rawJson, _jsonOptions);
        return settings ?? new BusinessSettings();
    }

    /// <summary>
    /// Saves settings to the temporary test settings file.
    /// </summary>
    public void SaveSettings(BusinessSettings settings)
    {
        string rawJson = JsonSerializer.Serialize(settings, _jsonOptions);
        File.WriteAllText(_settingsFilePath, rawJson);
    }

    /// <summary>
    /// Resolves the database path from temporary settings.
    /// </summary>
    public string GetResolvedDatabasePath()
    {
        BusinessSettings settings = LoadSettings();
        return settings.GetResolvedDatabasePath();
    }

    /// <summary>
    /// Validates that a candidate database path ends with .db.
    /// </summary>
    public bool ValidateDatabasePath(string path, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            errorMessage = "Database path cannot be empty.";
            return false;
        }

        string extension = Path.GetExtension(path);
        if (!extension.Equals(".db", StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "Database file must have a .db extension.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    #endregion
}
