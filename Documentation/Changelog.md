# EZBM Changelog

A chronological record of all changes, updates, and additions made to the Easy Business Manager (EZBM) project.

## Guidelines

- The logs must be sorted reverse-chronologically (nearest-first).
- Descriptions and changes must be written in plain English (minimize techno-jargon).
- Don't be too concise but don't be too verbose either.
- Changelogs are not always 1:1 with Git changes.
- Use the format template.
- *TBD*

## Format Template

```markdown
### 12/31/2099

An optional short description of the changes made on this day should be written in place of this text. It should be no more than 1-3 sentences long. The description may be skipped if the changes are minimal and self-explanatory.

Changes:

- <Change 1>
- <Change 2>
- [...]
```

## Logs

### 07/12/2026

Implemented comprehensive performance, database safety, UI layout, and backend refactoring updates. This includes caching user settings, configuring database delete cascades, optimizing seeder execution, rewriting the step wizard element lifecycle using CSS, merging logs subtabs and forms into a single unified Compensation tab/wizard, dynamically querying logs tables, and consolidating API controllers using generic routing extensions.

Changes:

- Cached user settings date format string on HttpContext items to prevent blocking synchronous file system queries.
- Configured cascading deletion rules on EF Core context mappings (Sales -> SaleEntries -> ItemTransactions).
- Modified the data seeder utility to populate mockup records only if the Staff table is completely empty.
- Tied payroll records deletion to staff adjustments, resetting the adjustment paid state when a payroll entry is deleted.
- Adjusted main dashboard sidebars and layout containers to scale and transition smoothly on wide screens without page content shifting.
- Re-styled the cashier mobile checkout sidebar to act as a collapsible drawer sliding in from the right.
- Refactored FormWizardComponent to toggle wizard steps via CSS displays instead of DOM detaching, preserving input state and listeners.
- Added step wizard layout options to default to standard form layouts and suppress toggle controls.
- Reordered inventory item creation steps to ask for unit type first, skipping stock-related fields for "Unlimited" units.
- Fixed dynamic POS cashier shopping cart rendering issues by bypassing common table handlers.
- Merged Payroll and Staff Payments subtabs into a unified "Compensation & Payments" ledger tab with separate payroll history and adjustments tables.
- Provided a single modal dialog step wizard for compensation entries, dynamically showing relevant inputs based on category.
- Optimized Logs page model to conditionally query SQLite database tables corresponding only to the active tab.
- Created generic MapCrud extension method for Minimal APIs, consolidating Program.cs routes boilerplate.
- Cleaned up trailing colons from all statement, label, and wizard question headers.

### 07/11/2026

Implemented security roles permissions layout updates and default access configuration presets. Refactored the unit test suite to compile cleanly with the updated string-based item tags schema.

Changes:

- Redesigned the Roles permission matrix tab under Staff Management to use a flat 2-column table layout, combining paired View/Modify options into single "Access" rows with separate View and Edit toggle switches.
- Displayed the active role name directly in the permissions header and repositioned the "Delete Role" button to the far right.
- Seeded a default "Logistics" role and renamed the "Administrator" role to "Admin" in the data seeder.
- Fixed compilation errors in the endpoint tests project by updating legacy tag enum references to string tags.

### 07/10/2026

Implemented comprehensive backend and Razor Pages frontend updates including rectangular visual design overrides, fixed layout panels, collapsible sidebar pull-tabs, dropdown options for peripheral USB devices, global page authentication middleware, optional RFID login workflows, a modern scrolling feed analytics dashboard, unlimited stock support, modal-based inventory item creation, hierarchical staff access configuration checklists with indeterminate states, percentage-based commission overrides, and expanded date/time display configurations.

Changes:

