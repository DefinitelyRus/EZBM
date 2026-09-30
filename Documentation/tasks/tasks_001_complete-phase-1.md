# Tasks 260929-00

## Current Status

Database connection configuration, bundled publishing scripts, and automatic database migration on startup are implemented and validated. The application successfully applies migrations to SQLite on launch, serves the static dashboard, and allows creating and querying products. Phase 1 validation is in progress.

## Tasks Summary

1. (COMPLETE) Configure Database Connection String and Verify Single-Command Run
2. (COMPLETE) Add Single-Command Production Bundle and Publishing Configuration
3. (COMPLETE) Automatically Apply Database Migrations on Startup
4. (COMPLETE) Validate and Test Bundled Deployment
5. Fix Stock Addition and Total Quantity Update

## Tasks

### Task 5: Fix Stock Addition and Total Quantity Update

Submitting the "Add Stock" form in the test dashboard does not update the total stock quantity for the target product.

Connect the "Add Stock" form in `wwwroot/js/app.js` to call the stock creation API and ensure the updated stock quantity reflects across the dashboard.

1. Inspect `Backend/Backend.DesktopHost/wwwroot/js/app.js`.
2. Add a submit event listener for `create-product-stock-form`:
    - Read input values for product ID, quantity, cost, expiry date, batch notes, and transaction notes.
    - Set the target prefix to `create-product-stock`.
3. Call `createProductStock` with the payload:
    - Verify that `createProductStock` sends a `POST` request to `/api/ProductStocks/create-stock`.
4. Call `loadProducts` after a successful stock addition to refresh the table.
5. Verify in the browser:
    - Open `index.html`.
    - Submit stock for an existing product.
    - Confirm that the "Total Quantity" column in the product table displays the updated quantity.

#### Things to Consider

- Verify whether `Backend/Backend.DesktopHost/Controllers/ProductStocksController.cs` or `ProductTransactionService.cs` updates the product entity and commits the transaction cleanly.
- Ensure input validation handles decimal quantities and non-empty product IDs.

### (COMPLETE) Task 4: Validate and Test Bundled Deployment

Phase 1 requires testing the full MVP to verify that a single command starts the server and serves the user interface.

Test the running application from end to end using manual browser steps or automated checks.

1. (COMPLETE) Execute the single startup command:
    - (COMPLETE) `dotnet run --project Backend/Backend.DesktopHost/Backend.DesktopHost.csproj`
2. (COMPLETE) Open a web browser and navigate to the application URL:
    - (COMPLETE) Go to `http://localhost:5063` or `https://localhost:7107`.
3. (COMPLETE) Verify Swagger UI accessibility:
    - (COMPLETE) Navigate to `/swagger` to confirm the API documentation is functional in development mode.
4. (COMPLETE) Perform an end-to-end user workflow:
    - (COMPLETE) Create a new product using the "Create Product" form.
    - (COMPLETE) Confirm the product appears in the table after clicking "Refresh Products".
5. (COMPLETE) Verify the static dashboard loads:
    - (COMPLETE) Check if the page title is "EZBM Test Dashboard".
    - (COMPLETE) Check if the product form, product list, and stock form appear.

#### Things to Consider

- Verify whether HTTPS redirection causes issues if certificates are not trusted locally.
- Test port conflicts if port 5063 or 7107 is in use.

### (COMPLETE) Task 1: Configure Database Connection String and Verify Single-Command Run

`Backend.DesktopHost` requires a database connection string named `DefaultConnection`. This configuration is not in `appsettings.json` or `appsettings.Development.json`.

Add the SQLite connection string to `appsettings.json`. Then test the local single command startup.

1. (COMPLETE) Open `Backend/Backend.DesktopHost/appsettings.json`.
2. (COMPLETE) Add the `ConnectionStrings` section with `DefaultConnection` pointing to the SQLite database file.
3. (COMPLETE) Start the application using a single command:
    - (COMPLETE) Run `dotnet run --project Backend/Backend.DesktopHost/Backend.DesktopHost.csproj`.
4. (COMPLETE) Verify that the backend server starts without errors and hosts `wwwroot` files.

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

### (COMPLETE) Task 2: Add Single-Command Production Bundle and Publishing Configuration

Users need a single bundled command or published artifact to execute both the backend and frontend together without secondary build tools.

Configure the project publishing profile and provide simple scripts or instructions for bundling.

