# EZBM.DesktopClient

This is an ASP.NET Core Razor Pages web application serving as an integrated testing platform and local desktop client prototype for the EZBM system. It integrates directly with `EZBM.Core` and provides a user-friendly UI for developers and operators to test workflows, log attendance, manage inventory, and process point-of-sale checkouts.

## Structure

* The `Helpers/` folder provides core helper tools:
  * `DataSeeder.cs` seeds exactly 10 mock records for all entities if the database is empty on startup.
  * `StateHelper.cs` handles operator authentication sessions.
* The `Pages/` folder holds the Razor Pages views for each module:
  * `Login.cshtml` checks credentials to manage user login sessions.
  * `Index.cshtml` displays the main dashboard with business summary cards.
  * `Inventory.cshtml` handles product catalog edits and stock updates.
  * `POS.cshtml` handles cart management, payment options, and receipts.
  * `Staff.cshtml` tracks shift clocks, commissions, and staff profiles.
  * `Logs.cshtml` displays database audit history and recent events.
  * `Settings.cshtml` lets you adjust business configurations like stock alerts.
  * `Shared/_Layout.cshtml` defines the global sidebar and navigation theme.

## Data Initialization & Seeding

On application startup, the entry point performs the following operations:

1. Automatically runs `DbManager.Initialize()` to ensure the SQLite schema is active.
2. Invokes `DataSeeder.Seed()` to populate the database with exactly 10 distinct mock records for each entity if they are not already present.

### Default Test Credentials

For testing and verification purposes, the database is seeded with a default test operator account:

* **Username:** `teto`
* **Password:** `teto41`

## How to Run

Execute the following command from the workspace root directory:

```bash
dotnet run --project Backend/EZBM.DesktopClient/EZBM.DesktopClient.csproj
```

Once running, open a web browser and navigate to the local host address printed in the terminal (typically `http://localhost:5000` or `https://localhost:5001`).
