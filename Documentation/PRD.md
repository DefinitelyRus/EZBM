# EZBM (Easy Business Manager) - Prototype PRD

## Project Overview & Goals

* Our purpose is to build a local, desktop-first app for tracking business tasks that is simpler and faster than a paper notebook.
* We are targeting micro-SMEs in the Philippines, like sari-sari stores, milk tea shops, gyms, and supply shops.
* The prototype should focus on building the bare minimum so we can see where users struggle. We want to avoid early optimizations, complex code, and cloud hosting for now.

## Technical Architecture & Data Strategy

* Both the front-end and back-end will run locally on a single machine (the merchant's PC).
* The front-end is built using React with a desktop-first layout.
* The back-end is an ASP.NET Core Web API that uses a clean service and repository pattern.
* The front-end and back-end talk to each other using a local REST API.
* For storing data, we use:
  * A local SQLite database (`ezbm.db`) for business data like users, attendance, inventory, and transactions.
  * A JSON file (`settings.json`) for application settings and UI preferences.

## Core Features & Functional Requirements

### Authentication (Simple Login)

* We want to provide basic access control for employees.
* The login screen should be very simple, asking only for a username and password.
* As a shortcut for this prototype, passwords are saved and checked in plain text. We do not need password hashing or secure tokens yet.

### Analytics Dashboard

* The merchant should be able to see how the business is doing at a glance.
* The dashboard will display total sales and total profit for today.
* An alerts section will list items that are running low (like having fewer than 5 units left) so they can be restocked.

### Inventory Management Tab

* The app needs to track what items the store offers.
* We support both physical products (which have stock counts that go down when sold) and services (like digital offerings that have unlimited stock).
* Merchants can add items using a simple form (specifying name, type, cost, price, and initial stock).
* An inventory table will list all items, and a search bar will let users filter items by name or code.

### Point-of-Sale (POS) Tab

* Cashiers need to process sales quickly.
* The POS tab will show a searchable list of products and services.
* An interactive cart will let cashiers select items, change quantities, choose a payment method, and see the checkout total.
* Clicking checkout will register the sale, save the transaction details, and update the stock levels.

### Employee Attendance

* We need a simple clock-in log.
* Cashiers will use a toggle button on the main sidebar or header to clock in and out.
* The action logs the user ID, action type (in or out), and the time directly into the database.

## Minimum Viable Database Schema (SQLite)

```sql
-- Users Table
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    Password TEXT NOT NULL, -- Plain text for prototype
    Role TEXT NOT NULL      -- Admin / Employee
);

-- Items Table (Handles both Products & Services)
CREATE TABLE Items (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Type TEXT NOT NULL,       -- 'Product' or 'Service'
    CostPrice REAL NOT NULL,
    RetailPrice REAL NOT NULL,
    StockQuantity INTEGER DEFAULT 0 -- Ignored if Type is 'Service'
);

-- Sales Table
CREATE TABLE Sales (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    TotalAmount REAL NOT NULL,
    FOREIGN KEY(UserId) REFERENCES Users(Id)
);

-- SaleItems Table
CREATE TABLE SaleItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SaleId INTEGER,
    ItemId INTEGER,
    Quantity INTEGER NOT NULL,
    PriceAtSale REAL NOT NULL,
    FOREIGN KEY(SaleId) REFERENCES Sales(Id),
    FOREIGN KEY(ItemId) REFERENCES Items(Id)
);

-- Attendance Table
CREATE TABLE Attendance (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,
    ActionType TEXT NOT NULL, -- 'In' or 'Out'
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(UserId) REFERENCES Users(Id)
);
```

## Explicitly Out of Scope (Do Not Build Yet)

* Password hashing, multi-factor login, or session expiration for this version.
* Database updates will not use complex migrations. If you change the database schema, just delete the database file and let the app recreate it on startup.
* We will not integrate physical hardware like barcode scanners yet (cashiers can search by text instead).
* Cloud syncing, cloud backups, and online APIs are out of scope.
* We will not build advanced payroll calculators or tax forms yet.
