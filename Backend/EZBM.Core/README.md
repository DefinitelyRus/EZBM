# EZBM.Core

This is the core business logic library for the EZBM project. It manages the database models, local SQLite database lifetime, and backend operations (calculations, validations, and data persistence).

## Structure

* The `Entities/` folder holds the database models for our project:
  * `User.cs`, `Staff.cs`, and `Customer.cs` handle profiles using single-table inheritance (`TPH`), with automated card expiry checks.
  * `Sale.cs`, `SaleEntry.cs`, `Transaction.cs`, and `ItemTransaction.cs` track cart checkouts, payments, and stock changes.
  * `Attendance.cs`, `Payroll.cs`, and `ActionLog.cs` manage hours worked, payouts, and database audit logs.
* The `Data/` folder configures the database connection and behavior:
  * `AppDbContext.cs` links our models to the SQLite database and logs modifications automatically.
  * `DbManager.cs` takes care of setting up, seeding sample data, and resetting the database.
* The `Services/` folder contains files that map core business logic:
  * `InventoryService.cs` manages item stocks and reorders.
  * `SalesService.cs` handles transactions and checkout splits.
  * `StaffService.cs` processes work logs and commission payouts.
  * `AuthenticationService.cs` checks user login credentials.
  * `SettingsService.cs` reads and writes app configurations.
  * `MockCashRegisterService.cs` emulates hardware connections like opening a cash drawer.
* The `Tools/` folder contains utilities like ID generators, logging functions, and invoice helpers.

## Local Database Save Location

The SQLite database `business_data.db` is configured to be saved inside the user's `My Documents` folder (or OneDrive-backed Documents folder):

* `C:\Users\Rus\Documents\business_data.db`

## How to Reference

To use the core library in other .NET projects (e.g. Host, Tests, CLI tools):

```bash
dotnet add <project-path>.csproj reference Backend/EZBM.Core/EZBM.Core.csproj
```

## Class and Record Details

Below is the complete list of all classes, records, and their properties in `EZBM.Core`.

### `Core.cs`

* `Core` (class): Empty class.

### `Entities`

* `Entity` (class in `Entities/Entity.cs`): Base class for all domain entities.
  * `Id` (`ulong`): Unique identifier.
  * `CreatedAt` (`DateTime`): Creation timestamp.
  * `UpdatedAt` (`DateTime`): Last modification timestamp.
* `User` (class in `Entities/User.cs`): Base user class. Inherits from `Entity`.
  * `FirstName` (`string?`): First name.
  * `LastName` (`string?`): Last name.
  * `PhoneNumber` (`string?`): Contact phone number.
  * `Email` (`string?`): Email address.
  * `AccessType` (`AccessCardType`): Type of access card.
  * `RfidCardId` (`string?`): Unique RFID card ID.
  * `Permissions` (`List<string>`): List of active permissions.
  * `PermissionsAfterExpiry` (`List<string>`): Fallback permissions after expiry.
  * `ExpirationDate` (`DateTime?`): Privilege expiration timestamp.
  * `Roles` (`List<Role>`): Assigned security roles.
* `Staff` (class in `Entities/Staff.cs`): Staff member details. Inherits from `User`.
  * `Username` (`string`): Username for login.
  * `Password` (`string?`): Account password.
  * `Position` (`string?`): Job title or role.
  * `PayFrequency` (`Frequency`): Frequency of payment.
  * `PayRate` (`float`): Monetary rate of pay.
  * `CommissionRate` (`float?`): Override commission rate.
* `Customer` (class in `Entities/Customer.cs`): Customer profile details. Inherits from `User`.
  * `TransactionHistory` (`ICollection<Transaction>`): History of customer transactions.
* `Item` (class in `Entities/Item.cs`): Base inventory item class. Inherits from `Entity`.
  * `Name` (`string`): Product display name.
  * `Description` (`string?`): Product description.
  * `ImageUrl` (`string?`): Image URL link.
  * `IsForSale` (`bool`): If available for purchase.
  * `Cost` (`float?`): Price paid to acquire item.
  * `SalePrice` (`float?`): Customer selling price.
  * `Quantity` (`float`): Available stock amount.
  * `UnitOfMeasurement` (`Unit`): Unit of measurement.
  * `ExpirationDate` (`DateTime?`): Expiration date.
  * `Tags` (`List<Tag>`): List of categories.
  * `Barcode` (`string?`): Product barcode.
