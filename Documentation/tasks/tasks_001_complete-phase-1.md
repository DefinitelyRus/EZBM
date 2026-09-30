# Tasks 260929-00

## Current Status

Backend APIs and SQLite models are implemented for products and stock. The static HTML test dashboard is served from `wwwroot`. However, the connection string is missing from `appsettings.json`, and running or bundling the project with a single command still needs verification and configuration.

## Tasks Summary

1. Configure Database Connection String and Verify Single-Command Run
2. Add Single-Command Production Bundle and Publishing Configuration
3. Validate and Test Bundled Deployment

## Tasks

### Task 1: Configure Database Connection String and Verify Single-Command Run

`Backend.DesktopHost` requires a database connection string named `DefaultConnection`. This configuration is not in `appsettings.json` or `appsettings.Development.json`.

Add the SQLite connection string to `appsettings.json`. Then test the local single command startup.

1. Open `Backend/Backend.DesktopHost/appsettings.json`.
2. Add the `ConnectionStrings` section with `DefaultConnection` pointing to the SQLite database file.
3. Start the application using a single command:
    - Run `dotnet run --project Backend/Backend.DesktopHost/Backend.DesktopHost.csproj`.
4. Verify that the backend server starts without errors and hosts `wwwroot` files.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ezbm.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### Things to Consider

- Verify whether the SQLite database file should be in the project root or in a specific data folder.
- Ensure the database migrations apply automatically or exist before testing.

### Task 2: Add Single-Command Production Bundle and Publishing Configuration

Users need a single bundled command or published artifact to execute both the backend and frontend together without secondary build tools.

Configure the project publishing profile and provide simple scripts or instructions for bundling.

1. Verify that `Backend.DesktopHost.csproj` includes `wwwroot` files during publish.
2. Add a publish command or script that creates a self-contained or framework-dependent executable bundle.
3. Verify that running the published output serves both API routes and static frontend files.

#### Things to Consider

- A single command like `dotnet publish -c Release -o ./publish` should produce an executable that serves the static files directly.

### Task 3: Validate and Test Bundled Deployment

Phase 1 requires testing the full MVP to verify that a single command starts the server and serves the user interface.

Test the running application from end to end using manual browser steps or automated checks.

1. Execute the single startup command:
    - `dotnet run --project Backend/Backend.DesktopHost/Backend.DesktopHost.csproj`
2. Open a web browser and navigate to the application URL:
    - Go to `http://localhost:5063` or `https://localhost:7107`.
3. Verify the static dashboard loads:
    - Check if the page title is "EZBM Test Dashboard".
    - Check if the product form, product list, and stock form appear.
4. Perform an end-to-end user workflow:
    - Create a new product using the "Create Product" form.
    - Confirm the product appears in the table after clicking "Refresh Products".
    - Add stock to the product using the "Add Stock" form.
    - Confirm that the total quantity updates correctly.
5. Verify Swagger UI accessibility:
    - Navigate to `/swagger` to confirm the API documentation is functional in development mode.

#### Things to Consider

- Verify whether HTTPS redirection causes issues if certificates are not trusted locally.
- Test port conflicts if port 5063 or 7107 is in use.

## Next Step(s)

1. Apply the connection string in `Backend/Backend.DesktopHost/appsettings.json`.
2. Run the application and execute the validation steps.
3. Update `Documentation/roadmap.md` to mark Phase 1 as complete.

## Affected File(s)

1. `Backend/Backend.DesktopHost/appsettings.json`
2. `Backend/Backend.DesktopHost/appsettings.Development.json`
3. `Documentation/roadmap.md`
