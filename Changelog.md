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
