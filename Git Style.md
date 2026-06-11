# EZBM Git Style Guidelines

This document outlines the Git version control standards, commit message formats, and staging conventions for the EZBM project. Adhering to these standards ensures a clean, readable, and trackable commit history.

---

## 1. Commit Message Formatting

* **Direct Imperative / Descriptive Style:** Prefer plain English, direct statements describing what the commit does.
  * *Example:* `Add backend core`, `Create Utils.cs`, `Reorganize Utils helper class member layout`
* **Sentence-Case Capitalization:** Always start the commit title with a capitalized letter.
* **No Trailing Periods:** Do not put a period at the end of the commit subject line.
* **Subject Line Length:** Keep the commit title concise, ideally under 72 characters.

---

## 2. Grouping Changes (Commit Granularity)

* **Logical Progressive Checkpoints:** Avoid staging massive, unrelated sets of changes. Commit changes in logical, self-contained increments.
* **Separation of Concerns:**
  * **Core Logic & Models:** Group modifications to database context (`AppDbContext`), models, and entities separately from controllers or endpoints.
  * **Helper Libraries:** Keep changes to shared helpers (like `Utils.cs`, `Log.cs`) in their own commits.
  * **Host & API Controllers:** Group API routing configuration (`Program.cs`) and endpoint controllers (`InventoryController.cs`) together when staging.
  * **Documentation:** Group updates to project documentation (e.g., `README.md`, `results.log`, `Results.md`) in dedicated commits rather than mixing them with application logic.
* **Build Configuration:** Keep updates to project configurations (e.g., `.csproj`, `.gitignore`, `.gitattributes`) isolated or grouped specifically with the updates they support.

---

## 3. Branching & Merges

* **Development Branch (`dev`):** All active development and proto-tasks should be performed and committed on the `dev` branch.
* **Merging and Synchronization:** Frequently sync the development branch with the default branch (`main`) using clean fast-forward or standard merges.