* `Product` (class in `Entities/Product.cs`): Physical inventory product. Inherits from `Item`.
  * `TargetStock` (`float`): Ideal stock count.
  * `LowStockThresholdPercentage` (`float`): Threshold fraction for low-stock warnings (defaults to 0.20).
* `Service` (class in `Entities/Service.cs`): Service or digital upgrade. Inherits from `Item`.
* `Role` (class in `Entities/Role.cs`): System role with permissions. Inherits from `Entity`.
  * `Name` (`string`): Role display name.
  * `PermissionsJson` (`string`): Serialized JSON permissions dictionary mapping.
* `StaffAdjustment` (class in `Entities/StaffAdjustment.cs`): Payroll bonus or advance. Inherits from `Entity`.
  * `StaffId` (`ulong`): Target employee ID.
  * `AdjustmentType` (`string`): "Bonus", "Commission" or "AdvancePay".
  * `Amount` (`float`): Financial amount.
  * `DeductFromCurrentPayroll` (`bool`): If deducted from net pay.
  * `IsPaid` (`bool`): Set to true when settled.
  * `Timestamp` (`DateTime`): Transaction timestamp.
  * `Notes` (`string?`): Additional comments.
* `ItemTransaction` (class in `Entities/ItemTransaction.cs`): Record of stock movement. Inherits from `Entity`.
  * `TransactionType` (`Type`): Type of stock movement.
  * `Quantity` (`float`): Quantity moved.
  * `Staff` (`Staff`): Responsible staff member.
  * `Timestamp` (`DateTime`): Transaction time.
  * `Note` (`string?`): Additional details.
  * `Item` (`Item`): Linked inventory item.
  * `SaleEntry` (`SaleEntry?`): Optional related sale entry.
  * `Transactions` (`ICollection<Transaction>`): Associated transactions.
* `Transaction` (class in `Entities/Transaction.cs`): Financial transaction. Inherits from `Entity`.
  * `TransactionType` (`Type`): Income, expense, or correction.
  * `Amount` (`float`): Monetary amount.
  * `Timestamp` (`DateTime`): Transaction date and time.
  * `Staff` (`Staff`): Associated staff member.
  * `PaymentMethod` (`PayMethod?`): Method of payment.
  * `InvoiceNumber` (`int`): Invoice sequence number.
  * `InvoiceId` (`string?`): Generated invoice identifier string.
  * `Notes` (`string?`): Additional context notes.
  * `ParentTransactionId` (`ulong?`): Split parent transaction ID.
  * `ParentTransaction` (`Transaction?`): Split parent transaction entity.
  * `ChildTransactions` (`ICollection<Transaction>`): Child split transactions.
  * `Customer` (`Customer?`): Associated customer.
  * `ItemTransactions` (`ICollection<ItemTransaction>`): Linked stock movements.
* `Sale` (class in `Entities/Sale.cs`): Completed customer sale. Inherits from `Transaction`.
* `SaleEntry` (class in `Entities/SaleEntry.cs`): Line item in a sale. Inherits from `Entity`.
  * `Quantity` (`float`): Quantity purchased.
  * `UnitPrice` (`float`): Price per unit at purchase.
  * `Subtotal` (`float`): Total cost of the entry.
  * `Sale` (`Sale`): Parent sale.
  * `Item` (`Item`): Purchased item.
* `Attendance` (class in `Entities/Attendance.cs`): Staff attendance record. Inherits from `Entity`.
  * `Staff` (`Staff`): Associated staff member.
  * `TimeIn` (`DateTime`): Clock-in time.
  * `TimeOut` (`DateTime?`): Clock-out time.
* `Payroll` (class in `Entities/Payroll.cs`): Payroll earnings and periods. Inherits from `Transaction`.
  * `PeriodStart` (`DateTime`): Pay period start date.
  * `PeriodEnd` (`DateTime`): Pay period end date.
  * `TotalHours` (`float`): Total hours worked.
  * `GrossAmount` (`float`): Earnings before modifiers.
  * `Modifiers` (`float`): Bonus or deduction adjustments.
* `ActionLog` (class in `Entities/ActionLog.cs`): Operator or device action audit log. Inherits from `Entity`.
  * `ActionType` (`string`): Category of logged action.
  * `OperatorUsername` (`string`): Username of operator.
  * `Details` (`string`): Descriptive context details.
  * `Timestamp` (`DateTime`): Event time.