1. (COMPLETE) Verify static asset inclusion in `Backend/Backend.DesktopHost/Backend.DesktopHost.csproj`.
    - (COMPLETE) Open `Backend/Backend.DesktopHost/Backend.DesktopHost.csproj`.
    - (COMPLETE) Confirm that `Microsoft.NET.Sdk.Web` is present.
    - (COMPLETE) Add an item group to copy `wwwroot` files during publish if needed:

      ```xml
      <ItemGroup>
        <Content Update="wwwroot\**" CopyToPublishDirectory="PreserveNewest" />
      </ItemGroup>
      ```

2. (COMPLETE) Build the production bundle with the .NET CLI.
    - (COMPLETE) Run the framework-dependent publish command:

      ```bash
      dotnet publish Backend/Backend.DesktopHost/Backend.DesktopHost.csproj -c Release -o ./publish
      ```

    - (COMPLETE) Run the self-contained single-file publish command if no .NET runtime is installed on the target machine:

      ```bash
      dotnet publish Backend/Backend.DesktopHost/Backend.DesktopHost.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
      ```

3. (COMPLETE) Create a publishing script `publish.sh` at the repository root.
    - (COMPLETE) Create a new file named `publish.sh` in the repository root.
    - (COMPLETE) Add parameters and publish logic to publish the project:

      ```bash
      #!/usr/bin/env bash
      set -e

      CONFIGURATION="${1:-Release}"
      OUTPUT_DIR="${2:-./publish}"

      echo "Publishing Backend.DesktopHost..."
      dotnet publish Backend/Backend.DesktopHost/Backend.DesktopHost.csproj \
          -c "$CONFIGURATION" \
          -o "$OUTPUT_DIR"

      echo "Publish complete. Output saved to: $OUTPUT_DIR"
      ```

    - (COMPLETE) Open a bash terminal at the repository root.
    - (COMPLETE) Give execute permissions to `publish.sh` if necessary:

      ```bash
      chmod +x ./publish.sh
      ```

    - (COMPLETE) Run the script to generate the `./publish` folder:

      ```bash
      ./publish.sh
      ```

4. (COMPLETE) Verify the published output.
    - (COMPLETE) Go to the `./publish` directory:

      ```bash
      cd ./publish
      ```

    - (COMPLETE) Start the application executable:

      ```bash
      ./Backend.DesktopHost.exe
      ```

    - (COMPLETE) Open `http://localhost:5000` in a web browser.
    - (COMPLETE) Confirm that the browser shows `index.html`.
    - (COMPLETE) Send a request to `/api/products` to verify that API routes operate correctly.

#### Things to Consider

- A single command like `dotnet publish -c Release -o ./publish` produces an executable that serves the static files directly.
- The `wwwroot` folder must stay in the same directory as the published binary unless embedded into the assembly.

### (COMPLETE) Task 3: Automatically Apply Database Migrations on Startup

The application throws database errors on startup or during first requests when a new SQLite database file is empty or missing required schema tables.

Apply database migrations automatically when `Backend.DesktopHost` starts so that the SQLite database file and schema exist before handling incoming requests.

1. (COMPLETE) Open `Backend/Backend.DesktopHost/Program.cs`.
2. (COMPLETE) Create a service scope after `builder.Build()`.
3. (COMPLETE) Resolve `AppDbContext` from the service provider.
4. (COMPLETE) Execute `Database.Migrate()` on the context instance.

```cs
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
```

5. (COMPLETE) Test running the host locally or from the published bundle to confirm that tables exist in `TEST_DATA.db` without manual database commands.

#### Things to Consider

- `Database.Migrate()` automatically creates the SQLite file if it does not exist yet.
- Verify that schema updates run safely without deleting existing data.

## Next Step(s)

1. Implement `create-product-stock-form` submit handler in `Backend/Backend.DesktopHost/wwwroot/js/app.js` (Task 5).
2. Verify stock addition updates the total quantity in the product table.
3. Update `Documentation/roadmap.md` to mark Phase 1 as complete.

## Affected File(s)

1. `Backend/Backend.DesktopHost/appsettings.json`
2. `Backend/Backend.DesktopHost/appsettings.Development.json`
3. `Backend/Backend.DesktopHost/Backend.DesktopHost.csproj`
4. `Backend/Backend.DesktopHost/Program.cs`
5. `Backend/Backend.DesktopHost/wwwroot/js/app.js`
6. `Documentation/roadmap.md`
7. `publish.sh`


