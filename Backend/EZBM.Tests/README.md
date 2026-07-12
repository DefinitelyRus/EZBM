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

# Run the test project
dotnet run --project EZBM.Tests.csproj
```

### Running with Test Data Flags

By default, the tests reset and verify against the production `business_data.db`. To run them safely on a separate test database:

* **`-t` or `--use_test_data` / `-g` or `--generate_test_data`:** Sets the target database to `test_data.db`.
  ```bash
  dotnet run --project Backend/EZBM.Tests/EZBM.Tests.csproj -- --use_test_data
  ```

