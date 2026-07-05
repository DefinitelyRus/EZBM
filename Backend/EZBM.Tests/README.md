# EZBM.Tests

This is a console application serving as the integration test suite for the EZBM REST API endpoints. It validates endpoint handlers by resetting the database, invoking controller methods with mock payloads directly, and writing test reports.

## Features

* Clears and resets the database before test runs to ensure a clean start.
* Tests individual endpoint controllers like `Auth`, `Staff`, `Inventory`, and `Sales` in isolation.
* Runs integration tests for newer features like split payments, promo code discounts, and automated audit logging.
* Writes a detailed test results summary report to the `Backend/` folder.

## How to Run Tests

Execute the following commands to clear existing test logs and run the suite:

```bash
cd Backend/EZBM.Tests

# Remove old results log file if it exists
rm results.log

# Run the test project
dotnet run --project EZBM.Tests.csproj
```
