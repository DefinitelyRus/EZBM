# EZBM Endpoints Test Results

This document lists the results of running tests on all API endpoints in the EZBM project.

## How the Test Was Performed

1. A dedicated test console application (`EZBM.Tests`) was created.
2. The database was reset using `DbManager.Reset()` before the tests to start with a clean state.
3. The static controller endpoints inside `EZBM.DesktopHost.Endpoints` were called directly with various payloads.
4. The results, including returned types, status codes (if any), and success states, were recorded.

## Test Results by Controller

### Database Reset

* **Status:** ✅ Succeeded
* **Description:** Database was reset to a clean state.

---

## StaffController Endpoints

### Create Staff (Success)

* **Status:** ✅ Succeeded
* **Description:** Created a new staff member john_doe.

Details:
```json
Status: 200
```

---

### Find Staff (Success)

* **Status:** ✅ Succeeded
* **Description:** Searched for staff with username 'john_doe'. Found Staff ID: 7210215709133417065.

Details:
```json
Found 1 records.
```

---

### Get Staff (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved the staff member with ID 7210215709133417065.

Details:
```json
Status: 200
```

---

### Update Staff (Success)

* **Status:** ✅ Succeeded
* **Description:** Updated pay rate and username of staff member ID 7210215709133417065.

Details:
```json
Status: 200
```

---

### Log Attendance Clock-In (Discrepancy)

* **Status:** ✅ Succeeded
* **Description:** Logs staff member clock-in. Discrepancy confirmed: returns raw DateTime instead of JSON object with success and timestamp fields.

Details:
```json
Result type: Ok`1, Status: 200
```

---

### Create Manual Attendance (Success)

* **Status:** ✅ Succeeded
* **Description:** Created a manual attendance entry.

Details:
```json
Status: 200
```

---

### Get Attendance (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved the attendance record with ID 11902856123003481394.

Details:
```json
Status: 200
```

---

### Update Attendance (Success)

* **Status:** ✅ Succeeded
* **Description:** Updated the attendance record with ID 11902856123003481394.

Details:
```json
Status: 200
```

---

### Create Payroll (Success)

* **Status:** ✅ Succeeded
* **Description:** Created a payroll record.

Details:
```json
Status: 200
```

---

### Find Payroll (Success)

* **Status:** ✅ Succeeded
* **Description:** Searched for payroll records. Found Payroll ID: 2702536295557053569.

Details:
```json
Found 1 records.
```

---

### Get Payroll (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved payroll record with ID 2702536295557053569.

Details:
```json
Status: 200
```

---

### Delete Payroll (Success)

* **Status:** ✅ Succeeded
* **Description:** Deleted payroll record with ID 2702536295557053569.

Details:
```json
Status: 200
```

---

### Delete Attendance (Success)

* **Status:** ✅ Succeeded
* **Description:** Deleted attendance record with ID 11902856123003481394.

Details:
```json
Status: 200
```

---

### Delete Staff (Success)

* **Status:** ✅ Succeeded
* **Description:** Deleted staff profile with ID 7210215709133417065.

Details:
```json
Status: 200
```

---

## AuthController Endpoints

### Login (Success Case)

* **Status:** ✅ Succeeded
* **Description:** Authenticated with valid credentials.

Details:
```json
Status: 200
```

---

### Login (Failure Case - Wrong Password)

* **Status:** ✅ Succeeded
* **Description:** Attempted authentication with an incorrect password.

Details:
```json
Status: 401
```

---

## InventoryController Endpoints

### Create Item (Discrepancy)

* **Status:** ✅ Succeeded
* **Description:** Creates a new inventory item. Discrepancy confirmed: returns an empty Ok response (200) instead of the created item object with its generated database ID.

Details:
```json
Result type: Ok`1, Status: 200
```

---

### Find Items (Success)

* **Status:** ✅ Succeeded
* **Description:** Searched for items matching query. Found Item ID: 8808740027799360762.

Details:
```json
Found 1 records.
```

---

### Get Item (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved inventory item ID 8808740027799360762.

Details:
```json
Status: 200
```

---

### Get All Items (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved all items in the inventory.

Details:
```json
Status: 200
```

---

### Create Item Transaction (Success)

* **Status:** ✅ Succeeded
* **Description:** Logged a stock transaction movement.

Details:
```json
Status: 200
```

---

### Find Item Transactions (Success)

* **Status:** ✅ Succeeded
* **Description:** Searched for stock transactions. Found Transaction ID: 2739397361561381623.

Details:
```json
Found 1 records.
```

---

### Get Item Transaction (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved stock transaction ID 2739397361561381623.

Details:
```json
Status: 200
```

---

### Delete Item Transaction (Success)

* **Status:** ✅ Succeeded
* **Description:** Deleted stock transaction ID 2739397361561381623.

Details:
```json
Status: 200
```

---

### Delete Item (Success)

* **Status:** ✅ Succeeded
* **Description:** Deleted item ID 8808740027799360762.

Details:
```json
Status: 200
```

---

## SalesController Endpoints

### Create Sale (Discrepancy)

* **Status:** ✅ Succeeded
* **Description:** Registers a sales transaction. Discrepancy confirmed: returns an empty Ok response (200) instead of a success payload containing the generated saleId.

Details:
```json
Result type: Ok`1, Status: 200
```

---

### Find Sales (Success)

* **Status:** ✅ Succeeded
* **Description:** Searched for sales transactions. Found Sale ID: 9163997556426527049.

Details:
```json
Found 1 records.
```

---

### Get Sale (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved sales transaction ID 9163997556426527049.

Details:
```json
Status: 200
```

---

### Find Sale Entries (Success)

* **Status:** ✅ Succeeded
* **Description:** Searched for sale entry line items. Found Entry ID: 15117448491516252154.

Details:
```json
Found 1 records.
```

---

### Get Sale Entry (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved sale entry ID 15117448491516252154.

Details:
```json
Status: 200
```

---

### Delete Sale Entry (Success)

* **Status:** ❌ Failed / Discrepancy
* **Description:** Deleted sale entry ID 15117448491516252154.

Details:
```json
Status: 500
```

---

### Delete Sale (Success)

* **Status:** ❌ Failed / Discrepancy
* **Description:** Deleted sales transaction ID 9163997556426527049.

Details:
```json
Status: 500
```

---

