# Endpoint Test Results

This document lists the test results for all the API endpoints in the EZBM project.

## How the Test Was Performed

The tests were performed using two methods:

1. **Code Analysis:** We inspected the C# source code for all endpoint controllers (`AuthController.cs`, `InventoryController.cs`, `SalesController.cs`, `StaffController.cs`) and their mapped routes in `Program.cs`.
2. **Test Runner Application:** We created a dedicated test project called `EZBM.Tests` in the `Backend` directory. This project calls the controller endpoints with mock payloads to verify their behavior.

During our setup, we found a compilation error in `Program.cs` where the attendance and payroll routes were mapped to non-existent controllers (`AttendanceController` and `PayrollController`). We corrected these routes to map to `StaffController` where the code is actually defined.

---

## Results by Controller

### 1. AuthController

* **`POST /api/auth/login`**: **✅ Succeeded.**
  * Authenticates successfully with correct credentials and returns the staff details.
  * Correctly returns `401 Unauthorized` with incorrect credentials.

---

### 2. StaffController

* **`POST /api/staff/create`**: **✅ Succeeded.**
* **`POST /api/staff/get`**: **✅ Succeeded.**
* **`POST /api/staff/find`**: **✅ Succeeded.**
* **`POST /api/staff/update`**: **✅ Succeeded.**
* **`POST /api/staff/delete`**: **✅ Succeeded.**
* **`POST /api/attendance`**: **✅ Succeeded.**
  * Logs staff attendance clock-in/out events and returns a structured JSON response with `success` and `timestamp`.
* **`POST /api/attendance/create`**: **✅ Succeeded.**
* **`POST /api/attendance/get`**: **✅ Succeeded.**
* **`POST /api/attendance/find`**: **✅ Succeeded.**
* **`POST /api/attendance/update`**: **✅ Succeeded.**
* **`POST /api/attendance/delete`**: **✅ Succeeded.**
* **`POST /api/payroll/create`**: **✅ Succeeded.**
* **`POST /api/payroll/get`**: **✅ Succeeded.**
* **`POST /api/payroll/find`**: **✅ Succeeded.**
* **`POST /api/payroll/delete`**: **✅ Succeeded.**

---

### 3. InventoryController

* **`GET /api/items`**: **✅ Succeeded.**
* **`POST /api/items/get`**: **✅ Succeeded.**
* **`POST /api/items/find`**: **✅ Succeeded.**
* **`POST /api/items/create`**: **✅ Succeeded.**
  * Creates a new inventory item and returns the created item object containing its database ID.
* **`POST /api/items/delete`**: **✅ Succeeded.**
* **`POST /api/items/transactions/create`**: **✅ Succeeded.**
* **`POST /api/items/transactions/get`**: **✅ Succeeded.**
* **`POST /api/items/transactions/find`**: **✅ Succeeded.**
* **`POST /api/items/transactions/delete`**: **✅ Succeeded.**

---

### 4. SalesController

* **`POST /api/sales` / `POST /api/sales/create`**: **✅ Succeeded.**
  * Processes checkout, deducts stock, records the sale, and returns a structured JSON response containing the generated `saleId`.
* **`POST /api/sales/get`**: **✅ Succeeded.**
* **`POST /api/sales/find`**: **✅ Succeeded.**
* **`POST /api/sales/delete`**: **✅ Succeeded.**
* **`POST /api/sales/entries/create`**: **✅ Succeeded.**
* **`POST /api/sales/entries/get`**: **✅ Succeeded.**
* **`POST /api/sales/entries/find`**: **✅ Succeeded.**
* **`POST /api/sales/entries/delete`**: **✅ Succeeded.**
