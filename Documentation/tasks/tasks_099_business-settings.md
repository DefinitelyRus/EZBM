# Tasks 260930-02

## Current Status

The application currently reads its database connection string from application configuration files. There is no in-app mechanism to read, store, or change business-wide settings such as the database file location. The default storage location must be `<user>/Documents/EZBM/business_data/data.db`, and users must be able to change this setting directly inside the app.

## Tasks Summary

1. Create Business Settings Storage and Model
2. Build Settings Service and Dynamic Database Resolver
3. Build Settings Controller and REST API Endpoints
4. Build Vanilla HTML Settings UI in `wwwroot`

## Tasks

### Task 1: Create Business Settings Storage and Model

The application needs a dedicated model and local file storage mechanism for application settings that exist outside the primary database.

Create a settings model class and a local JSON storage provider to load and persist business configuration.

1. Create a `BusinessSettings` model in `Backend.Core/Models/BusinessSettings.cs`.
2. Define properties for business configuration:
    - Add `DatabasePath` as a string property.
3. Add a helper method to resolve the default database path:
    - Retrieve the path to the user's `Documents` folder.
    - Append `EZBM/business_data/data.db`.
4. Create a settings file provider to serialize and deserialize the configuration to a `settings.json` file in `<user>/Documents/EZBM/settings.json`.

```cs
namespace Backend.Core.Models;

public class BusinessSettings
{
    public string DatabasePath { get; set; } = string.Empty;

    public static string GetDefaultDatabasePath()
    {
        string documentsFolder = Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments
        );
        string businessDataFolder = Path.Combine(
            documentsFolder,
            "EZBM",
            "business_data"
        );
        return Path.Combine(businessDataFolder, "data.db");
    }

    public string GetResolvedDatabasePath()
    {
        if (string.IsNullOrWhiteSpace(DatabasePath))
        {
            return GetDefaultDatabasePath();
        }

        return DatabasePath;
    }
}
```

#### Things to Consider

- Create parent directories automatically if they do not exist before writing the settings file.
- Handle corrupted or invalid JSON gracefully by falling back to defaults.

### Task 2: Build Settings Service and Dynamic Database Resolver

The database context setup in `Program.cs` needs to resolve the active database path dynamically on startup and support reload operations.

Implement a business settings service and register it with the dependency injection container.

1. Create `IBusinessSettingsService` and `BusinessSettingsService` in `Backend.DesktopHost/Services/`.
2. Add methods to read settings, update settings, and validate candidate database file paths.
3. Update `Program.cs` to resolve `AppDbContext` using the resolved path from `BusinessSettingsService`.
4. Ensure the directory path exists before initializing the SQLite connection.

```cs
namespace Backend.DesktopHost.Services;

public class BusinessSettingsService
{
    public BusinessSettings LoadSettings()
    {
        string documentsFolder = Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments
        );
        string configPath = Path.Combine(
            documentsFolder,
            "EZBM",
            "settings.json"
        );

        if (!File.Exists(configPath))
        {
            BusinessSettings defaultSettings = new();
            return defaultSettings;
        }

        string rawJson = File.ReadAllText(configPath);
        BusinessSettings? settings = System.Text.Json.JsonSerializer
            .Deserialize<BusinessSettings>(rawJson);
        return settings ?? new();
    }
}
```

#### Things to Consider

- When the database path is modified, indicate to the user that a restart of the server application may be necessary to reopen the SQLite context.
- Verify write permissions for the requested directory path.

### Task 3: Build Settings Controller and REST API Endpoints

The frontend interface needs HTTP endpoints to inspect and update business configuration.

Create `SettingsController` with endpoints to retrieve and modify the settings.

1. Create `SettingsController.cs` in `Backend.DesktopHost/Controllers/`.
2. Add a `GET` endpoint `/api/settings` to return current configuration and defaults.
3. Add a `PUT` endpoint `/api/settings/database-path` to validate and update the path.
4. Add DTO records for requests and responses according to project conventions.

```cs
namespace Backend.DesktopHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    [HttpGet]
    public ActionResult<BusinessSettings> GetSettings()
    {
        // Return active settings
        return Ok();
    }
}
```

#### Things to Consider

- Validate that the specified path has a valid `.db` extension and a valid file system path format.
- Return descriptive error messages if the directory cannot be created.

### Task 4: Build Vanilla HTML Settings UI in `wwwroot`

Users must be able to view and change business settings from the browser interface.

Add a settings section to `wwwroot/index.html` and connect it with `app.js`.

1. Add a settings form and fieldset in `Backend.DesktopHost/wwwroot/index.html`.
2. Add input fields to display the active database file path and default path.
3. Add a button to reset the path to the default location.
4. Add JavaScript handlers in `wwwroot/js/app.js` to fetch and submit changes via the settings API.
5. Display confirmation and restart notices upon successful save.

#### Things to Consider

- Keep styles consistent with the existing minimal layout in `wwwroot/css/style.css`.
- Ensure plain JavaScript `fetch` handles both success and failure responses.

## Next Step(s)

1. Implement the `BusinessSettings` class in `Backend.Core`.
2. Connect the dynamic resolver into `Program.cs`.
3. Add the controller endpoints and test from the browser UI.

## Affected File(s)

1. `Backend/Backend.Core/Models/BusinessSettings.cs`
2. `Backend/Backend.DesktopHost/Services/BusinessSettingsService.cs`
3. `Backend/Backend.DesktopHost/Controllers/SettingsController.cs`
4. `Backend/Backend.DesktopHost/Program.cs`
5. `Backend/Backend.DesktopHost/wwwroot/index.html`
6. `Backend/Backend.DesktopHost/wwwroot/js/app.js`
7. `Documentation/roadmap.md`