### `Data`

* `AppDbContext` (class in `Data/AppDbContext.cs`): Database context for SQLite. Inherits from `DbContext`.
  * `User` (`DbSet<User>`): Database set of users.
  * `Staff` (`DbSet<Staff>`): Database set of staff.
  * `Customer` (`DbSet<Customer>`): Database set of customers.
  * `ActionLog` (`DbSet<ActionLog>`): Database set of audit logs.
  * `Attendance` (`DbSet<Attendance>`): Database set of attendance.
  * `Payroll` (`DbSet<Payroll>`): Database set of payroll.
  * `Item` (`DbSet<Item>`): Database set of items.
  * `ItemTransaction` (`DbSet<ItemTransaction>`): Database set of stock movements.
  * `Sale` (`DbSet<Sale>`): Database set of sales.
  * `SaleEntry` (`DbSet<SaleEntry>`): Database set of sale entries.
  * `Transaction` (`DbSet<Transaction>`): Database set of transactions.
* `CurrentUserContext` (static class in `Data/CurrentUserContext.cs`): Accesses the logged-in user context.
  * `Username` (`string?`): Active request sender username.
* `DbManager` (static class in `Data/DbManager.cs`): Database lifecycle helper.
  * `DbFileName` (`string`): Name of the database file.
  * `DbFilePath` (`string`): Full system path to the database file.
* `DbInitializationFailedEventArgs` (class in `Data/DbManager.cs`): Arguments for database initialization failure. Inherits from `EventArgs`.
  * `Exception` (`Exception`): Captured exception.
  * `Message` (`string`): Descriptive error message.

### `Services`

* `AuthenticationService` (static class in `Services/AuthenticationService.cs`): Verifies credentials.
* `MockCashRegisterService` (class in `Services/MockCashRegisterService.cs`): Simulated cash register drawer helper. Implements `ICashRegisterService`.
* `InventoryService` (static class in `Services/InventoryService.cs`): Manages item stock and queries.
* `SalesService` (static class in `Services/SalesService.cs`): Manages checkouts and sales queries.
* `StoreSettings` (class in `Services/SettingsService.cs`): Store configurations and hardware policies.
  * `StoreName` (`string`): Display name of store.
  * `Currency` (`string`): Currency code.
  * `LowStockThreshold` (`float`): Warning limit for low stock.
  * `WrittenReceiptThreshold` (`float`): Amount threshold requiring physical receipts.
  * `StoreOpeningDate` (`DateTime`): Opening date.
  * `CardExpirationOffsets` (`Dictionary<string, int>`): Access card duration by level.
  * `MembershipCommissions` (`Dictionary<string, float>`): Commission rates by membership upgrade.
* `SettingsService` (static class in `Services/SettingsService.cs`): Loads and saves store configurations.
* `StaffService` (static class in `Services/StaffService.cs`): Handles staff, attendance, and payroll records.

### `Tools`

* `Log` (class in `Tools/Log.cs`): Logging utility helper.
* `Utils` (static class in `Tools/Utils.cs`): General utility functions.
  * `UserSavePath` (`string`): Active documents directory folder.
* `RequestResult` (record in `Tools/Utils.cs`): Outcome of a service request.
  * `Type` (`Result`): Success or error category.
  * `Message` (`string?`): Details.
* `RequestResult<T>` (record in `Tools/Utils.cs`): Generic outcome of a service request. Inherits from `RequestResult`.
  * `Type` (`Result`): Success or error category.
  * `Message` (`string?`): Details.
  * `Data` (`T?`): Generic payload.
* `GetItemRequest` (record in `Tools/ServiceRequests.cs`): Request to get an item by ID.
  * `Id` (`ulong`): Item identifier.
* `FindItemRequest` (record in `Tools/ServiceRequests.cs`): Search query filters for items.
  * `Id` (`ulong?`): Optional identifier.
  * `Name` (`string?`): Optional name.
  * `Description` (`string?`): Optional description.
  * `IsForSale` (`bool?`): Optional sale availability.
  * `MinCost` (`float?`): Optional minimum cost.
  * `MaxCost` (`float?`): Optional maximum cost.
  * `MinSalePrice` (`float?`): Optional minimum sale price.
  * `MaxSalePrice` (`float?`): Optional maximum sale price.
  * `MinQuantity` (`float?`): Optional minimum quantity.
  * `MaxQuantity` (`float?`): Optional maximum quantity.
  * `UnitOfMeasurement` (`Unit?`): Optional unit filter.
  * `MinExpirationDate` (`DateTime?`): Optional minimum expiration date.
  * `MaxExpirationDate` (`DateTime?`): Optional maximum expiration date.
  * `Tags` (`List<Tag>?`): Optional categories.
