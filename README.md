# EZBM (Easy Business Manager) - Prototype

> *Author(s): DefinitelyRus, Google Gemini*

EZBM is a lightweight, local, desktop-first store management app designed for micro-SMEs (sari-sari stores, milk tea shops, etc.) to track sales, inventory, and attendance faster and more reliably than a paper notebook.

This repository contains both the .NET back-end API and the React front-end. Both parts run locally on the same host machine.

---

## 🛠️ Tech Stack & Architecture

* **Front-end:** React (Desktop-first UI)
* **Back-end:** .NET Web API (SOLID principles, component-based structure)
* **Database:** SQLite (`ezbm.db` for relational user & operational data)
* **Configuration:** JSON (`settings.json` for app state/preferences)
* **Communication:** Local REST API (`localhost`)

---

## 📦 Project Structure

```text
ezbm/
├── backend/   # .NET Web API Project
│   ├── Controllers/ # REST Endpoints
│   ├── Services/  # Business Logic (SOLID)
│   ├── Repositories/ # SQLite Data Access
│   └── ezbm.db   # Local SQLite File (Auto-generated)
├── frontend/   # React Application
│   ├── src/
│   │   ├── components/ # Reusable UI Elements
│   │   ├── views/  # Dashboard, POS, Inventory, Login
│   │   └── api/  # Axios/Fetch Local Client
└── settings.json  # Shared local configuration file
```

---

<!-->
## 🚀 Getting Started (Local Setup)

Follow these steps to get the prototype running on your machine.

### Prerequisites

* [.NET SDK (Latest Stable)](https://dotnet.microsoft.com/download)
* [Node.js (LTS version)](https://nodejs.org/)

### 1. Spin up the Back-end

Navigate to the backend directory, restore dependencies, and launch the server:

```bash
cd backend
dotnet restore
dotnet run
```

The API should now be listening locally (e.g., `http://localhost:5000` or `https://localhost:5001`). Check your terminal output for the exact port.

### 2. Spin up the Front-end

Open a new terminal window, navigate to the frontend directory, install npm packages, and start the development server:

```bash
cd frontend
npm install
npm start
```

Your default browser will automatically open to `http://localhost:3000`.

---
<-->

## 💾 Data Strategy & Resetting

* **Database (`ezbm.db`):** The prototype uses a completely local SQLite file.
* **The "Wipe" Rule:** Because we are avoiding complex database migrations during early prototyping, if you change the database schema, simply delete the `ezbm.db` file and rerun the back-end to let it regenerate a fresh schema.
* **Authentication:** User passwords are stored in **plain text** for this prototype phase. Do not use real-world passwords.
