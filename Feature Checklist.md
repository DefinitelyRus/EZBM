# Feature Checklist

> *Author(s): DefinitelyRus, Google Gemini*

This is a step-by-step feature checklist split by front-end and back-end. The goal of this is to keep development entirely focused on the simplest possible implementation of the prototype.

## 1. Initial Project & Database Setup

* [ ] **Back-end (.NET):** Create a new web API project and configure the local port (`localhost`).
* [ ] **Back-end (SQLite):** Initialize the `ezbm.db` file and execute the database creation script.
* [ ] **Back-end (JSON):** Create a default `settings.json` file in the root folder for basic app configurations.
* [ ] **Front-end (React):** Boot a clean desktop-first React project.
* [ ] **Front-end (API Connection):** Set up a base API client (like Axios or native Fetch) pointed to the local .NET server.

## 2. Authentication (Simple Login)

### Back-end Requirements

* [ ] Create a `POST /api/auth/login` endpoint.
* [ ] Write an SQLite query to check if the entered username and password (plain text) match a record.
* [ ] Return a simple user object (ID, Username, Role) on success, or a 401 error code on failure.

### Front-end Requirements

* [ ] Design a simple, distraction-free Login Screen.
* [ ] Save the logged-in user's info in the React application state.
* [ ] Show a basic error message if the login fails.

## 3. Inventory Management Tab

### Back-end Requirements

* [ ] Create a `GET /api/items` endpoint to fetch all products and services.
* [ ] Create a `POST /api/items` endpoint to save a new item to SQLite.
* [ ] Implement back-end logic to handle validation (e.g., prevent negative prices, ensure names aren't empty).

### Front-end Requirements

* [ ] Build a main grid layout with a side navigation bar or top tabs.
* [ ] Create an **Inventory Table** displaying item name, type, cost price, retail price, and stock levels.
* [ ] Add a **Search Bar** that filters the visible table items in real-time as the user types.
* [ ] Build an **"Add New Item" Form** with inputs for:
* Item Name (text)
* Type (dropdown toggle: Product vs. Service)
* Cost Price (number)
* Retail Price (number)
* Initial Stock (number, disabled if "Service" is selected)

## 4. Point-of-Sale (POS) Tab

### Back-end Requirements

* [ ] Create a `POST /api/sales` endpoint to process a completed checkout.
* [ ] **Transaction Logic:** Ensure a single checkout performs two database actions:
* Insert a row into the `Sales` table and its matching rows into the `SaleItems` table.
* Subtract the purchased quantities from the `Items` table stock counts (only if the item type is a "Product").

### Front-end Requirements

* [ ] Create a searchable item picker list where clicking an item adds it to a checkout cart.
* [ ] Build the **Cart Component** that displays:
* Item names and current quantities.
* Quick action buttons to increase quantity (+), decrease quantity (-), or remove the item entirely.
* A live calculating **Total Amount** label.

* [ ] Add a prominent **"Confirm Sale" Button** that sends the cart data to the back-end and clears the cart on success.

## 5. Employee Attendance

### Back-end Requirements

* [ ] Create a `POST /api/attendance` endpoint.
* [ ] Write an SQLite query to log the user's ID, the action type ("In" or "Out"), and the exact server timestamp.

### Front-end Requirements

* [ ] Place a persistent **"Clock In / Out" Toggle Button** in the main header or sidebar.
* [ ] Update the button text depending on whether the current user is clocked in or out.
* [ ] Trigger an alert notification showing successful confirmation of the clock action.

## 6. Analytics Dashboard

### Back-end Requirements

* [ ] Create a `GET /api/dashboard/summary` endpoint.
* [ ] Write an aggregation query calculating total sales made during the current calendar day.
* [ ] Write an aggregation query calculating total gross profits made during the current calendar day.
* [ ] Write a filter query fetching all products with a stock quantity less than 5 units.

### Front-end Requirements

* [ ] Design the default landing view (Dashboard) with large, high-visibility metric cards for **Today's Sales** and **Today's Profit**.
* [ ] Build an **Alerts Box** that maps out the low-stock items returned by the back-end.
* [ ] Display a "No current alerts" placeholder message if all items are sufficiently stocked.