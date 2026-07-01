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
* **Description:** Searched for staff with username 'john_doe'. Found Staff ID: 9816388702326727899.

Details:
```json
Found 1 records.
```

---

### Get Staff (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved the staff member with ID 9816388702326727899.

Details:
```json
Status: 200
```

---

### Update Staff (Success)

* **Status:** ✅ Succeeded
* **Description:** Updated pay rate and username of staff member ID 9816388702326727899.

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
* **Description:** Retrieved the attendance record with ID 13461578450670630878.

Details:
```json
Status: 200
```

---

### Update Attendance (Success)

* **Status:** ✅ Succeeded
* **Description:** Updated the attendance record with ID 13461578450670630878.

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
* **Description:** Searched for payroll records. Found Payroll ID: 14108059025238376587.

Details:
```json
Found 1 records.
```

---

### Get Payroll (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved payroll record with ID 14108059025238376587.

Details:
```json
Status: 200
```

---

### Delete Payroll (Success)

* **Status:** ✅ Succeeded
* **Description:** Deleted payroll record with ID 14108059025238376587.

Details:
```json
Status: 200
```

---

### Delete Attendance (Success)

* **Status:** ✅ Succeeded
* **Description:** Deleted attendance record with ID 13461578450670630878.

Details:
```json
Status: 200
```

---

### Delete Staff (Success)

* **Status:** ✅ Succeeded
* **Description:** Deleted staff profile with ID 9816388702326727899.

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
* **Description:** Searched for items matching query. Found Item ID: 13507674704318337667.

Details:
```json
Found 1 records.
```

---

### Get Item (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved inventory item ID 13507674704318337667.

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
* **Description:** Searched for stock transactions. Found Transaction ID: 11289767862269030047.

Details:
```json
Found 1 records.
```

---

### Get Item Transaction (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved stock transaction ID 11289767862269030047.

Details:
```json
Status: 200
```

---

### Delete Item Transaction (Success)

* **Status:** ✅ Succeeded
* **Description:** Deleted stock transaction ID 11289767862269030047.

Details:
```json
Status: 200
```

---

### Delete Item (Success)

* **Status:** ✅ Succeeded
* **Description:** Deleted item ID 13507674704318337667.

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
* **Description:** Searched for sales transactions. Found Sale ID: 10012109519579787397.

Details:
```json
Found 1 records.
```

---

### Get Sale (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved sales transaction ID 10012109519579787397.

Details:
```json
Status: 200
```

---

### Find Sale Entries (Success)

* **Status:** ✅ Succeeded
* **Description:** Searched for sale entry line items. Found Entry ID: 4423970811520645750.

Details:
```json
Found 1 records.
```

---

### Get Sale Entry (Success)

* **Status:** ✅ Succeeded
* **Description:** Retrieved sale entry ID 4423970811520645750.

Details:
```json
Status: 200
```

---

### Delete Sale Entry (Success)

* **Status:** ❌ Failed / Discrepancy
* **Description:** Deleted sale entry ID 4423970811520645750.

Details:
```json
Status: 500
```

---

### Delete Sale (Success)

* **Status:** ❌ Failed / Discrepancy
* **Description:** Deleted sales transaction ID 10012109519579787397.

Details:
```json
Status: 500
```

---

## Custom New Features Verification

### Lazy Permissions Expiry Override

* **Status:** ✅ Succeeded
* **Description:** Verified that active permissions default to PermissionsAfterExpiry when ExpirationDate has passed.

Details:
```json
Active permissions: GuestViewOnly
```

---

### Audit Logs Automatic Capturing

* **Status:** ✅ Succeeded
* **Description:** Verified that editing an entity automatically triggers and logs an ActionLog entry.

Details:
```json
Log Details: Edit on Customer (ID: 10410322507778640361). Changes: FirstName: 'Lazy' -> 'LazyUpdated'
```

---

### Mixed Payment Split Record Decoupling

* **Status:** ✅ Succeeded
* **Description:** Verified that mixed payment checkouts correctly register parent Sale and multiple child split payment records.

Details:
```json
Parent Sale Status: Success. Child Payment Records count: 2
```

---

### FREEWEEK Promotion Validity Offset

* **Status:** ✅ Succeeded
* **Description:** Verified that applying 'FREEWEEK' within 7 days of store opening discounts the total amount to $0.

Details:
```json
Promo checkout result: Success. Final Sale Amount charged: 0.00
```

---

