# EZBM Git Style Guidelines

This document explains our rules for using Git, writing commit messages, and saving changes in the EZBM project. Following these rules helps keep our project history clean and easy to follow.

## Commit Message Formatting

* Describe what your commit does using simple, direct English.
  * *Example:* `Add backend core`, `Create Utils.cs`, `Reorganize Utils helper class member layout`
* Always start your commit title with a capital letter.
* Do not end your commit title with a period.
* Keep your commit title short and sweet (under 72 characters).

## Grouping Changes (Commit Granularity)

* Do not bundle a huge pile of unrelated changes into one commit. Make small, logical commits instead.
* Keep database setup, models, and entity changes separate from API controllers or endpoints.
* Commit changes to shared helpers (like `Utils.cs` or `Log.cs`) in their own separate commits.
* Group API route setup (`Program.cs`) and controller files (`InventoryController.cs`) together.
* Put document updates (like `README.md` or test results) into their own commits rather than mixing them with code.
* Keep configuration file changes (like `.csproj` or `.gitignore`) isolated or grouped with the code they support.

## Branching & Merges

* All active development work on the `dev` branch.
* Regularly update the `dev` branch with the `main` branch to keep them in sync.
