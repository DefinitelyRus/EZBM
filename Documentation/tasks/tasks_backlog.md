# Tasks Backlog

## Current Status

This document collects deferred, future, or reference tasks that are not part of the active development sprint. Currently, it includes instructions for documenting the bundled publishing workflow in `Backend.DesktopHost/README.md`, as well as the implementation tasks for business-wide settings (scheduled for Phase 3).

## Tasks Summary

1. Document Bundled Publishing and Execution in DesktopHost README
2. Implement Business Settings Backend (Storage, Dynamic Resolver, and API Endpoints)
3. Build Vanilla HTML Settings UI in `wwwroot`
4. Apply Database Migrations via CLI Tooling

## Tasks

### Task 1: Document Bundled Publishing and Execution in DesktopHost README

Developers and users cannot find instructions in `Backend.DesktopHost/README.md` on how to publish the bundled application or verify the published output.

Add a dedicated documentation section in `Backend/Backend.DesktopHost/README.md` that explains how to publish and run the bundled host.

1. Open `Backend/Backend.DesktopHost/README.md`.
2. Add a new section titled `## Publishing and Bundled Deployment`.
3. Document static asset handling:
    - Note that static assets in `wwwroot` are preserved during publish.
4. Document the framework-dependent publish command:

    ```powershell
    dotnet publish Backend/Backend.DesktopHost/Backend.DesktopHost.csproj -c Release -o ./publish
    ```

5. Document the self-contained single-file publish command for systems without a .NET runtime:

    ```powershell
    dotnet publish Backend/Backend.DesktopHost/Backend.DesktopHost.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
    ```

6. Document how to run and verify the published executable:
    - Change directory to `./publish`.
    - Execute `Backend.DesktopHost.exe`.
    - Open `http://localhost:5000` (or the configured port) to confirm that `index.html` loads.
    - Test an API endpoint like `/api/products` to verify that controllers respond.

#### Things to Consider

- Keep formatting and alert styles consistent with the existing README sections.
- Remind users that the `wwwroot` directory must stay alongside the published binary unless embedded into the assembly.

### Task 2: Implement Business Settings Backend (Storage, Dynamic Resolver, and API Endpoints)

The application lacks a mechanism to persist, resolve, and update business-wide configuration settings outside the primary SQLite database. The database path is hardcoded or read statically from application configuration, preventing users from customizing it.

Implement the backend configuration stack, including domain data models, local JSON persistence, startup resolution for SQLite, and REST API controller endpoints.

#### Subtask 2.1: Create Business Settings Model and Local Storage

The application needs a model and file storage mechanism for configuration values that exist before connecting to the database.

##### Step 2.1.1: Define the Domain Model

Create `BusinessSettings` in `Backend.Core/Models/BusinessSettings.cs` to hold configuration properties.

1. Define a `DatabasePath` string property.
2. Add a static method `GetDefaultDatabasePath()` returning `<user>/Documents/EZBM/business_data/data.db`.
3. Add a helper method `GetResolvedDatabasePath()` that falls back to the default path if none is specified.

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

##### Step 2.1.2: Implement Local File Storage

Add serialization logic to persist settings to `<user>/Documents/EZBM/settings.json`.

1. Create directory structures if they do not exist before writing.
2. Gracefully handle missing or malformed JSON files by returning default settings.

#### Subtask 2.2: Implement Settings Service and Dynamic Resolver

The database context registration in `Program.cs` needs to resolve the active database path dynamically on startup and support reload operations.

##### Step 2.2.1: Create Business Settings Service

Create `IBusinessSettingsService` and `BusinessSettingsService` in `Backend.DesktopHost/Services/`.

1. Add methods to read current settings, update settings, and validate paths.
2. Register the service with the dependency injection container in `Program.cs`.

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

##### Step 2.2.2: Configure Dynamic Connection in Program.cs

Update `Program.cs` to resolve `AppDbContext` using the resolved path from `BusinessSettingsService`.

1. Ensure the parent directory exists before establishing the SQLite connection.
2. Register `AppDbContext` with the dynamically resolved connection string.

#### Subtask 2.3: Build Settings API Endpoints

The frontend client needs HTTP endpoints to inspect and modify settings.

##### Step 2.3.1: Create DTO Records

Create request and response records for settings endpoints according to project conventions.

1. Add `UpdateDatabasePathRequest` to validate incoming paths.
2. Add `GetSettingsResponse` to return active and default paths.

##### Step 2.3.2: Implement SettingsController

Create `SettingsController.cs` in `Backend.DesktopHost/Controllers/`.

1. Add `GET /api/settings` to return current configuration and defaults.
2. Add `PUT /api/settings/database-path` to validate, save the new path, and return the result.

```cs
namespace Backend.DesktopHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    [HttpGet]
    public ActionResult<BusinessSettings> GetSettings()
    {
        return Ok();
    }
}
```

#### Things to Consider

- Validate that candidate paths have a valid `.db` extension and correct file system formatting.
- Notify callers that restarting the host service may be required to reopen SQLite with the new database file.
- Verify write permissions for target folders before applying changes.

### Task 3: Build Vanilla HTML Settings UI in `wwwroot`

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

### Task 4: Apply Database Migrations via CLI Tooling

Developers and automated CI/CD pipelines need a way to create and update database schemas explicitly without running the desktop host application.

Use the `dotnet ef` command line interface to apply pending migrations directly to target SQLite databases.

1. Open a terminal at the repository root.
2. Verify that the .NET EF global tool is available or install it:

   ```bash
   dotnet tool install --global dotnet-ef
   ```

3. Run the database update command specifying the core data project and startup project:

   ```bash
   dotnet ef database update \
       --project Backend/Backend.Core/Backend.Core.csproj \
       --startup-project Backend/Backend.DesktopHost/Backend.DesktopHost.csproj
   ```

4. Verify that SQLite tables exist:
   - Check that the configured database file (`TEST_DATA.db` or specified database) contains all schema tables.
   - Confirm that the `__EFMigrationsHistory` table records applied migrations.

#### Things to Consider

- When updating a published bundle, ensure the resulting `.db` file is located in the bundle directory or reachable by the connection string.
- In sandboxed environments or on fresh machines, global tool installation permissions may require elevated access.

## Next Step(s)

1. Review and apply the publishing documentation to `Backend/Backend.DesktopHost/README.md`.
2. Address the settings tasks during Phase 3 according to the roadmap schedule.
3. Use CLI migrations in environments where automatic startup migration is disabled.

## Affected File(s)

1. `Backend/Backend.DesktopHost/README.md`
2. `Backend/Backend.Core/Models/BusinessSettings.cs`
3. `Backend/Backend.DesktopHost/Services/BusinessSettingsService.cs`
4. `Backend/Backend.DesktopHost/Controllers/SettingsController.cs`
5. `Backend/Backend.DesktopHost/Program.cs`
6. `Backend/Backend.DesktopHost/wwwroot/index.html`
7. `Backend/Backend.DesktopHost/wwwroot/js/app.js`
8. `Documentation/roadmap.md`