* `UpdateItemRequest` (record in `Tools/ServiceRequests.cs`): Parameters to update an item.
  * `Id` (`ulong`): Target item identifier.
  * `Name` (`string?`): Item name.
  * `Description` (`string?`): Item description.
  * `IsForSale` (`bool`): Sale availability flag.
  * `Cost` (`float?`): Acquisition cost.
  * `SalePrice` (`float?`): Customer price.
  * `Quantity` (`float`): Updated stock level.
  * `UnitOfMeasurement` (`Unit`): Unit of measurement.
  * `ExpirationDate` (`DateTime?`): Expiration date.
  * `Tags` (`List<Tag>?`): Categories tags.
* `CreateItemRequest` (record in `Tools/ServiceRequests.cs`): Parameters to create an item.
  * `Name` (`string?`): Item name.
  * `Description` (`string?`): Item description.
  * `IsForSale` (`bool`): Sale availability flag.
  * `Cost` (`float?`): Acquisition cost.
  * `SalePrice` (`float?`): Customer price.
  * `Quantity` (`float`): Initial stock level.
  * `UnitOfMeasurement` (`Unit`): Unit of measurement.
  * `ExpirationDate` (`DateTime?`): Expiration date.
  * `Tags` (`List<Tag>?`): Categories tags.
  * `ImageUrl` (`string?`): Image link.
* `DeleteItemRequest` (record in `Tools/ServiceRequests.cs`): Request to delete an item.
  * `Id` (`ulong`): Target item identifier.
* `GetItemTransactionRequest` (record in `Tools/ServiceRequests.cs`): Request to get a stock movement by ID.
  * `Id` (`ulong`): Transaction identifier.
* `CreateItemTransactionRequest` (record in `Tools/ServiceRequests.cs`): Parameters to record stock movement.
  * `ItemId` (`ulong`): Target item identifier.
  * `Type` (`ItemTransaction.Type`): Category of stock change.
  * `SaleEntryId` (`ulong?`): Optional related sale entry identifier.
  * `Quantity` (`float`): Quantity changed.
  * `StaffId` (`ulong`): Staff member identifier.
  * `Timestamp` (`DateTime`): Transaction time.
  * `Note` (`string?`): Additional details.
* `FindItemTransactionRequest` (record in `Tools/ServiceRequests.cs`): Search query filters for stock movements.
  * `Id` (`ulong?`): Optional identifier.
  * `ItemQuery` (`FindItemRequest?`): Optional item search parameters.
  * `MinQuantity` (`float?`): Optional minimum quantity.
  * `MaxQuantity` (`float?`): Optional maximum quantity.
  * `Type` (`ItemTransaction.Type?`): Optional type filter.
  * `MinTimestamp` (`DateTime?`): Optional minimum timestamp.
  * `MaxTimestamp` (`DateTime?`): Optional maximum timestamp.
  * `Note` (`string?`): Optional note substring.
* `UpdateItemTransactionRequest` (record in `Tools/ServiceRequests.cs`): Parameters to update a stock movement.
  * `Id` (`ulong`): Transaction identifier.
  * `ItemId` (`ulong`): Target item identifier.
  * `Quantity` (`float`): Quantity changed.
  * `Type` (`ItemTransaction.Type`): Type of adjustment.
  * `Timestamp` (`DateTime`): Transaction time.
  * `Note` (`string?`): Context details.
* `DeleteItemTransactionRequest` (record in `Tools/ServiceRequests.cs`): Request to delete a stock movement.
  * `Id` (`ulong`): Transaction identifier.
* `SaleItemRequest` (record in `Tools/ServiceRequests.cs`): Represents an item to purchase in a sale.
  * `ItemId` (`ulong`): Target item identifier.
  * `Quantity` (`float`): Quantity to purchase.
  * `UnitPrice` (`float`): Unit price at purchase.