- Reverted Razor Page styling to minimal CSS, forcing rectangular inputs, buttons, and panels via a global `* { border-radius: 0 !important; }` layout override.
- Locked page headers to fixed top screen positions and set sidebars (navigation and right workspaces) as sticky-scrolling panels.
- Configured collapsibility for the left and right sidebars on wide screens (>=1600px) and built clickable pull-tabs to restore hidden sidebars.
- Built a global auto-initializer on page load in `_Layout.cshtml` to setup common tables and short-circuit double initialization.
- Added dropdown lists populated with USB HID presets for the RFID Scanner and Cash Register Trigger configurations on the User Preferences settings screen.
- Implemented global page authentication logic via `OnPageHandlerExecutionAsync` in all core page models, redirecting unauthenticated users to the Login page.
- Added a store-wide `EnableRfidLogin` preference option in business settings and integrated optional RFID scanning credentials login directly on the Login page.
- Redesigned the home dashboard into a single-column scrolling feed with custom widgets for statistics, SVG sales trends, transaction entries with purchase lists, low stock alerts, popular products, and cashiers performance.
- Displayed stock levels of `-1` as "unlimited" and updated checkout services to skip decrementing unlimited stocks.
- Moved the inventory catalog item addition form into a popup modal triggered by a button next to the catalog search input.
- Refactored security roles matrix to be categorized with nested checkboxes supporting indeterminate state checkboxes in staff access.
- Divide and display commission override input rate as a percentage value instead of raw decimal multiplier.
- Added support for expanded date & time formats in personal user configurations.

### 07/06/2026

Implemented comprehensive Razor Pages UI updates including responsive design, shared button/link styling, universal currency/date formatting, interactive pagination/sorting, inventory columns consolidation, inline subtabs navigation, staff permissions radio matrix, and POS checkout term updates.

Changes:

- Added responsive design CSS rules to support sidebar collapsing and overlap mode on narrow screens in `_Layout.cshtml`.
- Unified all page navigation actions and inputs as styled buttons and inputs.
- Integrated dynamic store currency symbols and active user date formatting configurations across all dashboard pages.
- Built a reusable vanilla JavaScript client-side pagination, sorting, and column visibility toggle helper in `_Layout.cshtml` and initialized it on all main data tables.
- Consolidated catalog columns in `Inventory.cshtml` to combine current and target quantities, and highlighted low stock items in red.
- Converted page-wide navigation headers in `Logs.cshtml` into inline document subtabs for attendance, payroll, and adjustments.
- Added a supervisor clock-out button for active shifts, cash drawer reconciliation dialog details, and a drop-down entity filter in the logs page.
- Rewrote the audit logs details processor in `Logs.cshtml.cs` to translate C# serialized type names into friendly text logs.
- Merged employee name and position columns, hid detailed columns by default, and replaced the security roles text input with an Allow/Inherit/Deny permissions radio matrix in `Staff.cshtml`.
- Updated `POS.cshtml` to auto-focus the search bar, sound off Web Audio API synthesizer tones, add quick cart quantity +/- buttons, and accept manual barcode text inputs.

### 07/05/2026

Updated documentation formatting, ignored generated test files, created API reference guide, and implemented backend refactoring updates including item inheritance, password hashing, setup onboarding, dashboard analytics, payroll calculation helpers, roles permissions matrix, and backup sync.

Changes:

- Cleaned up all project markdown documentation files, removing section numbering, stripping formatting prefixes, and simplifying technical jargon.
- Removed emojis from headings and replaced box-drawing characters in the folder layout diagram with standard ASCII equivalents in `README.md`.
- Updated `.gitignore` to exclude local test results and logs from version tracking.
- Created `API Documentation.md` detailing all REST endpoints, hosting commands, API usage instructions, and plain JavaScript consumption examples.
- Updated `README.md` and `Backend/README.md` to link to the new API documentation.
- Fixed C# compiler errors caused by pattern matching (`is null` / `is not null`) inside EF Core expression trees in `StaffService.cs`, `InventoryService.cs`, `Logs.cshtml.cs`, and `StateHelper.cs`.
- Fixed missing `using EZBM.Core.Entities;` directive in `Program.cs` of the desktop client and incorrect `Id` property reference in `StaffService.cs`.
- Refactored `Item` class with Table-Per-Hierarchy (TPH) inheritance mapping for `Product` and `Service` subclasses in `AppDbContext`.
- Implemented secure PBKDF2 with SHA-256 password hashing and validation in `AuthenticationService` and `AuthController`.
- Added `POST /api/auth/setup` onboarding setup API setting up storefront config, seeding default roles (`Admin`, `Cashier`), and registering the administrator profile.
- Created `GET /api/dashboard/analytics` exposing daily sales/profits metrics, weekly volume charts, top popular products, cashier performance leaderboards, and low-stock item warnings.
- Added custom staff commissions logging when cashiers checkout membership upgrades.
- Created `GET /api/payroll/calculate` payroll endpoint computing working hours, gross/net earnings, and auto-deducting advances.
- Implemented many-to-many security role matrix resolution in `User.HasPermission` where explicit `Deny` overrides all `Allow` actions.
- Re-routed settings configurations to directory-based paths under `<documents>/ezbm/` and implemented Google Drive simulated database backup and restore endpoints.
- Added server-side pagination (`limit`/`offset`) and sorting (`sortBy`/`sortOrder`) support in all core find requests.
- Modified the inventory catalog table and add/edit forms in `Inventory.cshtml` and `Inventory.cshtml.cs` to distinguish products and services, capture target stock/low stock threshold percentage inputs, and persist them via request services.
- Added client-side real-time catalog search filtering by ID, name, description, and tags on the POS checkout catalog and inventory screens.
- Updated user profile forms and database controllers to support custom employee commission rates and logged staff adjustments.
- Designed a modern, fully-featured home screen analytics dashboard in `Index.cshtml` and `Index.cshtml.cs` loading gross profit, net profit, recent transactions, popular products, weekly trends, and employee sales leaderboards.
- Created a first-time system onboarding configuration page `Setup.cshtml` redirecting fresh instances when no active staff members exist in the database.
- Split settings views in `Settings.cshtml` and `Settings.cshtml.cs` into individual User Preferences and Business & Policies tabs, supporting theme choices, date formats, password resets, and manual database backup/restore sync triggers.
- Integrated a generic client-side table sorting and pagination mechanism in vanilla JavaScript across Razor Page data lists.
- Added an adjustments ledger tab in `Logs.cshtml` and `Logs.cshtml.cs` showing advance pay and custom bonuses, and wired them to automatic net pay calculation modifiers.

