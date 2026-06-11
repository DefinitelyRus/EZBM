# Actionable Task List

> This file outlines the immediate next steps for the EZBM prototype, focusing on building the API endpoints and bootstrapping the frontend layout.

---

## `feat(api): build authentication endpoints`

**What:** Create the authentication API logic.

**How:**

- Create an `AuthController` in [EZBM.DesktopHost](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopHost).
- Find the user record by username using [StaffService](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.Core/Services/StaffService.cs).
- Verify the password in plain text.

**Subtasks:**

- [x] Create `AuthController.cs` file in the host API project.
- [x] Define the `POST /api/auth/login` route handler.
- [x] Parse request payload to extract username and password.
- [x] Query database users via `StaffService.GetStaffByUsernameAsync()` and find the match.
- [x] Return staff object (200 OK) if passwords match; return 401 Unauthorized if mismatch/not found.

**Example Request:**

```json
{
  "username": "jane_doe",
  "password": "password123"
}
```

**Example Success Response (200 OK):**

```json
{
  "id": 17163019283749,
  "username": "jane_doe",
  "position": "Admin",
  "payFrequency": "Hourly",
  "payRate": 150.0
}
```

**Example Error Response (401 Unauthorized):**

```json
{
  "error": "Invalid username or password."
}
```

**Complete when:**

- A client login request successfully authenticates and returns user details or a 401 status.

---

## `feat(api): build inventory endpoints`

**What:** Create endpoints to read and write items.

**How:**

- Create an `InventoryController` in [EZBM.DesktopHost](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopHost).
- Use [InventoryService](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.Core/Services/InventoryService.cs) to process database operations.

**Subtasks:**

- [x] Create `InventoryController.cs` in the host API project.
- [x] Implement `GET /api/items` endpoint:
  - Call `InventoryService.FindItemAsync(null)` to retrieve the item list.
  - Return the collection (200 OK).
- [/] Implement `POST /api/items` endpoint:
  - Read incoming item JSON.
  - Save the item to the database using `InventoryService.CreateItemAsync(request)`.
  - [ ] **DISCREPANCY/BUG:** Return the created item with its new database ID (currently returns an empty 200 OK response on success).

**Example GET Response (200 OK):**

```json
[
  {
    "id": 17163019284451,
    "name": "Coca-Cola 1.5L",
    "description": "1.5 Liter carbonated soft drink",
    "imageUrl": "https://example.com/coke.png",
    "isForSale": true,
    "cost": 50.0,
    "salePrice": 65.0,
    "quantity": 24.0,
    "unitOfMeasurement": "Count",
    "tags": ["Food", "Consumable"]
  }
]
```

**Example POST Request:**

```json
{
  "name": "Burger Combo",
  "description": "Burger with regular fries and drink",
  "imageUrl": null,
  "isForSale": true,
  "cost": 80.0,
  "salePrice": 120.0,
  "quantity": 10.0,
  "unitOfMeasurement": "Count",
  "tags": ["Food"]
}
```

**Example POST Response (200 OK):**

```json
{
  "id": 17163019289999,
  "name": "Burger Combo",
  "description": "Burger with regular fries and drink",
  "imageUrl": null,
  "isForSale": true,
  "cost": 80.0,
  "salePrice": 120.0,
  "quantity": 10.0,
  "unitOfMeasurement": "Count",
  "tags": ["Food"]
}
```

**Complete when:**

- Items can be retrieved and added through the API using a client or Swagger.

---

## `feat(api): build POS and attendance endpoints`

**What:** Create checkout and attendance endpoints.

**How:**

- Create `SalesController` in [EZBM.DesktopHost](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopHost) and implement attendance/payroll endpoints in `StaffController`.
- Use [SalesService](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.Core/Services/SalesService.cs) and [StaffService](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.Core/Services/StaffService.cs) for processing actions.

**Subtasks:**

