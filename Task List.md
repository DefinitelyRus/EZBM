# EZBM - Phase 1 Task List

> *Author(s): Google Gemini*

| Category | Stack | Task Description | Status |
| :--- | :--- | :--- | :--- |
| **1. Initial Setup** | Back-end | Create new .NET web API project and configure local port (`localhost`) | Todo |
| | Back-end | Initialize `ezbm.db` and execute SQLite creation script | Todo |
| | Back-end | Create default `settings.json` file | Todo |
| | Front-end | Boot clean desktop-first React project | Todo |
| | Front-end | Set up base API client (Axios/Fetch) pointed to local .NET server | Todo |
| **2. Authentication** | Back-end | Create `POST /api/auth/login` endpoint | Todo |
| | Back-end | Write SQLite query to check username/password (plain text) | Todo |
| | Back-end | Return user object on success, or 401 on login failure | Todo |
| | Front-end | Design distraction-free Login Screen | Todo |
| | Front-end | Save logged-in user state in React application | Todo |
| | Front-end | Show basic error message on login failure | Todo |
| **3. Inventory** | Back-end | Create `GET /api/items` endpoint | Todo |
| | Back-end | Create `POST /api/items` endpoint to save new item | Todo |
| | Back-end | Implement validation logic (prevent negative prices, empty names) | Todo |
| | Front-end | Build main grid layout with side nav / top tabs | Todo |
| | Front-end | Create Inventory Table (name, type, cost, retail, stock) | Todo |
| | Front-end | Add real-time Search Bar for table items | Todo |
| | Front-end | Build "Add New Item" Form (Name, Type, Cost, Retail, Initial Stock) | Todo |
| **4. POS** | Back-end | Create `POST /api/sales` endpoint for checkout | Todo |
| | Back-end | Implement transaction logic: insert Sales, SaleItems; deduct stock if Product | Todo |
| | Front-end | Create searchable item picker list to add to cart | Todo |
| | Front-end | Build Cart Component (item names, qty modifiers, total amount) | Todo |
| | Front-end | Add "Confirm Sale" Button to send cart to API and clear cart | Todo |
| **5. Attendance** | Back-end | Create `POST /api/attendance` endpoint | Todo |
| | Back-end | Write SQLite query to log user ID, action type (In/Out), timestamp | Todo |
| | Front-end | Place persistent "Clock In / Out" Toggle Button in header/sidebar | Todo |
| | Front-end | Update button text based on current status | Todo |
| | Front-end | Trigger alert notification for successful clock action | Todo |
| **6. Dashboard** | Back-end | Create `GET /api/dashboard/summary` endpoint | Todo |
| | Back-end | Write aggregation query for Today's Total Sales | Todo |
| | Back-end | Write aggregation query for Today's Total Profit | Todo |
| | Back-end | Write filter query for items with stock < 5 | Todo |
| | Front-end | Design default landing view with large metric cards | Todo |
| | Front-end | Build Alerts Box for low-stock items | Todo |
| | Front-end | Display "No current alerts" if sufficiently stocked | Todo |