### 07/03/2026

Implemented design and layout upgrades for the frontend React prototype.

Changes:

- Reorganized layout and styles for attendance logs, staff directory, checkout, and inventory screens.
- Added a collapsible sidebar navigation component for easier screen toggling.
- Added settings icon assets (`settings.svg`) and configured sidebar routing for settings parameters.

### 07/02/2026

Refactored the entire backend codebase for code style compliance and updated documentation.

Changes:

- Cleaned up C# models, database context, services, helpers, API controllers, and test console configurations to adhere to style guidelines.
- Added real-time search functionality and improved screen styling on the React frontend.
- Updated project and directory `README.md` documents.
- Deleted obsolete checklist files (`Results.md`, `Actionable Tasks.md`, `Feature Checklist.md`) and action logs from repository storage.

### 07/01/2026

Enacted major backend and frontend feature enhancements including user TPH database reparenting, ambient operator context tracking, automatic change tracking audit logs, POS mixed payment split transactions, cash drawer hardware overrides, modular upgrade commissions calculation, and dynamic storefront settings management.

Changes:

- Created the base `User` entity to unify common profile, RFID, and lazy access permissions expiry validations across `Staff` and `Customer` records.
- Configured Table-Per-Hierarchy (TPH) mappings for the user hierarchy and configured permission list semi-colon conversion filters in `AppDbContext`.
- Implemented automatic DB audit logs intercepting and saving edit/delete actions for all tracked entities.
- Added `/api/logs` and integrated an **Action Audit Logs** tab in the client page workspace to display system access history.
- Introduced parent-child payment relationship tracking for `Mixed` checkout modes, mapping line items to the parent Sale and finance channels to child Transaction records.
- Built a Split Payment checkout modal prompting for payment method details when checking out via mixed methods.
- Implemented console-logged `ICashRegisterService` drawer controls, global barcode reader key listeners, and an RFID manual register override button.
- Added modular payroll upgrade commission calculations (subtracting lower tier values already paid/earned) with an auto-calculate action in the client form.
- Exposed REST API endpoints `/api/settings` and built a settings configuration editor page.
- Added a recent transactions activity feed side container to the dashboard.
- Updated the automated integration test suite to verify lazy permissions, audit logs, split payments, and promos.

### 06/12/2026

Built and integrated a lightweight ASP.NET Core Razor Pages testing client to serve as a desktop console for backend verification. Added a database seeder utility to populate all system entities with diverse mock records, reorganized code structures using region formatting, updated comprehensive project README files, and implemented validation rules to block negative inputs across the client forms (except for payroll modifiers).

Changes:

