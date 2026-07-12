# EZBM API Documentation

This guide describes the local web API for the EZBM application. It acts as the bridge between the React frontend and the core database logic.

## Key Components

* The `Endpoints/` folder maps web requests directly to backend actions like inventory or sales.
* `Program.cs` sets up server routing rules, handles dependency injection, and initializes the database on startup.

## Getting Started with Consuming APIs

If you are new to working with web APIs, you can think of an API as a menu in a restaurant. You (the client) make a request to the kitchen (the server), and the kitchen sends back the food (the response).

Web developers commonly use JavaScript to communicate with APIs using the `fetch` function. Here is a simple breakdown of how this works:

> **Important:** The API host **must** be running before any of this will work! See [How to Run & Host](#how-to-run--host) below first if you haven't already.

### Making a GET Request

Use a `GET` request when you want to retrieve data from the server.

```javascript
// Fetch all inventory items
fetch('https://localhost:5001/api/items')
  .then(response => response.json()) // Convert raw data into a JavaScript object
  .then(data => {
    console.log("Here are the items:", data);
  })
  .catch(error => {
    console.error("Something went wrong:", error);
  });
```

### Making a POST Request

Use a `POST` request when you want to send data to the server (e.g. creating an item, logging in, or checking out). In this case, you must:

1. Specify the `method` as `'POST'`.
2. Add the `'Content-Type': 'application/json'` header so the server knows you are sending JSON.
3. Stringify your data object using `JSON.stringify()`.

```javascript
// Add a new inventory item
const newItem = {
  name: "Orange Juice",
  isForSale: true,
  quantity: 15.0,
  unitOfMeasurement: "Count"
};

fetch('https://localhost:5001/api/items/create', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'X-Operator-Username': 'teto' // Let the server know who is creating this item
  },
  body: JSON.stringify(newItem)
})
  .then(response => response.json())
  .then(data => {
    console.log("Item successfully created:", data);
  })
  .catch(error => {
    console.error("Failed to create item:", error);
  });
```

## How to Run & Host

To run the local REST server in the development environment:

```bash
# Execute from the project folder
dotnet run --project EZBM.DesktopHost.csproj

# Alternatively, from the repository root
dotnet run --project Backend/EZBM.DesktopHost/EZBM.DesktopHost.csproj
```

The server runs on HTTP/HTTPS local host ports defined in `appsettings.json` and `Properties/launchSettings.json`.
You can access the auto-generated Swagger/OpenAPI documentation (in development mode) at `http://localhost:<port>/openapi/v1.json` or by examining the mapped endpoints list in `Program.cs`.

## How to Consume the API

The frontend consumes these endpoints by sending standard HTTP requests to `localhost`.

### Request Headers

Every request should include the active operator's username in the header:

* `X-Operator-Username`: `username` (e.g. `teto`)

If this header is missing, the backend defaults the operator name to `System` for audit logging.

### Example Request (JavaScript Fetch)

Here is how you can fetch the inventory list from the frontend:

```javascript
fetch('https://localhost:5001/api/items', {
  method: 'GET',
  headers: {
    'Content-Type': 'application/json',
    'X-Operator-Username': 'teto'
  }
})
.then(response => response.json())
.then(data => console.log(data))
.catch(error => console.error('Error:', error));
```

## Mapped API Endpoints Reference

Below is a detailed guide for every endpoint including request formats and expected responses.

### Authentication

#### `POST /api/auth/login`

Checks credentials to log a user in.

* **Request Body:**

  ```json
  {
    "username": "teto",
    "password": "correct_password"
  }
  ```

* **Success Response (200 OK):**

  ```json
  {
    "id": 1,
    "username": "teto",
    "position": "Cashier",
    "payFrequency": "Daily",
    "payRate": 150.0
  }
  ```

* **Error Response (401 Unauthorized):**

  ```json
  {
    "error": "Invalid username or password."
  }
  ```

### Inventory Management

#### `GET /api/items`

Retrieves all items currently in the catalog.

* **Success Response (200 OK):**

  ```json
  [
    {
      "id": 1,
      "name": "Coca-Cola 1.5L",
      "description": "1.5 liter soft drink bottle",
      "isForSale": true,
      "cost": 45.0,
      "salePrice": 55.0,
      "quantity": 10.0,
      "unitOfMeasurement": "Count",
      "expirationDate": "2026-12-31T00:00:00Z",
      "tags": ["Food", "Consumable"]
    }
  ]
  ```

#### `POST /api/items/get`

Gets the details of a single item by its ID.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**

  ```json
  {
    "id": 1,
    "name": "Coca-Cola 1.5L",
    "description": "1.5 liter soft drink bottle",
    "isForSale": true,
    "cost": 45.0,
    "salePrice": 55.0,
    "quantity": 10.0,
    "unitOfMeasurement": "Count",
    "expirationDate": "2026-12-31T00:00:00Z",
    "tags": ["Food", "Consumable"]
  }
  ```

* **Error Response (404 Not Found):**
  * Status code is 404 if the item ID does not exist.

#### `POST /api/items/find`

Searches for items matching query parameters.

* **Request Body:** (all fields are optional)

  ```json
  {
    "id": 1,
    "name": "Coca",
    "isForSale": true,
    "minCost": 10.0,
    "maxCost": 100.0,
    "minSalePrice": 10.0,
    "maxSalePrice": 120.0,
    "minQuantity": 0.0,
    "maxQuantity": 50.0,
    "unitOfMeasurement": "Count",
    "minExpirationDate": "2026-01-01T00:00:00Z",
    "maxExpirationDate": "2026-12-31T23:59:59Z",
    "tags": ["Food"],
    "brand": "Coca-Cola",
    "barcode": "4801234567890"
  }
  ```

* **Success Response (200 OK):**
  * Returns an array of matching item objects.

#### `POST /api/items/create`

Adds a new product or service.

* **Request Body:**

  ```json
  {
    "name": "Noodles",
    "description": "Instant cup noodles",
    "isForSale": true,
    "cost": 15.0,
    "salePrice": 22.0,
    "quantity": 25.0,
    "unitOfMeasurement": "Count",
    "expirationDate": "2026-10-15T00:00:00Z",
    "tags": ["Food"],
    "imageUrl": "http://example.com/noodles.png",
    "itemType": "Product",
    "barcode": "4801234567890",
    "targetStock": 50.0,
    "lowStockThresholdPercentage": 0.20,
    "brand": "NoodleBrand"
  }
  ```

* **Success Response (200 OK):**
  * Returns the newly created item object with its assigned database `id`.

#### `POST /api/items/delete`

Deletes an item from the inventory database.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful deletion.

### Inventory Transactions

#### `POST /api/items/transactions/create`

Records a stock adjustment movement (like restocking or recording waste).

* **Request Body:**

  ```json
  {
    "itemId": 1,
    "type": "NewStock", // Valid types: NewStock, Sale, Consumed, Damaged_Lost_Expired, Correction_Sum, Correction_Set
    "quantity": 5.0,
    "staffId": 1,
    "timestamp": "2026-07-05T14:00:00Z",
    "note": "Weekly restock"
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful transaction creation.

#### `POST /api/items/transactions/get`

Retrieves a single stock transaction record.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**

  ```json
  {
    "id": 1,
    "itemId": 1,
    "type": "NewStock",
    "quantity": 5.0,
    "staffId": 1,
    "timestamp": "2026-07-05T14:00:00Z",
    "note": "Weekly restock"
  }
  ```

#### `POST /api/items/transactions/find`

Searches for stock transaction records.

* **Request Body:** (all fields are optional)

  ```json
  {
    "id": 1,
    "minQuantity": 1.0,
    "maxQuantity": 10.0,
    "type": "NewStock",
    "minTimestamp": "2026-07-01T00:00:00Z",
    "maxTimestamp": "2026-07-31T23:59:59Z",
    "note": "restock"
  }
  ```

* **Success Response (200 OK):**
  * Returns an array of matching transaction objects.

#### `POST /api/items/transactions/delete`

Deletes a stock transaction record.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful deletion.

### Sales & Checkouts

#### `POST /api/sales` or `POST /api/sales/create`

Processes checkout for an order. Supports single payments, split/mixed payments, and promo code discounts.

* **Request Body:**

  ```json
  {
    "staffId": 1,
    "paymentMethod": "Cash", // Cash, EWallet, Savings, Credit, Mixed, Other
    "totalAmount": 110.0,
    "notes": "Fast checkout",
    "items": [
      {
        "itemId": 1,
        "quantity": 2.0,
        "unitPrice": 55.0
      }
    ],
    "splitPayments": [
      {
        "paymentMethod": "Cash",
        "amount": 50.0
      },
      {
        "paymentMethod": "EWallet",
        "amount": 60.0
      }
    ],
    "promoCode": "WELCOME10",
    "customerId": 5
  }
  ```

* **Success Response (200 OK):**

  ```json
  {
    "success": true,
    "saleId": 123
  }
  ```

#### `POST /api/sales/get`

Retrieves a specific sale record by ID.

* **Request Body:**

  ```json
  {
    "id": 123
  }
  ```

* **Success Response (200 OK):**
  * Returns the full sale record object with item entries and split payment transactions.

#### `POST /api/sales/find`

Searches for sale records.

* **Request Body:** (all fields are optional)

  ```json
  {
    "id": 123,
    "invoiceNumber": 10001,
    "staffId": 1,
    "paymentMethod": "Cash",
    "minAmount": 50.0,
    "maxAmount": 500.0,
    "customerId": 5
  }
  ```

* **Success Response (200 OK):**
  * Returns an array of matching sale objects.

#### `POST /api/sales/delete`

Deletes a sale record.

* **Request Body:**

  ```json
  {
    "id": 123
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful deletion.

### Sale Item Entries

#### `POST /api/sales/entries/create`

Manually adds an item entry to a sale record.

* **Request Body:**

  ```json
  {
    "saleId": 123,
    "itemId": 1,
    "quantity": 2.0,
    "unitPrice": 55.0,
    "subtotal": 110.0
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful creation.

#### `POST /api/sales/entries/get`

Retrieves details of a single sale item entry.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Returns the sale entry object.

#### `POST /api/sales/entries/find`

Searches for sale item entries.

* **Request Body:** (all fields are optional)

  ```json
  {
    "id": 1,
    "saleId": 123,
    "itemId": 1,
    "minQuantity": 1.0,
    "maxQuantity": 5.0
  }
  ```

* **Success Response (200 OK):**
  * Returns an array of matching sale entry objects.

#### `POST /api/sales/entries/delete`

Deletes a sale item entry.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful deletion.

### Staff Profiles

#### `POST /api/staff/create`

Adds a new employee profile.

* **Request Body:**

  ```json
  {
    "username": "teto",
    "password": "teto_password",
    "firstName": "Teto",
    "lastName": "Kasane",
    "email": "teto@example.com",
    "phoneNumber": "09123456789",
    "position": "Cashier",
    "payFrequency": "Daily", // Hourly, Daily, Weekly, Biweekly, Monthly, Invalid
    "payRate": 150.0,
    "commissionRate": 0.05,
    "rfidCardId": "RFID-123456",
    "permissions": ["OpenRegister", "ApplyDiscount"],
    "permissionsAfterExpiry": ["ClockIn"],
    "expirationDate": "2027-12-31T23:59:59Z",
    "roleIds": [1, 2]
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful profile creation.

#### `POST /api/staff/get`

Retrieves an employee's details by ID.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Returns the full staff profile object.

#### `POST /api/staff/find`

Searches for employee profiles.

* **Request Body:** (all fields are optional)

  ```json
  {
    "id": 1,
    "username": "teto",
    "position": "Cashier",
    "payFrequency": "Daily"
  }
  ```

* **Success Response (200 OK):**
  * Returns an array of matching staff profiles.

#### `POST /api/staff/update`

Updates an employee's details.

* **Request Body:**

  ```json
  {
    "id": 1,
    "username": "teto_new",
    "position": "Senior Cashier",
    "payRate": 180.0,
    "commissionRate": 0.06,
    "rfidCardId": "RFID-123456-NEW",
    "permissions": ["OpenRegister", "ApplyDiscount", "ManageStaff"],
    "permissionsAfterExpiry": ["ClockIn"],
    "expirationDate": "2028-12-31T23:59:59Z",
    "roleIds": [1, 3]
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful update.

#### `POST /api/staff/delete`

Deletes an employee profile.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful deletion.

### Shift Attendance

#### `POST /api/attendance`

Logs a clock-in/out event (useful for simple toggle action buttons).

* **Request Body:**

  ```json
  {
    "staffId": 1,
    "actionType": "In" // Valid values: In, Out
  }
  ```

* **Success Response (200 OK):**

  ```json
  {
    "success": true,
    "timestamp": "2026-07-05T14:30:00Z"
  }
  ```

#### `POST /api/attendance/create`

Creates a manual attendance record.

* **Request Body:**

  ```json
  {
    "staffId": 1,
    "timeIn": "2026-07-05T08:00:00Z",
    "timeOut": "2026-07-05T17:00:00Z"
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful record creation.

#### `POST /api/attendance/get`

Retrieves a specific attendance record.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Returns the attendance record object.

#### `POST /api/attendance/find`

Searches for attendance records.

* **Request Body:** (all fields are optional)

  ```json
  {
    "id": 1,
    "staffId": 1,
    "minTimeIn": "2026-07-01T00:00:00Z",
    "maxTimeIn": "2026-07-31T23:59:59Z"
  }
  ```

* **Success Response (200 OK):**
  * Returns an array of matching attendance records.

#### `POST /api/attendance/update`

Updates an attendance record (like modifying clock times).

* **Request Body:**

  ```json
  {
    "id": 1,
    "timeOut": "2026-07-05T17:30:00Z"
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful update.

#### `POST /api/attendance/delete`

Deletes an attendance record.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful deletion.

### Payroll Records

#### `POST /api/payroll/create`

Logs a payroll payment distribution.

* **Request Body:**

  ```json
  {
    "staffId": 1,
    "periodStart": "2026-06-16T00:00:00Z",
    "periodEnd": "2026-06-30T23:59:59Z",
    "totalHours": 80.0,
    "grossAmount": 12000.0,
    "modifiers": -500.0,
    "netAmount": 11500.0,
    "payDate": "2026-07-01T10:00:00Z",
    "notes": "Basic salary payment minus advances"
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful record creation.

#### `POST /api/payroll/get`

Retrieves a specific payroll record.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Returns the payroll record object.

#### `POST /api/payroll/find`

Searches for payroll records.

* **Request Body:** (all fields are optional)

  ```json
  {
    "id": 1,
    "staffId": 1,
    "minPeriodStart": "2026-06-01T00:00:00Z",
    "maxPeriodStart": "2026-06-30T23:59:59Z"
  }
  ```

* **Success Response (200 OK):**
  * Returns an array of matching payroll records.

#### `POST /api/payroll/delete`

Deletes a payroll record.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Empty response indicating successful deletion.

### Customer Profiles

#### `POST /api/customers/create`

Adds a new customer profile.

* **Request Body:**

  ```json
  {
    "firstName": "Teto",
    "lastName": "Kasane",
    "email": "teto@example.com",
    "phoneNumber": "09123456789",
    "rfidCardId": "RFID-MEMBER-123",
    "permissions": ["DiscountEligible"],
    "permissionsAfterExpiry": [],
    "expirationDate": "2027-12-31T23:59:59Z"
  }
  ```

* **Success Response (200 OK):**
  * Returns the newly created customer object.

#### `POST /api/customers/get`

Retrieves details of a customer by ID.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Returns the customer profile object.

#### `POST /api/customers/find`

Searches for customer profiles.

* **Request Body:** (all fields are optional)

  ```json
  {
    "id": 1,
    "firstName": "Teto",
    "lastName": "Kasane",
    "phoneNumber": "09123456789",
    "email": "teto@example.com",
    "rfidCardId": "RFID-MEMBER-123"
  }
  ```

* **Success Response (200 OK):**
  * Returns an array of matching customer profiles.

#### `POST /api/customers/update`

Updates a customer's details.

* **Request Body:**

  ```json
  {
    "id": 1,
    "firstName": "Teto New",
    "email": "tetonew@example.com"
  }
  ```

* **Success Response (200 OK):**
  * Returns `{"success": true}`.

#### `POST /api/customers/delete`

Deletes a customer profile.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Returns `{"success": true}`.

### Permissions Roles

#### `POST /api/roles/create`

Creates a new permissions role.

* **Request Body:**

  ```json
  {
    "name": "Cashier",
    "permissionsJson": "{\"ApplyDiscount\":1,\"OpenRegister\":1}"
  }
  ```

* **Success Response (200 OK):**
  * Returns the newly created role object.

#### `POST /api/roles/get`

Retrieves role details by ID.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Returns the role object.

#### `POST /api/roles/find`

Searches for roles.

* **Request Body:** (all fields are optional)

  ```json
  {
    "id": 1,
    "name": "Cashier"
  }
  ```

* **Success Response (200 OK):**
  * Returns an array of matching role objects.

#### `POST /api/roles/update`

Updates a role's configurations.

* **Request Body:**

  ```json
  {
    "id": 1,
    "name": "Senior Cashier",
    "permissionsJson": "{\"ApplyDiscount\":1,\"OpenRegister\":1,\"Refund\":1}"
  }
  ```

* **Success Response (200 OK):**
  * Returns `{"success": true}`.

#### `POST /api/roles/delete`

Deletes a permissions role.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Returns `{"success": true}`.

### Staff Adjustments

#### `POST /api/staff/adjustments/create`

Adds a payroll adjustment (bonus or salary advance) for a staff member.

* **Request Body:**

  ```json
  {
    "staffId": 1,
    "adjustmentType": "Bonus",
    "amount": 250.0,
    "deductFromCurrentPayroll": false,
    "isPaid": false,
    "notes": "Excellent cashier performance bonus"
  }
  ```

* **Success Response (200 OK):**
  * Returns the newly created staff adjustment object.

#### `POST /api/staff/adjustments/get`

Retrieves details of a staff adjustment.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Returns the staff adjustment object.

#### `POST /api/staff/adjustments/find`

Searches for staff payroll adjustments.

* **Request Body:** (all fields are optional)

  ```json
  {
    "id": 1,
    "staffId": 1,
    "adjustmentType": "Bonus",
    "deductFromCurrentPayroll": false,
    "isPaid": false
  }
  ```

* **Success Response (200 OK):**
  * Returns an array of matching staff adjustment records.

#### `POST /api/staff/adjustments/delete`

Deletes a staff payroll adjustment.

* **Request Body:**

  ```json
  {
    "id": 1
  }
  ```

* **Success Response (200 OK):**
  * Returns `{"success": true}`.

### Action Audit Logs

#### `GET /api/logs`

Retrieves all system operational logs and audit traces sorted by most recent.

* **Success Response (200 OK):**

  ```json
  [
    {
      "id": 1,
      "actionType": "Edit",
      "operatorUsername": "teto",
      "details": "Edit on Item (ID: 1). Changes: Name: 'Coke' -> 'Coca-Cola'",
      "timestamp": "2026-07-05T14:30:00Z"
    }
  ]
  ```

### System Settings

#### `GET /api/settings`

Reads active storefront storefront settings (like warning thresholds).

* **Success Response (200 OK):**

  ```json
  {
    "storeName": "EZBM Store",
    "currency": "PHP",
    "lowStockThreshold": 5.0,
    "writtenReceiptThreshold": 1000.0,
    "storeOpeningDate": "2026-07-05T06:00:00Z",
    "cardExpirationOffsets": {
      "Staff": 365,
      "OneTime": 1,
      "Member": 30
    },
    "membershipCommissions": {
      "Silver Upgrade": 10.0,
      "Gold Upgrade": 25.0,
      "Platinum Upgrade": 50.0
    }
  }
  ```

#### `POST /api/settings`

Saves updated storefront settings.

* **Request Body:**

  ```json
  {
    "storeName": "EZBM Superstore",
    "currency": "PHP",
    "lowStockThreshold": 8.0,
    "writtenReceiptThreshold": 1200.0,
    "cardExpirationOffsets": {
      "Staff": 365,
      "OneTime": 1,
      "Member": 30
    },
    "membershipCommissions": {
      "Silver Upgrade": 12.0,
      "Gold Upgrade": 30.0,
      "Platinum Upgrade": 60.0
    }
  }
  ```

* **Success Response (200 OK):**

  ```json
  {
    "success": true
  }
  ```

#### `GET /api/settings/user/{id}`

Reads the custom UI layout and dashboard preferences JSON string for a specific user ID.

* **Success Response (200 OK):**
  * Returns the raw user preference JSON string.

#### `POST /api/settings/user/{id}`

Persists custom UI layouts, card sorting, or dashboard layout preference JSON string for a specific user ID.

* **Request Body:** Raw JSON preferences dictionary payload.
* **Success Response (200 OK):**

  ```json
  {
    "success": true
  }
  ```

### Dashboard Analytics

#### `GET /api/dashboard/analytics`

Computes shop-wide dashboard metrics (sales, gross/net profits, 7-day sales trends, top popular products, cashier performance leaderboards, and low stock warnings).

* **Success Response (200 OK):**

  ```json
  {
    "totalSales": 1540.50,
    "grossProfit": 450.20,
    "netProfit": 250.00,
    "salesVolumeTrends": [
      {
        "date": "2026-07-05",
        "totalAmount": 150.0,
        "salesCount": 3
      }
    ],
    "popularProducts": [
      {
        "itemId": 1,
        "name": "Fresh Red Apple",
        "totalQuantitySold": 12.0,
        "totalRevenue": 11.88
      }
    ],
    "cashierLeaderboard": [
      {
        "cashierId": 1,
        "username": "cashier_alice",
        "salesCount": 5,
        "totalRevenue": 250.0
      }
    ],
    "lowStockAlerts": [
      {
        "id": 1,
        "name": "Fresh Red Apple",
        "quantity": 10.0,
        "targetStock": 200.0,
        "lowStockThresholdPercentage": 0.20
      }
    ]
  }
  ```

### Google Drive Sync

#### `POST /api/sync/backup`

Uploads and syncs a database backup file to the simulated cloud backup directory.

* **Success Response (200 OK):**

  ```json
  {
    "success": true,
    "message": "Database backup successfully uploaded to Google Drive.",
    "fileName": "business_data_drive_sync_20260705_190000.db",
    "timestamp": "2026-07-05T11:00:00Z"
  }
  ```

#### `POST /api/sync/restore`

Restores the active database file using a specified backup file.

* **Query Parameters:**
  * `backupName` (string): The backup database filename to restore.

* **Success Response (200 OK):**

  ```json
  {
    "success": true,
    "message": "Database successfully restored from Google Drive backup."
  }
  ```

### Extended Calculations & Helpers

#### `GET /api/payroll/calculate`

Computes active payroll metrics (total hours, gross pay, commissions/bonuses, deductions, and net pay) for a staff member over a given window.

* **Query Parameters:**
  * `staffId` (ulong): The ID of the staff member.
  * `periodStart` (DateTime): Start of the payroll period.
  * `periodEnd` (DateTime): End of the payroll period.

* **Success Response (200 OK):**

  ```json
  {
    "totalHours": 40.5,
    "grossAmount": 810.0,
    "commissionsAndBonuses": 50.0,
    "deductions": 20.0,
    "netAmount": 840.0,
    "adjustmentIds": [123, 456]
  }
  ```

#### `GET /api/items/barcode/{code}`

Looks up a specific catalog product or service by its barcode.

* **Success Response (200 OK):**
  * Returns the matching product/service item object.

* **Error Response (404 Not Found):**
  * Returns `{"error": "Item with barcode '{code}' not found."}` if barcode does not match.

### Static Enums & Tags Metadata

#### `GET /api/enums`

Retrieves all static lists and enum values in the system in a single call.

* **Success Response (200 OK):**

  ```json
  {
    "units": ["Count", "Milligrams", "Grams", "Kilograms", "Ounces", "Pounds", "Milliliters", "Liters", "Gallons", "Unlimited"],
    "tags": ["Food", "Hygiene", "Consumable", "Reusable"],
    "accessCardTypes": ["None", "Staff", "OneTime", "Member"],
    "payFrequencies": ["Hourly", "Daily", "Weekly", "Biweekly", "Monthly", "Invalid"],
    "stockTransactionTypes": ["NewStock", "Sale", "Consumed", "Damaged_Lost_Expired", "Correction_Sum", "Correction_Set"],
    "transactionTypes": ["Income", "Expense", "Correction"],
    "paymentMethods": ["Cash", "EWallet", "Savings", "Credit", "Mixed", "Other"]
  }
  ```

#### Individual Enum Endpoints

You can also fetch each list individually via standard `GET` requests:

* `GET /api/enums/units` - Returns units of measurement array.
* `GET /api/enums/tags` - Returns default tags array.
* `GET /api/enums/access-cards` - Returns access card types array.
* `GET /api/enums/frequencies` - Returns pay frequency types array.
* `GET /api/enums/stock-transaction-types` - Returns stock transaction types array.
* `GET /api/enums/transaction-types` - Returns transaction types array.
* `GET /api/enums/payment-methods` - Returns payment methods array.

### Grid Pagination & Sorting

All `POST /api/.../find` search requests accept optional sorting and pagination fields:

* **Paging fields:**
  * `limit` (int): Number of records to return.
  * `offset` (int): Number of records to skip.
* **Sorting fields:**
  * `sortBy` (string): The property name to sort by (e.g. `Name`, `Quantity`, `Id`, `CreatedAt`).
  * `sortOrder` (string): Sort order direction (`Ascending` or `Descending`).