- [x] Create `SalesController.cs` in the host API project and implement attendance/payroll endpoints in `StaffController.cs` (correcting mapping bugs in `Program.cs`).
- [/] Implement `POST /api/sales` endpoint:
  - Parse the request payload to instantiate a `Sale` record, create `SaleEntry` records, deduct stock, and execute database transactions via `SalesService.CreateSaleAsync(request)`.
  - [ ] **DISCREPANCY/BUG:** Return the success payload containing the generated `saleId` (currently returns an empty 200 OK response on success).
- [/] Implement `POST /api/attendance` endpoint:
  - Log clock-in/clock-out events via `StaffService.LogAttendanceAsync(request)`.
  - [ ] **DISCREPANCY/BUG:** Return the success payload containing `success` and `timestamp` fields (currently returns a raw DateTime string).

**Example POST /api/sales Request:**

```json
{
  "staffId": 17163019283749,
  "paymentMethod": "Cash",
  "totalAmount": 185.0,
  "notes": "Customer checkout",
  "items": [
    {
      "itemId": 17163019284451,
      "quantity": 1,
      "unitPrice": 65.0
    },
    {
      "itemId": 17163019289999,
      "quantity": 1,
      "unitPrice": 120.0
    }
  ]
}
```

**Example POST /api/sales Response (200 OK):**

```json
{
  "success": true,
  "saleId": 17163020019283
}
```

**Example POST /api/attendance Request:**

```json
{
  "staffId": 17163019283749,
  "actionType": "In"
}
```

**Example POST /api/attendance Response (200 OK):**

```json
{
  "success": true,
  "timestamp": "2026-06-04T18:59:24.456Z"
}
```

**Complete when:**

- Checking out deducts stock from items, logs transactions, and records the sale in SQLite.
- Attendance clock actions are saved successfully.

---

### `feat(api): build dashboard endpoints`

**What:** Create endpoints to compute metrics for the landing screen.

**How:**

- Create a `DashboardController` in [EZBM.DesktopHost](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopHost).
- Calculate total sales and profits for the current day.
- Fetch all items with a quantity less than 5.

**Subtasks:**

- [ ] Create `DashboardController.cs` in the host API project.
- [ ] Implement `GET /api/dashboard/summary` endpoint:
  - Query all sales records via `SalesService.GetAllSales()`, filtering for the current calendar date, and sum `TotalAmount` for `todaySales`.
  - Calculate `todayProfit` by subtracting item cost price from sale retail price for all items in today's sales.
  - Query inventory items via `InventoryService.GetAllItems()` and filter for products where `StockQuantity` is less than 5.
  - Return the computed dashboard summary payload (200 OK).

**Example Response (200 OK):**

```json
{
  "todaySales": 12450.0,
  "todayProfit": 3480.0,
  "lowStockAlerts": [
    {
      "id": 17163019284451,
      "name": "Coca-Cola 1.5L",
      "quantity": 3.0
    }
  ]
}
```

**Complete when:**

- The summary endpoint returns correct sums and lists low-stock products.

---

## `chore(config): setup default settings.json`

**What:** Create the default configuration file in the project.

**How:**

- Create `settings.json` in the root workspace directory.

**Subtasks:**

- [ ] Create `settings.json` file in the workspace root directory.
- [ ] Populate the file with default storefront settings.

**Example Content:**

```json
{
  "storeName": "My Store",
  "currency": "PHP"
}
```

**Complete when:**

- The configuration file is successfully created in the root directory.

---

## `init(frontend): bootstrap desktop-first React app`

**What:** Initialize the React application directory.

**How:**

- Create the project inside the `Frontend` directory using Vite with React + TypeScript templates.
- Install Axios.

**Subtasks:**

- [ ] Run Vite project creation tool inside the `Frontend` directory.
- [ ] Configure `package.json` scripts and run an initial installation.
- [ ] Install Axios for API communication.
- [ ] Set up the default dashboard layout and client file setup.

**Complete when:**

- Running `npm run dev` in the frontend folder launches the React app locally.
