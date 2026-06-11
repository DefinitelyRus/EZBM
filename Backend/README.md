# EZBM Backend Directory

This directory contains the backend services, business logic, test suites, and diagnostic tools for the EZBM application. The backend is built on **.NET** using **C#** and **EF Core / SQLite** for local data persistence.

---

## Projects Overview

| Project | Type | Description |
| :--- | :--- | :--- |
| **[EZBM.Core](EZBM.Core/README.md)** | Class Library | The core engine containing data entities, database context, services, and core utilities. |
| **[EZBM.DesktopHost](EZBM.DesktopHost/README.md)** | Web API | The local REST API host mapping HTTP endpoints to controller methods, allowing the frontend to communicate with `EZBM.Core`. |
| **[EZBM.Tests](EZBM.Tests/README.md)** | Console App | Integration test suite that resets the database and tests API endpoints directly, outputting reports to `Results.md`. |
| **[TestDb](TestDb/README.md)** | Console App | A minor utility/diagnostic project used to test database creation, configuration, and connectivity independently. |

---

### 1. EZBM.Core

The foundational business logic layer.

* **Entities**: Defined in `Entities/` (e.g., `Staff`, `Item`, `Transaction`, `Attendance`, `Payroll`, `Sale`, `SaleEntry`).
* **Database Management**: Managed by `Data/DbManager` and `Data/AppDbContext` targeting SQLite. The database file `business_data.db` is stored under the user's `My Documents` folder (or OneDrive-backed equivalent).
* **Services**: Defined in `Services/` (e.g., `InventoryService`) containing core logic rules.

#### How to Reference

Add a project reference to this project in any runner project:

```bash
dotnet add <project-path>.csproj reference Backend/EZBM.Core/EZBM.Core.csproj
```

---

### 2. EZBM.DesktopHost

An ASP.NET Core Web API project hosting endpoints locally for front-end integration.

* **Endpoints**: Defined in `Endpoints/` (e.g., `InventoryController`, `SalesController`, `StaffController`, `AuthController`).
* **Initialization**: The main entry point `Program.cs` automatically triggers `DbManager.Initialize()` to ensure the database schema exists on startup.

#### How to Run

To spin up the local REST server:

```bash
dotnet run --project Backend/EZBM.DesktopHost/EZBM.DesktopHost.csproj
```

---

### 3. EZBM.Tests

A console project serving as our integration test suite.

* **Flow**: Resets the database to a clean state, invokes static API endpoint methods directly, and compiles the results in a markdown summary.
* **Test Outputs**: Compiles a detailed test summary to `Backend/Results.md`.

#### How to Run Tests

To run endpoint tests and review output:

```bash
cd Backend/EZBM.Tests
rm results.log # Remove old logs if they exist
dotnet run --project EZBM.Tests.csproj
```

---

### 4. TestDb

A minimal console application specifically created to isolate and test database lifecycle behavior.

* **Features**: Runs database initialization (`DbManager.Initialize()`) and checks whether the database is successfully created, listing the resolved save directory path.

#### How to Run Diagnostic

```bash
dotnet run --project Backend/TestDb/TestDb.csproj
```
