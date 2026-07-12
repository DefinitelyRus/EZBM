# Frontend Refactoring Results

This file summarizes the refactoring changes executed on the EZBM Razor Pages frontend.

## 13 July 2026 - Cycle 4 (Command Line Args, Isolate Test Database, Randomized Seeder)

* **Command Line Arguments Support:**
  * Added `--use_test_data` / `-t` and `--generate_test_data` / `-g` startup arguments to all executable projects.
  * Supported dynamic DB filename switching in `DbManager` to redirect database operations to a separate `test_data.db` file, fully isolating testing environments.
* **Redesigned Seeder Engine:**
  * Moved the `DataSeeder` implementation from `EZBM.DesktopClient` to `EZBM.Core` to make database seeding functionality globally accessible.
  * Rewrote the seeding logic to generate a fully randomized, realistic dataset with erratic attributes, including 5-10 staff, 15-30 customers, and over 100 inventory products/services.
  * Added random omission of optional attributes (such as email addresses, telephone numbers, and item brands) to verify system resilience.
  * Configured attendance logs, periodic payroll history, staff adjustments (bonuses, advances, deductions), and invoice sales (single, split, and mixed payments) to be generated on the fly.
* **Documentation Updates:**
  * Updated README files in `Backend/`, `EZBM.Core`, `EZBM.DesktopHost`, `EZBM.DesktopClient`, and `EZBM.Tests` with detailed instructions on how to use the startup argument flags.

## 12 July 2026 - Cycle 3 (Date Caching, DB Cascades, Seeder Optimization, Wizard CSS Refactoring, Cart Pagination Fix, Mobile Collapsible Cart)


* **Performance & DB Safety:**
  * Cached the user settings preferred date format on `HttpContext.Items` to eliminate redundant synchronous file I/O operations.
  * Configured delete behavior cascades on DbContext relationships (Sales -> SaleEntries -> ItemTransactions) to enable deleting sales and entries cleanly.
  * Modified `DataSeeder.Seed` to conditionally run only if no staff entries exist, avoiding data loss on app restart.
  * Associated payroll settlements with staff adjustments, ensuring deleted payrolls reset adjustment payment statuses.
* **Layout & Navigation:**
  * Shifted main layout content container margins/paddings when the left sidebar is extended or collapsed on 1600px+ monitors.
  * Rerouted navigation collapse buttons into the sidebar links list and aligned the closed sidebar icons.
  * Set flex-column resets on layout wrapper to clamp footer elements in portrait mode.
  * Restructured mobile cashier checkouts right sidebar to be a collapsible drawer sliding in from the right.
* **Form Wizard Component:**
  * Refactored `FormWizardComponent` to toggle visible steps via CSS `display: none/block` instead of DOM detaching, preserving inputs and listeners.
  * Added configuration options to hide mode toggle controls and default layouts to standard form.
* **Inventory & Cashier Improvements:**
  * Moved inventory stock limits into the wizard step list.
  * Reordered inventory creation to request unit of measurement before stock fields, skipping stock-related steps for "Unlimited" unit types.
  * Resolved invisible cart item bug in POS cashier by setting `data-no-common-table="true"` on the dynamic shopping cart list.
  * Cleaned up colons from statements/question labels in inventory and POS screens.
* **Logs Page & API Route Consolidation:**
  * Merged the "Payroll" and "Staff Payments" subtabs under a unified "Compensation & Payments" tab with a dual ledger layout.
  * Replaced separate creation forms with a single unified wizard where selecting the payout category dynamically adjusts form fields (Regular Payroll vs. Bonus/Advance/Deduction).
  * Optimized Logs page model by conditionally querying SQLite database tables based on the active tab (e.g. only querying Attendance logs for the attendance tab).
  * Created a generic C# extension method `MapCrud` on `IEndpointRouteBuilder` to consolidate API route bindings in `Program.cs`, reducing routing boilerplate by over 30 lines.
  * Cleaned up colons and labels on Logs page and other modal wizards.
  * Fixed and resolved Razor compilation errors from unescaped media queries in `_Layout.cshtml` (`@media` to `@@media`) and Javascript backticks inside C# code in `Logs.cshtml`.
  * Verified that the entire test suite compiles and runs successfully, outputting clean, validated test reports.

## 12 July 2026 - Subsystem Inspection & Refactoring Proposals

