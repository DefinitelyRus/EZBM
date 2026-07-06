# Backend Refactoring Results

This file summarizes the refactoring changes executed on the EZBM C# backend.

## 1. Inventory & Inheritance Mapping (TPH)

* Made `Item` concrete. Created child classes `Product` and `Service` inheriting from `Item` under Table-Per-Hierarchy (TPH) mapping.
* Configured SQLite mapping in `AppDbContext` using an `ItemType` discriminator column.
* Added `TargetStock` and `LowStockThresholdPercentage` fields to `Product`.
* Updated `SalesService.CreateSaleAsync` to only decrement stock if the purchased item is a physical `Product`.
* Updated the startup `DataSeeder` to initialize and populate physical products using `Product` and membership upgrades using `Service`.

## 2. Password Hashing & Onboarding Setup

* Replaced plain-text password storage with secure PBKDF2/SHA-256 hashing inside `AuthenticationService`.
* Implemented password hashing verification inside the login controller.
* Implemented the onboarding setup API `POST /api/auth/setup` to initialize store name, currency, default roles (`Admin` and `Cashier`), and register the initial administrator profile if no users exist.

## 3. Dashboard Analytics API

* Implemented `GET /api/dashboard/analytics` computing total sales, gross/net profits, sales volume trends over the last 7 days, top popular products, low-stock warning items, and cashier leaderboards.

## 4. Payroll tracking, Commissions, and Adjustments

* Added `CommissionRate` override property to `Staff`.
* Created the `StaffAdjustment` database table tracking bonuses, advance pay, and cashier commissions.
* Implemented cashier commission logging in `SalesService` when cashiers process membership upgrades.
* Implemented payroll details calculation helper compute salaries, bonuses, and deduct advance pay values automatically based on period attendance hours.
* Auto-settled adjustments (marked `IsPaid` as `true`) when a payroll is committed.

## 5. Security Roles Matrix

* Created `Role` database table storing Discord-style permissions levels: `Allow` (1), `Default` (0), `Deny` (-1).
* Configured many-to-many user roles in the database.
* Updated permission verification in `User.HasPermission(action)` so that any explicit `Deny` in any assigned role overrides all `Allow` values.

## 6. Storage Paths & Backup Syncing

* Re-routed file storage inside `SettingsService`:
  * Admin settings: `<documents>/ezbm/admin-settings`
  * User settings: `<documents>/ezbm/user-settings/<user-id>-settings.json`
* Created simulated Google Drive backup sync endpoints:
  * `POST /api/sync/backup`: Save db backup file.
  * `POST /api/sync/restore`: Restore database from a backup file.

## 7. Pagination, Sorting & CORS

* Added `Limit`, `Offset`, `SortBy`, and `SortOrder` parameters to all find request records.
* Created LINQ helper `ApplySortingAndPagination` in `Utils` to support server-side grid search.
* Configured CORS in `Program.cs` to allow requests from `http://localhost:5173`.

## 8. Desktop Client Razor Pages UI/UX Updates

* Added a collapsible sidebar menu and media queries supporting layout sizing shifts.
* Unified all interactive action options, tables, input fields, and tab components with styled, modern CSS.
* Integrated dynamic store currency symbol and user-customized date/time string selectors.
* Added client-side generic pagination, sorting, and toggleable column hide/show visibility helpers.
* Consolidated inventory item quantity metrics columns and highlighted low stock items.
* Re-routed page-wide headers to inline attendance, payroll, and adjustment document tabs.
* Added supervisor active shift manually-triggered clock-out actions and cash reconciliation dialog layouts.
* Created a friendly audit log action explainer parsing serialized namespace tags.
* Swapped security role text boxes with a role-scoped Allow/Inherit/Deny permissions radio matrix.
* Built POS checkout terminal scanner features: automated focus, Web Audio API synthesizers, quantity modifiers, and manual barcode lookup entries.

