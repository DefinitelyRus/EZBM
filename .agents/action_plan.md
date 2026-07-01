# Action Plan: New Features Implementation

This document details the action plan, resolved design decisions, and suggestions for implementing the new features of the EZBM application.

---

## 1. Resolved Design Decisions

> [!NOTE]
> **1. Parent-Child Relationship for Mixed Payments:**
> All line items (`SaleEntry` and `ItemTransaction` stock deductions) are stored **only on the parent transaction** record. Child transactions are simple financial records representing the split payment methods (e.g., PHP 500 Cash, PHP 700 E-Wallet) and referencing the parent transaction.

> [!NOTE]
> **2. RFID Reader Hardware Connection:**
> The 13.56MHz RFID scanner is connected via USB and operates as a Human Interface Device (HID) emulating keyboard input. The POS and server client will handle card scanning inputs as rapid keystrokes followed by an `Enter` character.

> [!NOTE]
> **3. Expiry and Access Rule Validation:**
> Expiry validation is evaluated **on-request** (lazy checking when the user scans their card or attempts an action) instead of using background timers or active polling services. If `DateTime.UtcNow > ExpirationDate`, the active permissions are overridden by the `PermissionsAfterExpiry` list.

> [!NOTE]
> **4. Hardware Mocking for Prototype Testing:**
> Hardware integrations (Cash register drawer kick and RFID reader inputs) will utilize interface abstractions (`ICashRegisterService`, etc.) whose default prototype implementations log actions directly to the console for testing.

> [!NOTE]
> **5. Authentication Lock Wall:**
> Access to all pages other than `/Login` must be strictly restricted. If a user tries to access any page without a valid active employee session, they must be redirected automatically to the login page.

> [!NOTE]
> **6. Written Receipt Blocking Threshold:**
> If a checkout transaction total exceeds the `WrittenReceiptThreshold` configured in store settings, the system will block checkout completion until the operator confirms a receipt has been issued and enters a validation token.

> [!NOTE]
> **7. Modular Promo Codes Engine:**
> Promo codes will be stored dynamically in the database (via a new `PromoCode` entity supporting variables like code text, discount value, active state, expiration dates) rather than hardcoded in the codebase. Managers can manage these promotions via a dedicated `/Promos` page.

> [!NOTE]
> **8. Dedicated Customer Management:**
> A new tab and CRUD views will support adding, updating, and viewing `Customer` profiles, active card expiration dates, permission sets, and their historical transaction ledgers.

> [!NOTE]
> **9. Global Scanner Focus Routing & USB Identification:**
> Scanner inputs (RFID and barcodes) must populate specific hidden fields or cart additions even when the keyboard focus is in a normal text area. Settings will store the exact USB Vendor ID / Product ID of the scanners to distinguish reader inputs from human typing.

---

## 2. Step-by-Step Task Checklist (Completed)

### Phase 1: Database & Entity Refactoring (User Reparenting)

- [x] Create `User.cs` base class inheriting from `Entity`. Add columns for `AccessType`, `RfidCardId`, `Permissions`, `PermissionsAfterExpiry`, `ExpirationDate`.
- [x] Refactor `Staff.cs` to inherit from `User.cs`. Modify constructor and DB configuration.
- [x] Create `Customer.cs` inheriting from `User.cs`. Add fields for `FirstName`, `LastName`, `PhoneNumber`, and `TransactionHistory`.
- [x] Configure TPH (Table-Per-Hierarchy) mapping for `User` hierarchy in `AppDbContext.cs`.
- [x] Add `Barcode` property to `Item.cs` entity and update EF configuration.

### Phase 2: Action Log & Audit Tracking

- [x] Create `ActionLog.cs` entity class to store logins, clock actions, register overrides, door logs, and edits/deletes.
- [x] Set up interceptors/filters or DB hooks to automatically log edit/delete actions of other entities.
- [x] Create API controller `/api/logs` and front-end interface page in Razor Pages (`Logs.cshtml`) to view Action Logs.

### Phase 3: Transaction Decoupling & Mixed Payments

- [x] Create junction/relation for `Transaction` and `ItemTransaction` (many-to-many link) to support resupply installments.
- [x] Modify `Transaction.cs` to add `ParentTransactionId` nullable foreign key.
- [x] Update POS checkout backend to handle `Mixed` payment checkout payloads (creating parent + child payment records).
- [x] Build Razor Pages split payment UI modal and logic in `POS.cshtml` with dynamic balance remaining verification.

### Phase 4: Hardware Integration Services (Mocked)

- [x] Implement `ICashRegisterService` interface and its console-logging implementation `MockCashRegisterService`.
- [x] Add "Open Register" manual override button to `POS.cshtml` POS UI, prompting for an RFID keyboard-emulated card input.
- [x] Add support for reading barcode keyboard inputs (HID) globally in `POS.cshtml` to search and add items to cart.
- [x] Create mock RFID scanning key listener on `POS.cshtml` layout to simulate card swipe inputs.

### Phase 5: Payroll Upgrade Commissions & Promos

- [x] Update `settings.json` to store promotional code definitions and membership upgrade commission rates.
- [x] Implement payroll calculation service upgrade commission checking (subtracting lower tier values already paid/earned).
- [x] Implement POS verification rule for `FREEWEEK` code (checks store opening date offset).
- [x] Render calculated net upgrade commissions inside Razor Pages payroll generation modals (`Logs.cshtml`).

### Phase 6: Settings Management (Back-end & Razor Pages)

- [x] Define structure for settings in `settings.json` (business name, currencies, thresholds, access card expiration preset rules).
- [x] Expose REST API endpoint `GET/POST /api/settings` to read/update configurations.
- [x] Bind active configurations (such as Business Name in header, written receipt requirement threshold) dynamically from `settings.json` within layout pages (`_Layout.cshtml`).
- [x] Create Settings management page in Razor Pages (`Settings.cshtml` + `Settings.cshtml.cs`) for system configuration.

### Phase 7: Dashboard Recent Transactions (Razor Pages)

- [x] Add a "Recent Transactions" log feed container to the Razor Pages dashboard (`Index.cshtml`), showing the latest 10 sales.

---

## 3. Step-by-Step Task Checklist: Future Implementation (React Rebuild)

### Phase 8: Strict Authentication Lock

- [ ] Add global middleware/filters to enforce employee login.
- [ ] Redirect all requests attempting to bypass `/Login` to the authentication screen.

### Phase 9: Modular Promotions Management

- [ ] Create `PromoCode` database entity structure with rate, expiration, and validation rules.
- [ ] Expose API endpoints for managing promotional codes.
- [ ] Create `/Promos` page to add, edit, and deactivate promotions.
- [ ] Update POS backend checkouts to evaluate dynamic promotion codes from the database.

### Phase 10: Customer Management Tab

- [ ] Create Customer Profile CRUD views under a new tab or page.
- [ ] Enable RFID card assignment, expiration updates, and permissions override configurations.
- [ ] Display transaction ledger histories inside customer detail panels.

### Phase 11: Written Receipt Threshold Enforcement

- [ ] Check transaction sum against `WrittenReceiptThreshold` on checkout initiation.
- [ ] If exceeded, show receipt issue validation popup blocking sale continuation.
- [ ] Add database field to record receipt token on the Sale transaction.

### Phase 12: Device-Specific Scanners Routing

- [ ] Add setting inputs to define Vendor ID (VID) and Product ID (PID) for both RFID and barcode scanners.
- [ ] Write scanner keystroke capture logic routing scan results to correct fields regardless of cursor focus.
