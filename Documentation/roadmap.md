# EZBM Project Roadmap

This document outlines the step-by-step plan for building the EZBM inventory and store management system. The project starts with a simple MVP and grows into a complete application.

> [!NOTE]
> Progress assumes 4 to 6 active development days per week (max 1-3 off-days). For all phases prior to Phase 6, every new backend endpoint must have a corresponding front-end interface built in basic HTML, CSS, and plain JavaScript in `wwwroot`.

## Phase 1: Core Inventory MVP (Backend + Vanilla UI)

The main goal of Phase 1 is to get a working inventory tracker running with a single bundled command.

* **Estimated Duration**: 1 active day (Remaining)
* **Estimated Target**: September 28 – September 29, 2026

### (COMPLETE) Backend Setup

* (COMPLETE) Set up a C# and .NET Core project using ASP.NET.
* (COMPLETE) Connect a local SQLite database to store your data.
* (COMPLETE) Create database models and tables for products and product handling.

### (COMPLETE) API Development

* (COMPLETE) Build REST API endpoints to fetch, add, update, and delete products.
* (COMPLETE) Add endpoints to increase or decrease stock counts.

### (COMPLETE) Frontend Setup

* (COMPLETE) Create a simple user interface using plain HTML, CSS, and JavaScript.
* (COMPLETE) Place these files inside the ASP.NET `wwwroot` folder so the server can serve them directly.
* (COMPLETE) Use JavaScript `fetch` calls to talk to the backend API.

### Bundled Deployment

* Configure the app so running a single command starts both the backend server and serves the frontend interface.
* *Estimated Duration*: 1 active day (Sep 28 – Sep 29, 2026)

## Phase 2: POS and Transactions

Once the core inventory works, Phase 2 adds sales processing and stock tracking history. Each new backend endpoint must include a basic HTML UI in `wwwroot`.

* **Estimated Duration**: 9–11 active days (~2 to 2.5 calendar weeks)
* **Estimated Target**: September 30 – October 15, 2026

### Inventory Transactions

* Track every stock movement (such as new deliveries, sales, waste, or manual counts) in an audit table.
* Store timestamps and reference notes for each transaction.
* Build basic HTML controls and transaction history table views in `wwwroot`.
* *Estimated Duration*: 3 active days (Sep 30 – Oct 3, 2026)

### Payment Processing

* Record how customers pay (cash, credit card, or digital wallets) for each sale.
* Build basic HTML payment form inputs and split/mixed payment controls in `wwwroot`.
* *Estimated Duration*: 2–3 active days (Oct 5 – Oct 8, 2026)

### Point of Sale (POS)

* Build a checkout screen in plain HTML/JS to scan or select items.
* Calculate totals, taxes, and itemized sales automatically in the cart.
* Reduce stock levels in real-time when a sale finishes.
* *Estimated Duration*: 4–5 active days (Oct 9 – Oct 15, 2026)

## Phase 3: Security & Single-User Authentication

Phase 3 secures your app with essential cryptographic safeguards. To speed up backend completion, multi-user accounts and the permission matrix are deferred; the app will operate in single-user mode. Each security endpoint must include a basic HTML UI in `wwwroot`.

* **Estimated Duration**: 2–3 active days (~0.5 calendar weeks)
* **Estimated Target**: October 16 – October 20, 2026

### Core Security & Authentication

* Build a single-user authentication system using secure password hashing (PBKDF2/BCrypt).
* Implement authentication token generation, validation, and session verification.
* Add a simple HTML login page and credential management interface in `wwwroot`.
* *Estimated Duration*: 2–3 active days (Oct 16 – Oct 20, 2026)

### Access Permissions (DEFERRED)

* Multi-role permission system (Cashier, Inventory Clerk, Admin) is deferred until multi-user support is prioritized.

## Phase 4: Attendance and Payroll (ON HIATUS)

Phase 4 helps you manage your workforce inside the same system.

* **Status**: On hiatus (relies on Phase 3 staff management system).
* **Estimated Duration**: Deferred (0 active days currently allocated).

### Staff Attendance (DEFERRED)

* Add a simple clock-in and clock-out feature for shifts.
* Store attendance records and daily work logs.

### Staff Payroll (DEFERRED)

* Calculate employee pay based on hours worked, base rates, and deductions.

## Phase 5: Event Logging

Phase 5 adds system auditing to track important actions and security events. Every logging endpoint must include a basic HTML UI in `wwwroot`.

* **Estimated Duration**: 3–4 active days (~1 calendar week)
* **Estimated Target**: October 21 – October 26, 2026

### Event Logging

* Automatically record important system events (such as failed logins, admin actions, and large stock adjustments).
* Save logs to the SQLite database so admins can review them later.
* Create a basic HTML audit log viewer in `wwwroot` to filter and inspect logged events.
* *Estimated Duration*: 3–4 active days (Oct 21 – Oct 26, 2026)

## Phase 6: Modern UI Redesign

Phase 6 replaces the simple HTML interface with a modern, fast frontend.

* **Estimated Duration**: 13–17 active days (~3 to 3.5 calendar weeks)
* **Estimated Target**: October 27 – November 20, 2026

### Frontend Setup

* Initialize a new frontend project using Vite, React, TypeScript, and Tailwind CSS.
* Configure Tailwind pre-flight styles and add manual custom CSS where needed.
* *Estimated Duration*: 2–3 active days (Oct 27 – Oct 30, 2026)

### Component Architecture

* Build clean, reusable components for the POS terminal, inventory tables, and management dashboards.
* *Estimated Duration*: 8–10 active days (Nov 2 – Nov 13, 2026)

### Final Integration

* Connect the React app to your existing ASP.NET Core backend API.
* Update the build pipeline to bundle the compiled React files into the final .NET executable package.
* *Estimated Duration*: 3–4 active days (Nov 16 – Nov 20, 2026)
