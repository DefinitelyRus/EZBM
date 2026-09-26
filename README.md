# EZBM (Easy Business Manager) - Prototype

> [!IMPORTANT]
> **This repository is undergoing an overhaul!** Information from this README is unmodified
> from v1.0, so assume most details provided are no longer accurate. It will be updated
> at the end of each phase. ([See the roadmap](/Documentation/roadmap.md))

EZBM is a lightweight, local, desktop-first store management app designed for micro-SMEs (sari-sari stores, milk tea shops, etc.) to track sales, inventory, and attendance faster and more reliably than a paper notebook.

This repository contains both the .NET back-end solutions/services and the React front-end. All parts run locally on the same host machine.

---

## Tech Stack & Architecture

* We use `React` for the front-end user interface, designed specifically for desktop screens.
* The back-end offers two options:
  * A local Web API using `ASP.NET Core` that serves endpoints for the front-end (see the [API Documentation](Backend/EZBM.DesktopHost/API%20Documentation.md) for details on all available endpoints and how to consume them).
  * A `Razor Pages` desktop client that serves as a local testing platform.
* Core business logic and database access are handled by a .NET library using `EF Core` and a local `SQLite` database.
* Communication happens over a local `REST API` or direct library integration.

---

## Project Structure

```text
EZBM/
|-- Backend/                     # .NET Solution and Projects
|   |-- EZBM.Core/               # Database Models, Services, and EF Core Context
|   |-- EZBM.DesktopHost/        # Web API Controllers (REST Endpoints)
|   |-- EZBM.DesktopClient/      # Razor Pages Local Desktop Client & Testing Platform
|   |-- EZBM.Tests/              # Integration Tests Console App
|   \-- EZBM.slnx                # .NET Solution file
|-- Frontend/                    # React Application (Desktop-first UI)
\-- README.md                    # Root project documentation
```

---

## Features & Prototypes

The project is built to test and validate several SME workflows and advanced business rules:

* The Point-of-Sale (`POS`) screen lets you manage your cart and checkout.
* Customers can split payments between multiple methods (like cash and mobile wallet) on a single order.
* Staff and customer profiles are stored in a single table, with checks for card expiration dates.
* Every time database entries are updated or deleted, the action is automatically saved in an audit log.
* The payroll logic automatically deducts commissions that were already paid out when calculating new earnings.
* We mock physical hardware interactions, such as simulating `RFID` scans and triggering cash drawer alerts.

---

## Getting Started (Local Setup)

Follow these steps to get the prototype running on your machine.

### Prerequisites

* [.NET SDK (Latest Stable)](https://dotnet.microsoft.com/download)
* [Node.js (LTS version)](https://nodejs.org/)

### 1. Spin up the Back-end

To run the back-end REST Web API host (see the [API Documentation](Backend/EZBM.DesktopHost/API%20Documentation.md) for details on all endpoints and how to consume them):

```bash
dotnet run --project Backend/EZBM.DesktopHost/EZBM.DesktopHost.csproj
```

Alternatively, to run the Razor Pages Desktop Client:

```bash
dotnet run --project Backend/EZBM.DesktopClient/EZBM.DesktopClient.csproj
```

### 2. Spin up the Front-end

Navigate to the Frontend directory, install npm packages, and start the local Vite development server:

```bash
cd Frontend
npm install
npm run dev
```

---

## Data Strategy & Resetting

* The database is stored locally as a `SQLite` file inside your `Documents` folder.
* If you make changes to the database structure, just delete the `business_data.db` file and restart the backend to recreate it.
* Passwords are saved in plain text for this prototype. Please do not use real passwords.