During code inspection of the backend services, database structures, and Razor Pages, several architectural refactoring and optimization opportunities were identified.

### 1. Performance & I/O Optimization

* **User Settings File-Access Caching:**
  In [StateHelper.cs](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopClient/Helpers/StateHelper.cs), the `FormatDate` function runs synchronously but calls `GetActiveStaffAsync(httpContext).GetAwaiter().GetResult()`, blocking the thread pool. Furthermore, it calls `SettingsService.LoadUserSettings(activeStaff.Id)` on every execution, reading settings files from disk synchronously via `File.ReadAllText`. For views rendering lists of records (e.g. Logs page), this results in hundreds of synchronous disk reads per page request.
  *Proposal:* Cache the user preferences on `HttpContext.Items` (similar to how the active staff profile is cached) or in a memory cache.
* **OnGet Data Queries on Logs Page:**
  In [Logs.cshtml.cs](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopClient/Pages/Logs.cshtml.cs), `OnGetAsync` queries all log tables (`Attendance`, `Payroll`, `StaffAdjustment`, `Staff`, `ActionLog`, and `Transaction`) unconditionally on every request.
  *Proposal:* Conditionally query only the specific database tables corresponding to the current `ActiveTab`.

### 2. Subsystem Simplification & Safety

* **Database Wiping on Startup:**
  In [Program.cs](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopClient/Program.cs), the call to `DataSeeder.Seed()` executes unconditionally on startup. Because `Seed()` calls `context.Database.EnsureDeleted()`, any user/production data is deleted and reset to mock data on every launch.
  *Proposal:* Update `DataSeeder.Seed()` to only seed if the database does not exist or has no staff entries, and restrict `EnsureDeleted()` behind developer/testing flags.
* **Form Wizard JavaScript DOM Shuffling:**
  The `FormWizardComponent` script in [_Layout.cshtml](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopClient/Pages/Shared/_Layout.cshtml) dynamically manipulates the DOM by detaching elements from the form and moving them inside a temporary container, which is brittle and disrupts layout specificity/accessibility tab order.
  *Proposal:* Retain all form elements in their original positions and manage wizard step visibility using simple CSS styles (e.g., toggling `display: none` / `display: block`).

### 3. Consolidation & Genericization

* **Authorized Page Base Class:**
  Every page model (e.g., [Staff.cshtml.cs](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopClient/Pages/Staff.cshtml.cs), [Settings.cshtml.cs](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopClient/Pages/Settings.cshtml.cs), [POS.cshtml.cs](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopClient/Pages/POS.cshtml.cs)) duplicates the same `OnPageHandlerExecutionAsync` check to ensure the staff member is logged in and redirect to `/Login`.
  *Proposal:* Create a base class `AuthorizedPageModelBase : PageModel` to run this validation, then inherit from it in pages requiring staff authentication.
* **EF Core Foreign Key Delete Cascades:**
  In [AppDbContext.cs](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.Core/Data/AppDbContext.cs), relations between transactions, sales, sale entries, and item transactions don't define cascade rules. This causes backend endpoints like `DeleteSale` and `DeleteSaleEntry` to throw 500 errors if child records exist.
  *Proposal:* Configure cascade deletes on these foreign keys in the database configuration.
