# EZBM (Easy Business Manager) - Prototype

> [!IMPORTANT]
> **This repository has undergone an overhaul.** This document has been updated to reflect the current repository architecture, backend host, and development setup. ([See the roadmap](/Documentation/roadmap.md))

EZBM is a lightweight, local, desktop-first store management app designed for micro-SMEs (sari-sari stores, milk tea shops, etc.) to track sales, inventory, and attendance faster and more reliably than a paper notebook.

This repository contains both the .NET backend solution and the desktop-first front-end interfaces. All parts run locally on the same host machine.

## Tech Stack & Architecture

* **Frontend Interfaces:**
  * A test dashboard served directly from the backend via `wwwroot` using vanilla HTML, CSS, and plain JavaScript.
  * A desktop-first React application (`Frontend-VCC`) utilizing Vite, TypeScript, and Material 3 design tokens and component standards.
* **Backend Architecture:**
  * A local REST Web API host using `ASP.NET Core` (`Backend.DesktopHost`) that serves API endpoints and static assets. Interactive OpenAPI / Swagger documentation is available in development mode at `/swagger`.
  * `Backend.Core`: Class library defining EF Core database entities, models, and core business logic.
  * Data storage is handled locally via `SQLite` with dynamic settings resolution (`Documents/EZBM/settings.json`) and automatic database migrations on startup.
* Communication happens over local HTTP `REST API` endpoints.

## Project Structure

```text
EZBM/
|-- Backend/                     # .NET Solution and Projects
|   |-- Backend.Core/            # Database Models, Entities, and Core Business Logic
|   |-- Backend.DesktopHost/     # Web API Controllers, Static wwwroot Host, and Services
|   \-- Backend.slnx             # .NET Solution file
|-- Frontend-VCC/                # React + Vite + TypeScript Component Library & UI
|-- Documentation/               # Architecture, Code/Git style guidelines, and Roadmap
\-- README.md                    # Root project documentation
```

## Features & Prototypes

The project is built to test and validate several SME workflows and advanced business rules:

* **Product & Inventory Management:** Create, list, edit, and delete products, track batch stock, costs, expiration dates, and real-time total quantities.
* **Stock Transactions:** Create purchase transactions, apply batch tracking, and calculate inventory changes.
* **Dynamic Business Settings:** Persist and resolve custom SQLite database locations outside the binary root via `Documents/EZBM/settings.json`.
* **Interactive Test Dashboard:** Immediate browser-based verification for all backend endpoints hosted directly in `wwwroot`.
* **Hardware & Workflow Prototypes:** Mock physical hardware interactions and checkout workflows as the roadmap progresses.

## Getting Started (Local Setup)

Follow these steps to get the prototype running on your machine.

### Prerequisites

* [.NET SDK 10.0 or Latest](https://dotnet.microsoft.com/download)
* [Node.js (LTS version)](https://nodejs.org/)

### 1. Spin up the Backend

To run the backend REST Web API host and serve the static dashboard:

```bash
dotnet run --project Backend/Backend.DesktopHost/Backend.DesktopHost.csproj
```

Once running:
* Open `http://localhost:5063` (or the port specified in console output) to view the test dashboard.
* Open `http://localhost:5063/swagger` to inspect the Swagger UI documentation.

### 2. Spin up the Frontend

Navigate to the `Frontend-VCC` directory, install npm packages, and start the local Vite development server:

```bash
cd Frontend-VCC
npm install
npm run dev
```

## Data Strategy & Resetting

* The SQLite database file path is resolved dynamically by `BusinessSettingsService`. By default, it is stored inside your user documents folder at:
  ```text
  <user>/Documents/EZBM/business_data/data.db
  ```
* Database migrations apply automatically on application launch.
* To reset the database, stop the backend host, remove `data.db` (or reset the path in `Documents/EZBM/settings.json`), and re-run the backend host.
