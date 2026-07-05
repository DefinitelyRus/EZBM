# EZBM.Core

This is the core business logic library for the EZBM project. It manages the database models, local SQLite database lifetime, and backend operations (calculations, validations, and data persistence).

---

## Structure

* The `Entities/` folder holds the database models for our project:
  * `User.cs`, `Staff.cs`, and `Customer.cs` handle profiles using single-table inheritance (`TPH`), with automated card expiry checks.
  * `Sale.cs`, `SaleEntry.cs`, `Transaction.cs`, and `ItemTransaction.cs` track cart checkouts, payments, and stock changes.
  * `Attendance.cs`, `Payroll.cs`, and `ActionLog.cs` manage hours worked, payouts, and database audit logs.
* The `Data/` folder configures the database connection and behavior:
  * `AppDbContext.cs` links our models to the SQLite database and logs modifications automatically.
  * `DbManager.cs` takes care of setting up, seeding sample data, and resetting the database.
* The `Services/` folder contains files that map core business logic:
  * `InventoryService.cs` manages item stocks and reorders.
  * `SalesService.cs` handles transactions and checkout splits.
  * `StaffService.cs` processes work logs and commission payouts.
  * `AuthenticationService.cs` checks user login credentials.
  * `SettingsService.cs` reads and writes app configurations.
  * `MockCashRegisterService.cs` emulates hardware connections like opening a cash drawer.
* The `Tools/` folder contains utilities like ID generators, logging functions, and invoice helpers.

---

## Local Database Save Location

The SQLite database `business_data.db` is configured to be saved inside the user's `My Documents` folder (or OneDrive-backed Documents folder):

* `C:\Users\Rus\Documents\business_data.db`

---

## How to Reference

To use the core library in other .NET projects (e.g. Host, Tests, CLI tools):

```bash
dotnet add <project-path>.csproj reference Backend/EZBM.Core/EZBM.Core.csproj
```