* **Generic API Controller Mappings:**
  In [Program.cs](file:///c:/Users/Rus/ALPHA/Projects/Software/EZBM/Backend/EZBM.DesktopHost/Program.cs), dozens of endpoints are mapped manually (e.g. `/api/items/create`, `/api/staff/create`, `/api/customers/create`).
  *Proposal:* Create generic router endpoints or middleware to automatically bind services returning `RequestResult<T>` to HTTP endpoints.

---

## 12 July 2026 - Cycle 2 (Full-height Sidebar, POS ID Bugfixes, Staff Roles & Modals, settings Refactoring)

* **General Layout:** Extended the left navigation sidebar to full page height. Configured the layout container to prevent page content shift when the sidebar is extended on desktop (1600px+). Changed the shopping cart styling to flow statically on smaller screen sizes.
* **POS 64-bit ID Bug Fix:** Wrapped C# `ulong` product IDs in single quotes within JS arrays and templates, and updated the C# JSON deserializer options to allow reading number values from strings. This resolves checking out errors and blank cart displays caused by Javascript's double-precision limit.
* **Staff & Roles:**
  * Standardized the access levels terminology to "Roles" across C# records, Razor pages, and wizards.
  * Relocated custom role creation from a sidebar form to a dynamic "+ Create New Role" prompt button above the role list.
  * Fixed checked roles toggle switches CSS specificity to correctly highlight switches in blue when active.
  * Rendered pay rates naturally (e.g. `₱15.50 every hour`, `₱500.00 every week`).
* **Commission Removal:** Completely removed all commission-related features from database entities (`Staff.cs`), settings, API request objects (`ServiceRequests.cs`), checkout processing (`SalesService.cs`), and management forms. This feature has been archived to be reintroduced later.
* **Log Records & Modal Forms:**
  * Renamed tab to "Log Records" and renamed all subtabs ("Attendance", "Transaction", "Payroll", "Staff Payments", "Actions").
  * Moved Attendance, Payroll, and Staff Payments forms out of the sidebars and into pop-up overlay modals.
  * Added clean "+ Add" buttons to the far right of the subtab header labels to toggle modal displays.
* **Settings:**
  * Standardized settings label text using user-friendly titles.
  * Removed default fallback hardware devices (e.g. "Default Keyboard HID"), defaulting exclusively to "None".
  * Configured VID/PID parameters to display after the device name in dropdown selects.
  * Renamed hardware scanning trigger button to "Select New Device".
  * Rounded low stock threshold warnings to 2 decimal places in index, inventory, and settings.

### Payroll & Staff Payments Integration Proposal

* **Backend Model:** Currently, `StaffAdjustment` tracks one-off compensation items (bonuses, advances, deductions), while `PayrollLog` represents periodic payouts. We suggest combining these under a unified `CompensationLedger` entity with a `PaymentCategory` enum discriminator (`RegularPayroll`, `Bonus`, `CashAdvance`, `Deduction`). This simplifies query logic and database table counts. Alternatively, establish a foreign key collection linking settled `StaffAdjustment` entries to their parent `PayrollLog` invoice to record adjustments inside the payroll history cleanly.

* **Frontend UI:** Merge the "Payroll" and "Staff Payments" subtabs under a unified "Compensation & Payments" tab. Replacing separate creation forms with a single unified wizard where selecting the payout category dynamically adjusts form fields (e.g., prompting for period range and hourly calculation only for "Regular Payroll", while asking for a direct amount for "Advance Pay" or "Bonus").

---

## 12 July 2026 - Cycle 1

* **Modal Popups:** Converted the Add/Edit Staff and Edit Item sidebar forms into overlay modal popups (`addStaffModal`, `editStaffModal`, `editItemModal`).
* **Edit Form Wizards:** Integrated the client-side `FormWizardComponent` for both employee editing and item editing.
* **Wizard Element Preservation:** Fixed the wizard step-clearing and raw form toggling to safely append inputs back to the parent form, preventing DOM destruction and preserving input values and event hooks.
* **Granular Validation:** Updated the wizard validation to respect the HTML `required` attribute and dynamically toggle button labels between "Skip" and "Next".
* **Label De-emphasis:** De-emphasized secondary field labels in wizard mode by rendering them in a lighter gray, normal weight style.
* **Login Autofill & Layout:** Moved the login button directly below the RFID input field. Added standard `autocomplete="username"` and `autocomplete="current-password"` attributes to allow password manager auto-fill.
* **Unit Abbreviations:** Configured quantity display fields to use short unit abbreviations (e.g. `kg`, `L`, `mg`, `oz`) and rendered detailed descriptions (e.g. `Kilogram (kg)`) inside add/edit modals.
* **POS Checkout & Hover Buttons:** In Cashier view, hid the "Add" button unless hovering over the catalog row. Replaced inline string-passing in "Add" button with robust ID-based JS lookup.
* **POS Mobile Ordering:** Configured CSS flexbox ordering to position the shopping cart above the catalog list on screen widths below 1600px.
* **USB Peripheral Selectors:** Transformed peripheral device text inputs in Settings into dropdown select lists populated with connected HID/USB devices, showing a browser support error if WebHID/WebUSB is not supported.
* **Roles Toggle Color:** Standardized the checked Roles matrix switches to use the primary blue color (`#3182ce`) when turned ON.

---

## 1. General Formatting & UI Adjustments

* **Decimal formatting:** Created `Utils.FormatDecimal` to clean up numbers by hiding decimal places if they round to `.00` or an integer, otherwise rounding to 2 decimal places. Applied this cleanly to currency and quantities.
* **Friendly Names:** Updated UI components and tables to display friendly readable names instead of internal code constants (e.g. displaying "Apply Discounts" instead of "ApplyDiscounts").
* **Settings:** Created a default `settings.json` file inside `EZBM.DesktopHost` housing all standard user preferences and store/admin configurations.

## 2. Login Page Enhancements

* **Clean UI:** Hidden the sidebar menu button and the "Not Logged in" header indicator when on the login screen to prevent unauthorized actions.
* **Remove Test Section:** Excised the "Database Users for Test" section from the login page to prepare the app for production.
* **Clock-in Modal:** Created a pop-up confirmation modal upon successful login prompting users to clock-in. Added options to "Do not show again today" and "Never show again".

## 3. Dashboard Changes

* **Recent Transactions:** Updated the dashboard transaction feed to show list of item names purchased in each transaction, moving the technical transaction ID string to small subtitle text at the bottom.

## 4. Inventory Page Upgrades

* **Naming:** Renamed all pages, sidebar links, and page headers from "Inventory Management" to "Inventory".
* **Brand & Images:** Integrated "brand" and "image" fields into the database entities and displayed them formatted nicely inside the catalog table.
* **Reusable Wizard Component:** Built a client-side generic `FormWizardComponent` class managing multi-step creation and edit operations with "Next", "Skip", "Finish" buttons, progress indicators, dynamic validation, a summary page, and a quick switch between wizard and raw form layouts.
* **Text Tag Bubbles:** Replaced the legacy tag enum system with a string list database mapping. Implemented text-field-dropdown selection on the item wizard and forms, with chosen tags appearing as distinct bubbles with individual remove buttons.
* **Required Field Indicators:** Added a red asterisk next to the header labels of all required inputs on forms and wizards.
* **Quantity Formatting:** Stripped the redundant "Count" unit suffix from quantity displays (showing "118/200" instead of "118 Count / 200 Count"). Fixed the display of "9999 Unlimited" to render cleanly as "Unlimited".

## 5. POS Rename

* **Cashier Page:** Renamed all references to "POS Checkout Terminal" to "Cashier" for clarity.

## 6. Staff Management & Access matrix

* **Menu Restructuring:** Renamed the sidebar tab to "Staff Management", and updated sub-tabs to "Staff" and "Roles".
* **Entry Wizards:** Registered the reusable wizard component for employee registration/edit, attendance clocking, and payroll logging.
* **RFID Cards:** Exposed an RFID Card ID text input on staff create and edit screens.
* **Preset Roles:** Seeded default roles ("Admin", "Manager", "Cashier", "Logistics") on system startup, matching seeded employees to their active roles.
* **Roles Matrix Redesign:** Redesigned the roles permission settings tab into a clean 2-column list of permissions with toggle switches. Paired read/write operations (e.g. `ViewInventory` and `ModifyInventory`) are condensed into a single row ("Access Inventory") with individual sliding toggle switches for "View" and "Edit" permissions.
* **Role Actions:** Displayed the selected role as the section header next to a "Delete Role" button. Added a shortcut button in the staff registration form to instantly create a new custom role named after the staff member's username. Persisted selected role state in `localStorage` across page reloads.

## 7. Peripheral Device Settings & Scan Interception

* **Formatting Examples:** Configured date/time preferences options in Settings to show static Dec. 31, 1999 examples.
* **HID & USB API Integration:** Replaced text input boxes for hardware devices with interactive "Select Device" buttons leveraging the WebHID API (for barcode and RFID readers) and WebUSB API (for the cash drawer trigger relay). Added dynamic connection status indicators and compatibility help texts.
* **Scan Interception:** Implemented a global scanner wedge interception script tracking input speed to intercept physical barcode/RFID scanner inputs and route them automatically to relevant page elements (e.g. manual barcode entry, staff card ID mapping, login screens, search tables).
* **Discount Configuration:** Updated promotional discount inputs to accept percentage values directly (e.g. entering "20" means "20% off" stored as 0.8) and display them in the tables.
