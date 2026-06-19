# EZBM Changelog

> *Author(s): DefinitelyRus*

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

---

&nbsp;

**You have reached the end of the changelog.**

&nbsp;

---
