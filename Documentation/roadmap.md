# EZBM Project Roadmap

This document outlines the step-by-step plan for building the EZBM inventory and store management system. The project starts with a simple MVP and grows into a complete application.

## Phase 1: Core Inventory MVP (Backend + Vanilla UI)

The main goal of Phase 1 is to get a working inventory tracker running with a single bundled command.

### Backend Setup

* Set up a C# and .NET Core project using ASP.NET.
* Connect a local SQLite database to store your data.
* Create database models and tables for `Products`, `Categories`, and `Stock Levels`.

### API Development

* Build REST API endpoints to fetch, add, update, and delete products.
* Add endpoints to increase or decrease stock counts.

### Frontend Setup

* Create a simple user interface using plain HTML, CSS, and JavaScript.
* Place these files inside the ASP.NET `wwwroot` folder so the server can serve them directly.
* Use JavaScript `fetch` calls to talk to the backend API.

### Bundled Deployment

* Configure the app so running a single command starts both the backend server and serves the frontend interface.

## Phase 2: POS and Transactions

Once the core inventory works, Phase 2 adds sales processing and stock tracking history.

### Point of Sale (POS)

* Build a checkout screen to scan or select items.
* Calculate totals, taxes, and itemized sales automatically.
* Reduce stock levels in real-time when a sale finishes.

### Inventory Transactions

* Track every stock movement (such as new deliveries, sales, waste, or manual counts) in an audit table.
* Store timestamps and reference notes for each transaction.

### Payment Processing

* Record how customers pay (cash, credit card, or digital wallets) for each sale.

## Phase 3: Staff and Permissions

Phase 3 secures your app so different users have different access rights.

### Staff and Login

* Build a login system using secure password hashing.
* Create a database table for store employees and staff accounts.

### Access Permissions

* Set up role-based access control (such as Cashier, Inventory Clerk, and Admin).
* Restrict sensitive actions (like editing prices or viewing reports) to managers and admins.

## Phase 4: Attendance and Payroll

Phase 4 helps you manage your workforce inside the same system.

### Staff Attendance

* Add a simple clock-in and clock-out feature for shifts.
* Store attendance records and daily work logs.

### Staff Payroll

* Calculate employee pay based on hours worked, base rates, and deductions.

## Phase 5: Event Logging

Phase 5 adds system auditing to track important actions and security events.

### Event Logging

* Automatically record important system events (such as failed logins, admin actions, and large stock adjustments).
* Save logs to the SQLite database so admins can review them later.

## Phase 6: Modern UI Redesign

Phase 6 replaces the simple HTML interface with a modern, fast frontend.

### Frontend Setup

* Initialize a new frontend project using Vite, React, TypeScript, and Tailwind CSS.
* Configure Tailwind pre-flight styles and add manual custom CSS where needed.

### Component Architecture

* Build clean, reusable components for the POS terminal, inventory tables, and management dashboards.

### Final Integration

* Connect the React app to your existing ASP.NET Core backend API.
* Update the build pipeline to bundle the compiled React files into the final .NET executable package.