* `SplitPaymentRequest` (record in `Tools/ServiceRequests.cs`): Part of a split payment.
  * `PaymentMethod` (`PayMethod`): Payment method used.
  * `Amount` (`float`): Paid amount.
* `CreateSaleRequest` (record in `Tools/ServiceRequests.cs`): Parameters to process a sale.
  * `StaffId` (`ulong`): Staff member identifier.
  * `PaymentMethod` (`PayMethod`): Principal payment method.
  * `TotalAmount` (`float`): Total checkout amount.
  * `Notes` (`string?`): Transaction notes.
  * `Items` (`List<SaleItemRequest>`): Purchased items.
  * `SplitPayments` (`List<SplitPaymentRequest>?`): Optional split payments breakdown.
  * `PromoCode` (`string?`): Optional promo code.
* `FindSaleRequest` (record in `Tools/ServiceRequests.cs`): Search query filters for sales.
  * `Id` (`ulong?`): Optional sale identifier.
  * `InvoiceNumber` (`int?`): Optional invoice number sequence.
  * `StaffId` (`ulong?`): Optional staff identifier.
  * `MinTimestamp` (`DateTime?`): Optional minimum timestamp.
  * `MaxTimestamp` (`DateTime?`): Optional maximum timestamp.
  * `PaymentMethod` (`PayMethod?`): Optional payment method.
  * `MinAmount` (`float?`): Optional minimum amount.
  * `MaxAmount` (`float?`): Optional maximum amount.
* `GetSaleRequest` (record in `Tools/ServiceRequests.cs`): Request to get a sale by ID.
  * `Id` (`ulong`): Sale identifier.
* `DeleteSaleRequest` (record in `Tools/ServiceRequests.cs`): Request to delete a sale.
  * `Id` (`ulong`): Sale identifier.
* `CreateSaleEntryRequest` (record in `Tools/ServiceRequests.cs`): Parameters to create a sale entry.
  * `SaleId` (`ulong`): Associated sale identifier.
  * `ItemId` (`ulong`): Associated item identifier.
  * `Quantity` (`float`): Quantity purchased.
  * `UnitPrice` (`float`): Price per unit.
  * `Subtotal` (`float`): Total cost of the entry.
* `GetSaleEntryRequest` (record in `Tools/ServiceRequests.cs`): Request to get a sale entry by ID.
  * `Id` (`ulong`): Target entry identifier.
* `FindSaleEntryRequest` (record in `Tools/ServiceRequests.cs`): Search query filters for sale entries.
  * `Id` (`ulong?`): Optional entry identifier.
  * `SaleId` (`ulong?`): Optional parent sale identifier.
  * `ItemId` (`ulong?`): Optional item identifier.
  * `MinQuantity` (`float?`): Optional minimum quantity.
  * `MaxQuantity` (`float?`): Optional maximum quantity.
  * `MinUnitPrice` (`float?`): Optional minimum unit price.
  * `MaxUnitPrice` (`float?`): Optional maximum unit price.
* `DeleteSaleEntryRequest` (record in `Tools/ServiceRequests.cs`): Request to delete a sale entry.
  * `Id` (`ulong`): Entry identifier.
* `CreateStaffRequest` (record in `Tools/ServiceRequests.cs`): Parameters to register staff.
  * `Username` (`string`): Staff username.
  * `Password` (`string?`): Account password.
  * `FirstName` (`string?`): First name.
  * `LastName` (`string?`): Last name.
  * `Email` (`string?`): Email address.
  * `PhoneNumber` (`string?`): Phone number.
  * `Position` (`string?`): Job title or role.
  * `PayFrequency` (`Frequency`): Frequency of payments.
  * `PayRate` (`float`): Payment rate.
* `GetStaffRequest` (record in `Tools/ServiceRequests.cs`): Request to get staff by ID.
  * `Id` (`ulong`): Staff identifier.
* `FindStaffRequest` (record in `Tools/ServiceRequests.cs`): Search query filters for staff.
  * `Id` (`ulong?`): Optional identifier.
  * `Username` (`string?`): Optional username.
  * `FirstName` (`string?`): Optional first name.
  * `LastName` (`string?`): Optional last name.
  * `Position` (`string?`): Optional job position.
  * `PayFrequency` (`Frequency?`): Optional pay frequency.
