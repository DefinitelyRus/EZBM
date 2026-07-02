# EZBM Backend Directory

This directory contains the backend services, business logic, test suites, and diagnostic tools for the EZBM application. The backend is built on **.NET** using **C#** and **EF Core / SQLite** for local data persistence.

---

## Projects Overview

| Project | Type | Description |
| :--- | :--- | :--- |
| **[EZBM.Core](EZBM.Core/README.md)** | Class Library | The core engine containing data entities, database context, services, and core utilities. |
| **[EZBM.DesktopHost](EZBM.DesktopHost/README.md)** | Web API | The local REST API host mapping HTTP endpoints to controller methods, allowing the frontend to communicate with `EZBM.Core`. |
| **[EZBM.DesktopClient](EZBM.DesktopClient/README.md)** | Web App (Razor Pages) | The integrated testing platform and local desktop client prototype providing an interactive user interface to run and verify business workflows. |
| **[EZBM.Tests](EZBM.Tests/README.md)** | Console App | Integration test suite that resets the database and tests API endpoints directly, outputting reports to `Results.md`. |

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

* **Endpoints**: Defined in `Endpoints/` (e.g., `InventoryController`, `SalesController`, `StaffController`, `AuthController`, `SettingsController`, `LogsController`).
* **Initialization**: The main entry point `Program.cs` automatically triggers `DbManager.Initialize()` to ensure the database schema exists on startup.

#### How to Run

To spin up the local REST server:

```bash
dotnet run --project Backend/EZBM.DesktopHost/EZBM.DesktopHost.csproj
```

---

### 3. EZBM.DesktopClient

An ASP.NET Core Razor Pages application serving as the integrated testing platform and desktop prototype interface.

* **Features**: Dynamic attendance clock-in/out, operator registration/login state, product inventory management, Point-of-Sale checkouts, payroll verification, dynamic settings configurations, and historical logs/audit tracking.
* **Initialization**: The application automatically runs `DbManager.Initialize()` and seeds the database with exactly 10 distinct, representative entries for each entity.

#### How to Run

To run the local desktop client and serve the pages locally:

```bash
dotnet run --project Backend/EZBM.DesktopClient/EZBM.DesktopClient.csproj
```

---

### 4. EZBM.Tests

A console project serving as our integration test suite.

* **Flow**: Resets the database to a clean state, invokes static API endpoint methods directly, verifies core behavior (including new Phase 2-7 features: Split Payments, Promo Codes, Lazy Expiry validation, and automatic DB audit logs), and compiles the results in a markdown summary.
* **Test Outputs**: Compiles a detailed test summary to `Backend/Results.md`.

#### How to Run Tests

To run endpoint tests and review output:

```bash
# From the repository root
dotnet run --project Backend/EZBM.Tests/EZBM.Tests.csproj
```
