# EZBM (Easy Business Manager) - Prototype

> *Author(s): DefinitelyRus, Google Gemini*

EZBM is a lightweight, local, desktop-first store management app designed for micro-SMEs (sari-sari stores, milk tea shops, etc.) to track sales, inventory, and attendance faster and more reliably than a paper notebook.

This repository contains both the .NET back-end solutions/services and the React front-end. All parts run locally on the same host machine.

---

## 🛠️ Tech Stack & Architecture

* **Front-end UI:** React (Desktop-first UI)
* **Back-end Options:**
  * **Desktop Host (REST API):** ASP.NET Core Web API serving endpoints for front-end integration.
  * **Desktop Client (Razor Pages):** Integrated Razor Pages web app serving as a local desktop client and testing platform.
* **Core Logic & DB:** .NET Class Library using EF Core & SQLite (`business_data.db` stored locally).
* **Communication:** Local REST API (`localhost`) or Direct Library Integration (for Razor Pages client).

---

## 📦 Project Structure

```text
EZBM/
├── Backend/                     # .NET Solution and Projects
│   ├── EZBM.Core/               # Database Models, Services, and EF Core Context
│   ├── EZBM.DesktopHost/        # Web API Controllers (REST Endpoints)
│   ├── EZBM.DesktopClient/      # Razor Pages Local Desktop Client & Testing Platform
│   ├── EZBM.Tests/              # Integration Tests Console App
│   └── EZBM.slnx                # .NET Solution file
├── Frontend/                    # React Application (Desktop-first UI)
└── README.md                    # Root project documentation
```

---

## 🚀 Features & Prototypes

The project is built to test and validate several SME workflows and advanced business rules:

* **Point-of-Sale (POS) System:** Support for real-time checkout, cart management, and receipt configuration validation.
* **Mixed & Split Payments:** Decoupled transactions logic allowing customers to pay using split channels (e.g., cash, mobile wallet, card) on a single check-out order.
* **Unified User Profile System:** Single table-per-hierarchy (TPH) SQLite mapping representing both `Staff` and `Customer` details, equipped with automatic card/permissions expiry lazy checking.
* **Automatic Audit Logs:** Interceptor tracking inside EF Core DbContext to capture all update/delete actions into an `ActionLogs` audit ledger.
* **Payroll & Commissions Upgrade Engine:** Deducting already-paid lower-tier commissions from newly earned higher-tier payroll distributions.
* **Hardware Integration Mocking:** HID reader keyboard emulation for RFID/barcode scans and automated console logs for cash drawer triggers.

---

## 🚀 Getting Started (Local Setup)

Follow these steps to get the prototype running on your machine.

### Prerequisites

* [.NET SDK (Latest Stable)](https://dotnet.microsoft.com/download)
* [Node.js (LTS version)](https://nodejs.org/)

### 1. Spin up the Back-end

To run the back-end REST Web API host:

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

## 💾 Data Strategy & Resetting

* **Database (`business_data.db`):** The prototype uses a completely local SQLite file saved inside the user's `My Documents` folder (or OneDrive-backed equivalent).
* **The "Wipe" Rule:** Because we are avoiding complex database migrations during early prototyping, if you change the database schema, simply delete the `business_data.db` file and rerun the back-end application to let it regenerate a fresh schema.
* **Authentication:** User passwords are stored in **plain text** for this prototype phase. Do not use real-world passwords.