* `UpdateStaffRequest` (record in `Tools/ServiceRequests.cs`): Parameters to update staff.
  * `Id` (`ulong`): Target staff identifier.
  * `Username` (`string?`): Staff username.
  * `Password` (`string?`): Account password.
  * `FirstName` (`string?`): First name.
  * `LastName` (`string?`): Last name.
  * `Email` (`string?`): Email address.
  * `PhoneNumber` (`string?`): Phone number.
  * `Position` (`string?`): Job title or role.
  * `PayFrequency` (`Frequency?`): Payment frequency.
  * `PayRate` (`float?`): Payment rate.
* `DeleteStaffRequest` (record in `Tools/ServiceRequests.cs`): Request to delete staff.
  * `Id` (`ulong`): Staff identifier.
* `CreatePayrollRequest` (record in `Tools/ServiceRequests.cs`): Parameters to create a payroll record.
  * `StaffId` (`ulong`): Target staff identifier.
  * `PeriodStart` (`DateTime`): Start of pay period.
  * `PeriodEnd` (`DateTime`): End of pay period.
  * `TotalHours` (`float`): Total hours worked.
  * `GrossAmount` (`float`): Gross pay amount.
  * `Modifiers` (`float`): Deductions or bonuses.
  * `NetAmount` (`float`): Net pay amount.
  * `PayDate` (`DateTime?`): Optional payment date.
  * `Notes` (`string?`): Optional remarks.
* `GetPayrollRequest` (record in `Tools/ServiceRequests.cs`): Request to get payroll by ID.
  * `Id` (`ulong`): Payroll identifier.
* `FindPayrollRequest` (record in `Tools/ServiceRequests.cs`): Search query filters for payroll records.
  * `Id` (`ulong?`): Optional identifier.
  * `StaffId` (`ulong?`): Optional staff identifier.
  * `MinPeriodStart` (`DateTime?`): Optional minimum period start.
  * `MaxPeriodStart` (`DateTime?`): Optional maximum period start.
  * `MinPeriodEnd` (`DateTime?`): Optional minimum period end.
  * `MaxPeriodEnd` (`DateTime?`): Optional maximum period end.
  * `MinNetAmount` (`float?`): Optional minimum net amount.
  * `MaxNetAmount` (`float?`): Optional maximum net amount.
* `DeletePayrollRequest` (record in `Tools/ServiceRequests.cs`): Request to delete payroll.
  * `Id` (`ulong`): Payroll identifier.
* `CreateAttendanceRequest` (record in `Tools/ServiceRequests.cs`): Parameters to create an attendance record.
  * `StaffId` (`ulong`): Target staff identifier.
  * `TimeIn` (`DateTime`): Clock-in time.
  * `TimeOut` (`DateTime?`): Optional clock-out time.
* `GetAttendanceRequest` (record in `Tools/ServiceRequests.cs`): Request to get attendance by ID.
  * `Id` (`ulong`): Attendance identifier.
* `FindAttendanceRequest` (record in `Tools/ServiceRequests.cs`): Search query filters for attendance.
  * `Id` (`ulong?`): Optional identifier.
  * `StaffId` (`ulong?`): Optional staff identifier.
  * `MinTimeIn` (`DateTime?`): Optional minimum clock-in time.
  * `MaxTimeIn` (`DateTime?`): Optional maximum clock-in time.
  * `MinTimeOut` (`DateTime?`): Optional minimum clock-out time.
  * `MaxTimeOut` (`DateTime?`): Optional maximum clock-out time.
* `UpdateAttendanceRequest` (record in `Tools/ServiceRequests.cs`): Parameters to update attendance.
  * `Id` (`ulong`): Attendance identifier.
  * `StaffId` (`ulong?`): Associated staff identifier.
  * `TimeIn` (`DateTime?`): Clock-in time.
  * `TimeOut` (`DateTime?`): Clock-out time.
* `DeleteAttendanceRequest` (record in `Tools/ServiceRequests.cs`): Request to delete attendance.
  * `Id` (`ulong`): Attendance identifier.
* `LogAttendanceRequest` (record in `Tools/ServiceRequests.cs`): Parameters to log staff clock action.
  * `StaffId` (`ulong`): Associated staff identifier.
  * `ActionType` (`string`): Clock action description.
* `LoginRequest` (record in `Tools/ServiceRequests.cs`): Parameters to log in.
  * `Username` (`string`): Input username.
  * `Password` (`string`): Input password.