- Created the `EZBM.DesktopClient` web application project and registered it in the solution file.
- Designed the main application layout shell featuring employee cookie-based context tracking and a shift attendance toggle.
- Implemented view pages and page models for Login, Analytics Dashboard, Inventory, POS Checkout, Staff, and Logs.
- Added client-side real-time filtering for inventory catalog search and local cart logic in the POS workspace.
- Added a `DataSeeder` helper to automatically populate 10 wildly different entries for all entities when the database is empty.
- Reorganized members of all desktop client C# source files using region blocks to align with code layout guidelines.
- Created the project-specific README for the desktop client and updated root and backend-wide README specifications.
- Implemented numerical value validation constraints in both client-side views and server-side page models of the desktop client to disallow negative numbers (excluding payroll modifier adjustments).
- Updated the XML documentation for the CartItemDto checkout record to align with code documentation standards.

### 06/11/2026

Implemented backend API endpoints and supporting database services for staff authentication, inventory management, checkout sales, payroll, and attendance tracking. Unified request/response models and routing structures across the host application.

Changes:

- Created and exposed API controllers for Auth, Inventory, Sales, Attendance, Payroll, and Staff.
- Implemented asynchronous CRUD and search/filtering operations in the Core services.
- Added constructor definitions and unique ID generation for the `ItemTransaction` and `SaleEntry` domain entities.
- Registered endpoints, service dependencies, and configured OpenAPI/Swagger mapping in the WebApplication builder.
- Introduced `EndpointHelpers` to unify HTTP response formatting from request results.
- Added comprehensive XML documentation across all newly added endpoints and service methods.

### 06/04/2026

Cleaned up project instructions and updated repository configuration to exclude agent-specific configurations.

Changes:

- Removed agent instructions from the repository.
- Updated the `.gitignore` rules.
- Revised the list of actionable prototype micro-tasks in `Actionable Tasks.md`.

### 06/03/2026

Implemented backend database services for managing staff, inventory, and sales. Updated unique identifiers across all records, added helper functions for reading data, and cleaned up unused components.

Changes:

- Created database services to handle staff details, inventory items, and sales transactions.
- Updated unique IDs across all data entities.
- Added data conversion helper functions to prevent errors when loading data.
- Removed the obsolete staff management helper class.
- Switched to strict variable types across the C# projects to ensure safety.
- Added code documentation and checked off completed items on project lists.

### 06/02/2026

Refactored and enhanced the backend core data models, including setting up an SQLite database context and its respective mappings. Revised various entity relationships, refined utility functions, and added comprehensive XML documentation to C# source files.

Changes:

- Added the SQLite package to the backend core project.
- Created `AppDbContext` and configured entity mappings and `DbSets`.
- Refactored `Transaction`, `Sale`, and `Payroll` entities to improve structure and inheritance.
- Updated entity associations to reference object instances instead of primitive IDs.
- Implemented core database initialization and reset logic.
- Restructured code by renaming the storage folder to `Data` and updating ID generation methods.
- Refined models by removing redundant properties and adding new enumerations.
- Added and updated comprehensive in-code XML documentation for C# classes, records, and members.

### 06/01/2026

Established the foundational architecture for the backend core, introducing domain entities, utility functions, and logging mechanisms. Expanded in-code documentation.

Changes:

- Added the initial core domain entity models for the application backend.
- Created `Utils.cs` containing essential JSON processing and file I/O helpers.
- Implemented a centralized `Log` class for output formatting.
- Set up local storage architecture and created a `DbManager`.
- Enhanced in-line code documentation for models and core operations.

### 05/31/2026

Initiated the backend environment by setting up the core C# library and the desktop host application, establishing the primary server infrastructure.

Changes:

- Created the `EZBM.Core` backend library project.
- Initialized the `EZBM.DesktopHost` API project.
- Added a dedicated `README.md` for the Backend directory.

### 05/27/2026

Initial repository setup and documentation generation for the EZBM prototype phase.

Changes:

- Generated a detailed `Task List.md` outlining phase 1 development micro-tasks.
- Renamed the initial feature checklist from `Features.md` to `Feature Checklist.md`.
- Added ReactJS specific ignore rules to `.gitignore`.
- Drafted the core Product Requirements Document (`PRD.md`) and initial feature checklist.
- Populated `README.md` with the main project overview.
- Initialized the repository with standard git configuration files (`.gitignore` and `.gitattributes`).

&nbsp;

**You have reached the end of the changelog.**

&nbsp;
