# EZBM.Core

This is the core business logic library for the EZBM project. It manages the database models, local SQLite database lifetime, and backend operations (calculations, validations, and data persistence).

---

## Structure

* **`Entities/`**: Database models/entities (e.g. `Item`, `Staff`, `Transaction`, `Attendance`, `Payroll`, `Sale`, `SaleEntry`).
* **`Data/`**: Contains database configuration.
  * `AppDbContext.cs`: Configures the Entity Framework Core connection and database context using SQLite.
  * `DbManager.cs`: Handles database lifecycle (initializing schemas and resetting data).
* **`Services/`**: Core calculations and backend logic (such as inventory, sales, and employee logging logic).
* **`Tools/`**: Utility methods like ID generator, logging, invoice formatters, etc.

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
