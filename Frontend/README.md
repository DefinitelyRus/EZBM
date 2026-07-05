# EZBM Frontend Directory

This directory contains the user interface, client-side routing, interactive workflows, and desktop-first prototype for the EZBM application. The frontend is built on **React 19** using **TypeScript**, **Vite**, **Bootstrap 5**, and **React Router DOM**.

---

## Modules & Pages Overview

| Module / Page | Type | Description |
| :--- | :--- | :--- |
| [App Shell & Routing](src/App.tsx) | Core Layout | Main application layout incorporating client-side routing, sidebar navigation, and page state management. |
| [Analytics Dashboard](src/pages/Dashboard.jsx) | Page View | Home screen displaying daily business metrics (sales/profit summaries), shift attendance controls, and low-stock alerts. |
| [Inventory Management](src/pages/Inventory.jsx) | Page View | Product and service catalog interface with real-time search filtering, stock tracking, and item add/edit forms. |
| [POS Checkout](src/pages/Checkout.jsx) | Page View | Point-of-Sale checkout workspace featuring a searchable product grid, interactive shopping cart, and transaction finalization. |
| [Staff Management](src/pages/Staff.jsx) | Page View | Directory interface for managing employee profiles, positions, and compensation configuration. |
| [Attendance & Payroll Logs](src/pages/Attendance_Logs.jsx) | Page View | Historical audit ledger presenting employee shift time logs and past payroll distributions. |

---

### App Shell & Reusable Components

The foundational layout and UI library for the application.

* Routes in `src/App.tsx` and `src/main.tsx` map paths (like `/dashboard` or `/pos`) to their page views.
* Sidebar navigation from `src/components/sidebar.jsx` helps users jump between sections quickly.
* Reusable UI components like buttons, dropdowns, and alert banners are stored in the `src/components/` folder.

#### How to Reference

Import components directly into any page view or layout container:

```tsx
import Sidebar from './components/Sidebar';
import InventoryAlert from './components/InventoryAlert';
```

---

### Page Views & Business Workflows

The primary client-side screens providing interactive interfaces for store operations.

* The Dashboard displays daily sales cards, low-stock warnings, cashier clocks, and recent transactions.
* The Inventory page lets users filter items in real time, configure product values, and track stocks.
* The POS Checkout links items to a cart, calculates totals, selects payments, and handles split payment options.
* The Staff & Audit logs track employee profiles, shift entries, system audits, and payroll/commissions.

#### How to Run

To install dependencies and start the local Vite development server:

```bash
npm install
npm run dev
```

---

### Build & Code Quality

Configuration tools for compiling production assets and enforcing code consistency.

* Vite build configurations are set up in `vite.config.ts`.
* TypeScript types are configured in the `tsconfig.json` files.
* Formatting and code quality checks are managed by ESLint in `eslint.config.js`.

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
