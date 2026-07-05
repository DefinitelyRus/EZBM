# EZBM Frontend Developer Task List

Since the C# backend is already (mostly) fully built and verified, this is here to walk you through the frontend layout. Your focus is strictly on building the user interface in React.

I laid out exactly what screens need creating. Because store staff will be running this application all day long on a local PC, the goal is a fast, clean, desktop-first layout that keeps operations simple.

Keep in mind that it's supposed to be simple as the desktop form will *not* be its final version--it's going to be a mobile app. This is just the test platform testers will be using.

Lastly, make sure to update your Notion task list to reflect what you're *actually* doing; this task list is probably not 100% accurate or comprehensive.

---

## Step Initial App Structure

### App Shell Framework

* A main dashboard layout that features a navigation section (like a sidebar or top menu) and a main workspace area. It also needs a header that displays who is currently logged in.
* Cashiers and store owners need to jump between their daily tasks quickly (like pausing a sale to check stock) without losing their active screen.

### Tasks

* [x] Bootstrap the React codebase.
* [x] Structure the main layout showing a navigation panel, user header, and active workspace panel.

---

## Page Login Screen

### Credential Entry Form

* A clean login box with fields for Username and Password, a submit button, and a space to display validation errors.
* We need to know exactly who is operating the register so that shift times and sales records are attributed to the right employee.

### Tasks

* [ ] Build a login window with username and password input elements.
* [ ] Build validation warnings that block empty submissions and highlight incomplete fields.
* [ ] Build an error banner display for cases when the login details do not match.
* [ ] Save the active employee details in the application state once verified.

### Note

This won't be used for the time being, but it'll be good to have it built anyway for testing purposes. More details on this later.s

---

## Page Analytics Dashboard (Home Screen)

### Performance Summaries & Restock Warnings

* A simple home dashboard displaying daily summary metrics (Total Sales and Total Profit) and a low-stock alert area.
* Store owners need a quick way to check if they had a good business day and see which products are running out so they can restock them.

### Shift Attendance Toggle

* A prominent toggle button in the header or sidebar that allows cashiers to clock in or clock out.
* Employees need to log their shift times without opening a separate, complicated attendance app.

### Tasks

* [x] Build summary cards for today's total sales and today's total profit.
* [x] Build a Low Stock Alerts list that flags items with quantity below 5 units, with an alternative state when all items are fully stocked.
* [ ] Build the shift attendance toggle button, using distinct colors or labels to show if the user is clocked in or clocked out.
* [x] Build a Recent Transactions list feed panel.

---

## Page Inventory Management

### Product & Service Catalog View

* A list or table showing all items the store sells (including both physical products and digital services). It needs a search bar at the top so users can filter items in real time.
* Users need to quickly see what items are in stock, check their retail prices, and find a product instantly.

### Add & Edit Item Workspace

* Popups or form sheets where users can enter details to create or update an item (its name, cost, retail price, stock quantity, measurement unit, and tags).
* Restocking items or adding new products shouldn't require technical database knowledge. The merchant just needs a simple form.

### Tasks

* [x] Build an inventory table showing details (Name, Type, Cost, Retail, Quantity, Unit, Tags).
* [x] Build a real-time search input that filters the catalog table.
* [x] Build the "Add/Edit Item" form with inputs for item properties (including dropdowns for unit types and multi-select tags).
* [ ] Build a confirmation prompt to prevent accidental item deletion.

---

## Page Point-of-Sale (POS) Checkout

### Active Item Grid & Cart Panel

* A side-by-side layout. On one side, a grid of items available for sale that users can click to add to a cart. On the other side, an interactive shopping cart listing the selected items, quantities, and payment details.
* Cashiers need to process customers quickly. They need to see the items, click to add them, adjust quantities easily, choose a payment method, and complete the checkout in a single screen.

### Tasks

* [x] Build a searchable list showing only items marked as "for sale."
* [x] Build a shopping cart panel that displays selected item details, subtotals, and adjustable quantity selectors.
* [x] Build checkout fields for Total Amount, Payment Method (dropdown), and optional transaction notes.
* [ ] Build a checkout confirmation dialog that resets the cart when the sale is completed.
* [ ] Build a split/mixed payment modal with a live balance remaining display.
* [ ] Integrate a global barcode keyboard listener to automatically scan and add items to the cart.
* [ ] Build a manual cash drawer override RFID card prompt.

---

## Page Staff Management

### Staff Profile Directory

* A dashboard listing employee profiles, positions, and pay details, alongside forms to manage them.
* The business owner needs to add new employees, update active cashiers, and configure hourly or daily pay rates.

### Tasks

* [ ] Build an employee details table showing names, positions, pay frequencies, and pay rates.
* [ ] Build the employee profile form to add new staff members or update existing credentials.

---

## Page Shift Attendance & Payroll Logs

### Historical Audit Trails

* A simple history panel showing employee clock-in/out logs and past payroll payments.
* The store manager needs a clear ledger to review cashier hours and verify that payroll was calculated and paid out correctly.

### Tasks

* [ ] Build a tabbed panel to swap between attendance ledger and payroll records.
* [ ] Build formatted tables for time logs and payroll history, converting dates, times, and financial totals into easy-to-read listings.

---

## Page Settings Menu

### App Settings & User Preferences

* Configuration options for individual staff settings (UI preferences, passwords) and business configurations (currency, limits).
* Allows administrators to modify application thresholds (business name, low stock levels, written receipt limits, card expiration days) and users to manage their profiles.

### Tasks

* [ ] Build a settings workspace page routing in `App.tsx` and its sidebar link.
* [ ] Build a User Settings sub-tab to allow staff members to change their password or details.
* [ ] Build a Business Settings sub-tab (admin-only view check) to configure:
  * Business Name (text field)
  * Currency (dropdown)
  * Low Stock Threshold (number field)
  * Written Receipt Threshold (number field)
  * Card Access rules and automatic expiration date offsets by Access Type (Staff, OneTime, Member).

---

## Testing Guide

If you want to proceed with testing using the backend, let me know and I'll update this document with the relevant information.

For now, here is a quick guide to test the frontend:

1. Open a command prompt or terminal in the Frontend directory.
2. Run the command to install the required packages:

   ```bash
   npm install
   ```

3. Start the local React development server by running:

   ```bash
   npm run dev
   ```

4. Open the web link shown in your terminal (usually `http://localhost:5173`) in your browser to view the application.
5. To stop the development server at any time, press `Ctrl + C` in your terminal window.

Since there is no live backend database connected yet, you should test the user interface using local mock data. You can hardcode sample arrays of items, staff members, and sales logs in your React state to verify that navigation, searches, the shopping cart calculations, and form interactions all work correctly.

If anything related to React goes wrong, you're gonna have to look it up on your own; that's no longer my domain. But if it's within the confines of the project itself, you can ask me any time. Good luck!

\\- DefinitelyRus
