# EZBM.Tests

This is a console application serving as the integration test suite for the EZBM REST API endpoints. It validates endpoint handlers by resetting the database, invoking controller methods with mock payloads directly, and writing test reports.

---

## Features

* **Database Reset**: Invokes `DbManager.Reset()` before executing test groups to ensure a clean state.
* **Controller Isolation Tests**: Tests `AuthController`, `StaffController`, `InventoryController`, and `SalesController` endpoints.
* **New Features Integration Tests**: Tests `Mixed Payments` checkouts, `PromoCode` discount evaluation logic, lazy `User` permissions/expiry validations, and automated DbContext `ActionLog` auditing.
* **Outputs**: Generates a test summary report at `Backend/Results.md` documenting succeeding/failing endpoints and diagnostic logs.

---

## How to Run Tests

Execute the following commands to clear existing test logs and run the suite:

```bash
cd Backend/EZBM.Tests

# Remove old results log file if it exists
rm results.log

# Run the test project
dotnet run --project EZBM.Tests.csproj
```
