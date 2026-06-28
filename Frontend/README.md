# EZBM Frontend Directory

This directory contains the user interface, client-side routing, interactive workflows, and desktop-first prototype for the EZBM application. The frontend is built on **React 19** using **TypeScript**, **Vite**, **Bootstrap 5**, and **React Router DOM**.

---

## Modules & Pages Overview

| Module / Page | Type | Description |
| :--- | :--- | :--- |
| **[App Shell & Routing](src/App.tsx)** | Core Layout | Main application layout incorporating client-side routing, sidebar navigation, and page state management. |
| **[Analytics Dashboard](src/pages/Dashboard.jsx)** | Page View | Home screen displaying daily business metrics (sales/profit summaries), shift attendance controls, and low-stock alerts. |
| **[Inventory Management](src/pages/Inventory.jsx)** | Page View | Product and service catalog interface with real-time search filtering, stock tracking, and item add/edit forms. |
| **[POS Checkout](src/pages/Checkout.jsx)** | Page View | Point-of-Sale checkout workspace featuring a searchable product grid, interactive shopping cart, and transaction finalization. |
| **[Staff Management](src/pages/Staff.jsx)** | Page View | Directory interface for managing employee profiles, positions, and compensation configuration. |
| **[Attendance & Payroll Logs](src/pages/Attendance_Logs.jsx)** | Page View | Historical audit ledger presenting employee shift time logs and past payroll distributions. |

---

### 1. App Shell & Reusable Components

The foundational layout and UI library for the application.

* **Layout & Routing**: Managed in `src/App.tsx` and `src/main.tsx`. Routes map paths (`/dashboard`, `/inventory`, `/pos`, `/staff`, `/logs`) to their respective page views within a responsive Bootstrap grid frame.
* **Navigation**: Provided by `src/components/sidebar.jsx`, enabling quick navigation between core business modules.
* **UI Components**: Reusable elements located in `src/components/` (e.g., `button.tsx`, `Dropdown.jsx`, `InventoryAlert.jsx`) providing modular UI controls and restock warning banners.

#### How to Reference

Import components directly into any page view or layout container:

```tsx
import Sidebar from './components/Sidebar';
import InventoryAlert from './components/InventoryAlert';
```

---

### 2. Page Views & Business Workflows

The primary client-side screens providing interactive interfaces for store operations.

* **Dashboard**: Defined in `src/pages/Dashboard.jsx`. Displays high-level daily performance cards (Total Sales, Total Profit), low-stock alert lists for items with quantity below threshold, and shift attendance clock-in/out toggles.
* **Inventory**: Defined in `src/pages/Inventory.jsx`. Provides real-time filtering across product catalogs, item property customization forms, and restock tracking.
* **POS Checkout**: Defined in `src/pages/Checkout.jsx`. Connects available inventory items to an interactive shopping cart panel with instant subtotal calculation and payment method selection.
* **Staff & Audit Logs**: Defined in `src/pages/Staff.jsx` and `src/pages/Attendance_Logs.jsx`. Handles employee profiles, work shift logs, and historical payroll payment ledger tracking.

#### How to Run

To install dependencies and start the local Vite development server:

```bash
npm install
npm run dev
```

---

### 3. Build & Code Quality

Configuration tools for compiling production assets and enforcing code consistency.

* **Vite Configuration**: Managed via `vite.config.ts` utilizing `@vitejs/plugin-react` and `vite-plugin-svgr`.
* **TypeScript Setup**: Configured across `tsconfig.json`, `tsconfig.app.json`, and `tsconfig.node.json` for type checking.
* **Linting Rules**: Enforced via ESLint configuration in `eslint.config.js`.

#### How to Build & Test

To validate code quality or build production distribution files:

```bash
# Run ESLint quality checks
npm run lint

# Compile TypeScript and build production bundle
npm run build

# Preview the built production bundle locally
npm run preview
```
