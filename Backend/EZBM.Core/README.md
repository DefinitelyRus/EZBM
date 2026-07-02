# EZBM.Core

This is the core business logic library for the EZBM project. It manages the database models, local SQLite database lifetime, and backend operations (calculations, validations, and data persistence).

---

## Structure

* **`Entities/`**: Core database models representing domain entities:
  * **User Hierarchy (TPH):** `User.cs`, `Staff.cs`, and `Customer.cs` are mapped using Table-Per-Hierarchy (TPH) in EF Core. Features lazy permissions and card RFID validation expiry.
  * **Sales & Ledger:** `Sale.cs` (parent checkouts), `SaleEntry.cs` (line items), `Transaction.cs` (split/mixed payment records referencing the parent sale), and `ItemTransaction.cs` (stock ledger actions).
  * **Attendance & Operations:** `Attendance.cs` (time-in/out logs), `Payroll.cs` (employee payout calculations), and `ActionLog.cs` (audit interceptor records).
* **`Data/`**: Contains database configuration.
  * `AppDbContext.cs`: Configures SQLite EF Core connection, TPH entity conversion filters, and intercepts save operations to automatically log updates/deletes into the audit log.
  * `DbManager.cs`: Handles database lifecycle (initializing schemas, seeding mock data, and resetting schemas).
* **`Services/`**: Core business services mapping backend rules:
  * `InventoryService.cs`: Product and service management, stock ledger tracking, and restock calculations.
  * `SalesService.cs`: Mixed payments checkout and transaction logic.
  * `StaffService.cs`: Shift attendance logging and modular upgrade commission adjustments.
  * `AuthenticationService.cs`: Plain-text employee login validations.
  * `SettingsService.cs`: JSON configuration reader/writer endpoints adapter.
  * `ICashRegisterService.cs` / `MockCashRegisterService.cs`: Hardware emulation layer logging cash drawer triggers.
* **`Tools/`**: Utility methods like ID generator (`IdHelper.cs`), logging (`Log.cs`), and invoice helpers (`Utils.cs`).

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
