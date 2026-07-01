# EZBM - Task List Status

| Category | Stack | Task Description | Status |
| :--- | :--- | :--- | :--- |
| **1. Initial Setup** | Back-end | Create new .NET web API project and configure local port (`localhost`) | Done |
| | Back-end | Initialize `ezbm.db` and execute SQLite creation script | Done |
| | Back-end | Create default `settings.json` file | Done |
| | Front-end | Boot clean desktop-first Razor Pages application | Done |
| | Front-end | Set up base API client pointed to local .NET server | Done |
| **2. Authentication** | Back-end | Create `POST /api/auth/login` endpoint | Done |
| | Back-end | Write SQLite query to check username/password | Done |
| | Back-end | Return user object on success, or 401 on login failure | Done |
| | Front-end | Design distraction-free Login Screen | Done |
| | Front-end | Save logged-in user state in session state | Done |
| | Front-end | Show basic error message on login failure | Done |
| **3. Inventory** | Back-end | Create `GET /api/items` endpoint | Done |
| | Back-end | Create `POST /api/items` endpoint to save new item | Done |
| | Back-end | Implement validation logic (prevent negative prices, empty names) | Done |
| | Front-end | Build main grid layout with side nav / top tabs | Done |
| | Front-end | Create Inventory Table (name, type, cost, retail, stock) | Done |
| | Front-end | Add real-time Search Bar for table items | Done |
| | Front-end | Build "Add New Item" Form (Name, Type, Cost, Retail, Initial Stock) | Done |
| **4. POS** | Back-end | Create `POST /api/sales` endpoint for checkout | Done |
| | Back-end | Implement transaction logic: insert Sales, SaleItems; deduct stock | Done |
| | Front-end | Create searchable item picker list to add to cart | Done |
| | Front-end | Build Cart Component (item names, qty modifiers, total amount) | Done |
| | Front-end | Add "Confirm Sale" Button to send cart to API and clear cart | Done |
| **5. Attendance & Payroll** | Back-end | Create `POST /api/attendance` endpoint | Done |
| | Back-end | Write SQLite query to log user ID, action type (In/Out), timestamp | Done |
| | Front-end | Place persistent "Clock In / Out" Toggle Button in layout | Done |
| | Front-end | Update button text based on current status | Done |
| | Front-end | Trigger alert notification for successful clock action | Done |
| **6. Dashboard** | Back-end | Create `GET /api/dashboard/summary` endpoint | Done |
| | Back-end | Write aggregation query for Today's Total Sales | Done |
| | Back-end | Write aggregation query for Today's Total Profit | Done |
| | Back-end | Write filter query for items with stock < settings low stock threshold | Done |
| | Front-end | Design default landing view with large metric cards | Done |
| | Front-end | Build Alerts Box for low-stock items | Done |
| | Front-end | Display "No current alerts" if sufficiently stocked | Done |

## Added New Features (Phase 2-7)

| Category | Description | Status |
| :--- | :--- | :--- |
| **User Reparenting** | Map User hierarchy (Staff, Customer) under a single table (TPH) with lazy card permissions expiry evaluation | Done |
| **Action Log Audit Tracking** | Intercept and log edit/delete events automatically inside DbContext to system ActionLogs audit ledger | Done |
| **Mixed Payments & Split Pay** | Support multiple payment channels on a single transaction with parent-child transaction decoupling | Done |
| **Hardware Emulations** | Simulate RFID drawer opening manual overrides and barcode keyboard scan inputs | Done |
| **Upgrade Commissions** | Enforce net upgrade payroll commissions (deducting previously earned lower tier commissions) | Done |
| **Settings Management** | Manage storefront parameters dynamically via `/api/settings` REST endpoints and custom view page | Done |
| **Recent Sales Container** | Feed dashboard index with the latest 10 sales transactions log feed side container | Done |
