# EZBM.DesktopClient

This is an ASP.NET Core Razor Pages web application serving as an integrated testing platform and local desktop client prototype for the EZBM system. It integrates directly with `EZBM.Core` and provides a user-friendly UI for developers and operators to test workflows, log attendance, manage inventory, and process point-of-sale checkouts.

---

## Structure

* **`Helpers/`**:
  * `DataSeeder.cs`: Seeds exactly 10 distinct, representative records for all database entities on startup if the database is newly created or empty.
  * `StateHelper.cs`: Manages operator authentication and active shift tracking via cookie-based state helpers.
* **`Pages/`**:
  * `Login.cshtml` / `Login.cshtml.cs`: Active operator selection and login portal, verifying staff shifts and clock-ins.
  * `Index.cshtml` / `Index.cshtml.cs`: Core dashboard showcasing quick summary metrics, active cashier shifts, and registration statuses.
  * `Inventory.cshtml` / `Inventory.cshtml.cs`: Inventory management module supporting product creation, price tracking, and stock adjustments.
  * `POS.cshtml` / `POS.cshtml.cs`: Point-of-Sale checkout interface allowing barcode/name search, cart management, and receipt/invoice generation.
  * `Staff.cshtml` / `Staff.cshtml.cs`: Attendance and staff management, providing views of active/historical shifts, payroll calculations, and operator logs.
  * `Logs.cshtml` / `Logs.cshtml.cs`: System audits and historical logs for inventory stock actions and sales transactions.
  * `Shared/_Layout.cshtml`: Global application shell featuring direct top-tab navigation and the global attendance clock-in/clock-out controller.

---

## Data Initialization & Seeding

On application startup, the entry point performs the following operations:
1. Automatically runs `DbManager.Initialize()` to ensure the SQLite schema is active.
2. Invokes `DataSeeder.Seed()` to populate the database with exactly 10 distinct mock records for each entity if they are not already present.

### Default Test Credentials

For testing and verification purposes, the database is seeded with a default test operator account:
* **Username:** `teto`
* **Password:** `teto41`

---

## How to Run

Execute the following command from the workspace root directory:

```bash
dotnet run --project Backend/EZBM.DesktopClient/EZBM.DesktopClient.csproj
```

Once running, open a web browser and navigate to the local host address printed in the terminal (typically `http://localhost:5000` or `https://localhost:5001`).
