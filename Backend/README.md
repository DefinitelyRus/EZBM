# EZBM Backend Directory

This directory contains the backend services, business logic, test suites, and diagnostic tools for the EZBM application. The backend is built on **.NET** using **C#** and **EF Core / SQLite** for local data persistence.

---

## Projects Overview

| Project | Type | Description |
| :--- | :--- | :--- |
| [`EZBM.Core`](EZBM.Core/README.md) | Class Library | The core engine containing data entities, database context, services, and core utilities. |
| [`EZBM.DesktopHost`](EZBM.DesktopHost/README.md) | Web API | The local REST API host mapping HTTP endpoints to controller methods, allowing the frontend to communicate with `EZBM.Core`. |
| [`EZBM.DesktopClient`](EZBM.DesktopClient/README.md) | Web App (Razor Pages) | The integrated testing platform and local desktop client prototype providing an interactive user interface to run and verify business workflows. |
| [`EZBM.Tests`](EZBM.Tests/README.md) | Console App | Integration test suite that resets the database and tests API endpoints directly, outputting reports to `Results.md`. |

---

### `EZBM.Core`

The foundational business logic layer.

* Core entities like `Staff`, `Item`, and `Sale` are defined in the `Entities/` folder.
* The `SQLite` database is managed by the DB context in the `Data/` folder. The database file is saved in your `Documents` folder.
* Core business rules and logic are handled by services in the `Services/` folder.

#### How to Reference

Add a project reference to this project in any runner project:

```bash
dotnet add <project-path>.csproj reference Backend/EZBM.Core/EZBM.Core.csproj
```

---

### `EZBM.DesktopHost`

An ASP.NET Core Web API project hosting endpoints locally for front-end integration.

* Controllers in the `Endpoints/` folder map HTTP requests to backend actions.
* The `Program.cs` entry point sets up the database schema when the app starts.

#### How to Run

To spin up the local REST server:

```bash

dotnet run --project Backend/EZBM.DesktopHost/EZBM.DesktopHost.csproj
```

---

### `EZBM.DesktopClient`

An ASP.NET Core Razor Pages application serving as the integrated testing platform and desktop prototype interface.

* Includes features like shift logs, profile updates, product management, checkout, payroll, settings, and audit trails.
* The app seeds the database with 10 sample entries for each table on startup.

#### How to Run

To run the local desktop client and serve the pages locally:

```bash
dotnet run --project Backend/EZBM.DesktopClient/EZBM.DesktopClient.csproj
```

---

### `EZBM.Tests`

A console project serving as our integration test suite.

* Resets the database, tests API endpoints with mock data, checks business rules (like split payments and audit logging), and writes a test summary.
* Generates a detailed test summary report.

#### How to Run Tests

To run endpoint tests and review output:

```bash
# From the repository root
dotnet run --project Backend/EZBM.Tests/EZBM.Tests.csproj
```
