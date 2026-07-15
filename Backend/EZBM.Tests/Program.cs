using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EZBM.Core.Entities;
using EZBM.Core.Data;
using EZBM.Core.Services;
using EZBM.Core.Tools;

namespace EZBM.Tests;

internal class Program
{
    #region Fields

    private static readonly StringBuilder resultsBuilder = new();

    #endregion

    #region Main Entry Point

    static async Task Main(string[] args)
    {
        DbManager.ConfigureFromArgs(args);
        Console.WriteLine("Starting Endpoint Tests...");
        resultsBuilder.AppendLine("# EZBM Endpoints Test Results");
        resultsBuilder.AppendLine();
        resultsBuilder.AppendLine("This document lists the results of running tests on all API endpoints in the EZBM project.");
        resultsBuilder.AppendLine();
        resultsBuilder.AppendLine("## How the Test Was Performed");
        resultsBuilder.AppendLine();
        resultsBuilder.AppendLine("1. A dedicated test console application (`EZBM.Tests`) was created.");
        resultsBuilder.AppendLine("2. The database was reset using `DbManager.Reset()` before the tests to start with a clean state.");
        resultsBuilder.AppendLine("3. The static controller endpoints inside `EZBM.DesktopHost.Endpoints` were called directly with various payloads.");
        resultsBuilder.AppendLine("4. The results, including returned types, status codes (if any), and success states, were recorded.");
        resultsBuilder.AppendLine();
        resultsBuilder.AppendLine("## Test Results by Controller");
        resultsBuilder.AppendLine();

        try
        {
            // Reset Database
            DbManager.Reset();
            LogResult("Database Reset", "Database was reset to a clean state.", true);

            // 1. StaffController Tests
            await RunStaffTests();

            // 2. AuthController Tests
            await RunAuthTests();

            // 3. InventoryController Tests
            await RunInventoryTests();

            // 4. SalesController Tests
            await RunSalesTests();

            // 5. New Features Tests (Mixed Payments, Promos, Lazy Expire, Audit Logs)
            await RunNewFeatureTests();
        }

        catch (Exception ex)
        {
            resultsBuilder.AppendLine($"### CRITICAL TEST RUN ERROR");
            resultsBuilder.AppendLine($"An unhandled error occurred during testing: {ex.Message}");
            resultsBuilder.AppendLine("```");
            resultsBuilder.AppendLine(ex.ToString());
            resultsBuilder.AppendLine("```");
        }

        // Write Results.md
        string? currentDir = AppContext.BaseDirectory;
        while (currentDir is not null)
        {
            if (File.Exists(Path.Combine(currentDir, "EZBM.slnx"))) break;
            currentDir = Directory.GetParent(currentDir)?.FullName;
        }

        string resultsDir = currentDir ?? AppContext.BaseDirectory;
        string resultsPath = Path.Combine(resultsDir, "Results.md");
        await File.WriteAllTextAsync(resultsPath, resultsBuilder.ToString());
        Console.WriteLine($"Tests completed. Results written to {resultsPath}");
    }

    #endregion

    #region Helper Methods

    private static void LogResult(
        string testName,
        string description,
        bool success,
        string details = ""
    )
    {
        resultsBuilder.AppendLine($"### {testName}");
        resultsBuilder.AppendLine();
        resultsBuilder.AppendLine($"* **Status:** {(success ? "✅ Succeeded" : "❌ Failed / Discrepancy")}");
        resultsBuilder.AppendLine($"* **Description:** {description}");
        if (!string.IsNullOrEmpty(details))
        {
            resultsBuilder.AppendLine();
            resultsBuilder.AppendLine("Details:");
            resultsBuilder.AppendLine("```json");
            resultsBuilder.AppendLine(details);
            resultsBuilder.AppendLine("```");
        }
        resultsBuilder.AppendLine();
        resultsBuilder.AppendLine("---");
        resultsBuilder.AppendLine();
    }

    private static int? GetStatusCode(IResult result)
    {
        if (result is IStatusCodeHttpResult statusCodeResult)
        {
            return statusCodeResult.StatusCode;
        }
        return null;
    }

    #endregion

    #region Test Runners

    private static async Task RunStaffTests()
    {
        resultsBuilder.AppendLine("## StaffController Endpoints");
        resultsBuilder.AppendLine();

        // Test CreateStaff
        CreateStaffRequest createRequest = new(
            Username: "john_doe",
            PayFrequency: Staff.Frequency.Hourly,
            PayRate: 100f,
            Password: "password123",
            FirstName: "John",
            LastName: "Doe",
            Email: "john@example.com",
            PhoneNumber: "123456",
            Position: "Cashier"
        );

        IResult createResult = await StaffController.CreateStaff(createRequest);
        int? createStatus = GetStatusCode(createResult);
        LogResult("Create Staff (Success)",
            "Created a new staff member john_doe.",
            createStatus == 200,
            $"Status: {createStatus}");

        // Find the created staff to get its ID
        FindStaffRequest findRequest = new(
            Id: null,
            Username: "john_doe",
            FirstName: null,
            LastName: null,
            Position: null,
            PayFrequency: null
        );
        IResult findResult = await StaffController.FindStaff(findRequest);
        ulong staffId = 0;
        if (findResult is Ok<List<Staff>> okFindList)
        {
            List<Staff>? staffList = okFindList.Value;
            if (staffList is not null && staffList.Count > 0)
            {
                staffId = staffList[0].Id;
            }
        }

        Ok<List<Staff>>? okFindListResult = findResult as Ok<List<Staff>>;
        int staffCount = okFindListResult?.Value?.Count ?? 0;
        LogResult("Find Staff (Success)",
            $"Searched for staff with username 'john_doe'. Found Staff ID: {staffId}.",
            staffId > 0,
            $"Found {staffCount} records.");

        // GetStaff
        GetStaffRequest getRequest = new(Id: staffId);
        IResult getResult = await StaffController.GetStaff(getRequest);
        int? getStatus = GetStatusCode(getResult);
        Ok<Staff>? okGetStaffResult = getResult as Ok<Staff>;
        bool getStaffSuccess = getStatus == 200 && okGetStaffResult?.Value?.Username == "john_doe";
        LogResult("Get Staff (Success)",
            $"Retrieved the staff member with ID {staffId}.",
            getStaffSuccess,
            $"Status: {getStatus}");

        // UpdateStaff
        UpdateStaffRequest updateRequest = new(
            Id: staffId,
            Username: "john_doe_updated",
            Password: null,
            FirstName: null,
            LastName: null,
            Email: null,
            PhoneNumber: null,
            Position: null,
            PayFrequency: null,
            PayRate: 120f
        );
        IResult updateResult = await StaffController.UpdateStaff(updateRequest);
        int? updateStatus = GetStatusCode(updateResult);
        LogResult("Update Staff (Success)",
            $"Updated pay rate and username of staff member ID {staffId}.",
            updateStatus == 200,
            $"Status: {updateStatus}");

        // Attendance - LogAttendance (In)
        LogAttendanceRequest clockInRequest = new(
            StaffId: staffId,
            ActionType: "In"
        );
        IResult clockInResult = await StaffController.LogAttendance(clockInRequest);
        int? clockInStatus = GetStatusCode(clockInResult);

        // Check returned payload for LogAttendance - Actionable Tasks says it returns a raw DateTime string
        string clockInDetails = "";
        bool logAttendanceDiscrepancy = false;
        if (clockInResult is Ok<DateTime> okTime)
        {
            clockInDetails = $"Returned Ok<DateTime>: {okTime.Value}";
            // Actionable Tasks says it should return {"success": true, "timestamp": "..."} instead of raw DateTime string
            logAttendanceDiscrepancy = true;
        }
        else
        {
            clockInDetails = $"Result type: {clockInResult?.GetType().Name}, Status: {clockInStatus}";
        }

        LogResult("Log Attendance Clock-In (Discrepancy)",
            "Logs staff member clock-in. Discrepancy confirmed: returns raw DateTime instead of JSON object with success and timestamp fields.",
            !logAttendanceDiscrepancy,
            clockInDetails);

        // Attendance - CreateAttendance (Manual)
        CreateAttendanceRequest manualAttendanceRequest = new(
            StaffId: staffId,
            TimeIn: DateTime.UtcNow.AddHours(-8),
            TimeOut: DateTime.UtcNow.AddHours(-1)
        );
        IResult manualAttendanceResult = await StaffController.CreateAttendance(manualAttendanceRequest);
        int? manualAttendanceStatus = GetStatusCode(manualAttendanceResult);
        LogResult("Create Manual Attendance (Success)",
            "Created a manual attendance entry.",
            manualAttendanceStatus == 200,
            $"Status: {manualAttendanceStatus}");

        // GetAttendance
        FindAttendanceRequest findAttRequest = new(
            Id: null,
            StaffId: staffId,
            MinTimeIn: null,
            MaxTimeIn: null,
            MinTimeOut: null,
            MaxTimeOut: null
        );
        IResult findAttResult = await StaffController.FindAttendance(findAttRequest);
        ulong attId = 0;
        if (findAttResult is Ok<List<Attendance>> okAttList)
        {
            List<Attendance>? attList = okAttList.Value;
            if (attList is not null && attList.Count > 0)
            {
                attId = attList[0].Id;
            }
        }

        GetAttendanceRequest getAttRequest = new(Id: attId);
        IResult getAttResult = await StaffController.GetAttendance(getAttRequest);
        int? getAttStatus = GetStatusCode(getAttResult);
        LogResult("Get Attendance (Success)",
            $"Retrieved the attendance record with ID {attId}.",
            getAttStatus == 200,
            $"Status: {getAttStatus}");

        // UpdateAttendance
        UpdateAttendanceRequest updateAttRequest = new(
            Id: attId,
            StaffId: null,
            TimeIn: DateTime.UtcNow.AddHours(-9),
            TimeOut: DateTime.UtcNow.AddHours(-2)
        );
        IResult updateAttResult = await StaffController.UpdateAttendance(updateAttRequest);
        int? updateAttStatus = GetStatusCode(updateAttResult);
        LogResult("Update Attendance (Success)",
            $"Updated the attendance record with ID {attId}.",
            updateAttStatus == 200,
            $"Status: {updateAttStatus}");

        // CreatePayroll
        CreatePayrollRequest createPayrollRequest = new(
            StaffId: staffId,
            PeriodStart: DateTime.UtcNow.AddDays(-14),
            PeriodEnd: DateTime.UtcNow,
            TotalHours: 80f,
            GrossAmount: 8000f,
            Modifiers: 0f,
            NetAmount: 8000f,
            PayDate: DateTime.UtcNow,
            Notes: "Regular pay"
        );
        IResult createPayrollResult = await StaffController.CreatePayroll(createPayrollRequest);
        int? createPayrollStatus = GetStatusCode(createPayrollResult);
        LogResult("Create Payroll (Success)",
            "Created a payroll record.",
            createPayrollStatus == 200,
            $"Status: {createPayrollStatus}");

        // FindPayroll
        FindPayrollRequest findPayrollRequest = new(
            Id: null,
            StaffId: staffId,
            MinPeriodStart: null,
            MaxPeriodStart: null,
            MinPeriodEnd: null,
            MaxPeriodEnd: null,
            MinNetAmount: null,
            MaxNetAmount: null
        );
        IResult findPayrollResult = await StaffController.FindPayroll(findPayrollRequest);
        ulong payrollId = 0;
        if (findPayrollResult is Ok<List<Payroll>> okPayrollList)
        {
            List<Payroll>? payrollList = okPayrollList.Value;
            if (payrollList is not null && payrollList.Count > 0)
            {
                payrollId = payrollList[0].Id;
            }
        }

        Ok<List<Payroll>>? okPayrollListResult = findPayrollResult as Ok<List<Payroll>>;
        int payrollCount = okPayrollListResult?.Value?.Count ?? 0;
        LogResult("Find Payroll (Success)",
            $"Searched for payroll records. Found Payroll ID: {payrollId}.",
            payrollId > 0,
            $"Found {payrollCount} records.");

        // GetPayroll
        GetPayrollRequest getPayrollRequest = new(Id: payrollId);
        IResult getPayrollResult = await StaffController.GetPayroll(getPayrollRequest);
        int? getPayrollStatus = GetStatusCode(getPayrollResult);
        LogResult("Get Payroll (Success)",
            $"Retrieved payroll record with ID {payrollId}.",
            getPayrollStatus == 200,
            $"Status: {getPayrollStatus}");

        // DeletePayroll
        DeletePayrollRequest deletePayrollRequest = new(Id: payrollId);
        IResult deletePayrollResult = await StaffController.DeletePayroll(deletePayrollRequest);
        int? deletePayrollStatus = GetStatusCode(deletePayrollResult);
        LogResult("Delete Payroll (Success)",
            $"Deleted payroll record with ID {payrollId}.",
            deletePayrollStatus == 200,
            $"Status: {deletePayrollStatus}");

        // DeleteAttendance
        DeleteAttendanceRequest deleteAttRequest = new(Id: attId);
        IResult deleteAttResult = await StaffController.DeleteAttendance(deleteAttRequest);
        int? deleteAttStatus = GetStatusCode(deleteAttResult);
        LogResult("Delete Attendance (Success)",
            $"Deleted attendance record with ID {attId}.",
            deleteAttStatus == 200,
            $"Status: {deleteAttStatus}");

        // DeleteStaff
        DeleteStaffRequest deleteStaffRequest = new(Id: staffId);
        IResult deleteStaffResult = await StaffController.DeleteStaff(deleteStaffRequest);
        int? deleteStaffStatus = GetStatusCode(deleteStaffResult);
        LogResult("Delete Staff (Success)",
            $"Deleted staff profile with ID {staffId}.",
            deleteStaffStatus == 200,
            $"Status: {deleteStaffStatus}");
    }

    private static async Task RunAuthTests()
    {
        resultsBuilder.AppendLine("## AuthController Endpoints");
        resultsBuilder.AppendLine();

        // Create a temporary staff member for authentication
        CreateStaffRequest createRequest = new(
            Username: "auth_test_user",
            PayFrequency: Staff.Frequency.Monthly,
            PayRate: 50000f,
            Password: "secretPassword",
            FirstName: "Auth",
            LastName: "Test",
            Email: "auth@example.com",
            PhoneNumber: "000000",
            Position: "Manager"
        );
        await StaffController.CreateStaff(createRequest);

        // Test Login - Success
        EZBM.DesktopHost.Endpoints.LoginRequest loginSuccessRequest = new(
            Username: "auth_test_user",
            Password: "secretPassword"
        );
        IResult loginSuccessResult = await AuthController.Login(loginSuccessRequest);
        int? loginSuccessStatus = GetStatusCode(loginSuccessResult);
        LogResult("Login (Success Case)",
            "Authenticated with valid credentials.",
            loginSuccessStatus == 200,
            $"Status: {loginSuccessStatus}");

        // Test Login - Invalid Password
        EZBM.DesktopHost.Endpoints.LoginRequest loginFailRequest = new(
            Username: "auth_test_user",
            Password: "wrongPassword"
        );
        IResult loginFailResult = await AuthController.Login(loginFailRequest);
        int? loginFailStatus = GetStatusCode(loginFailResult);
        LogResult("Login (Failure Case - Wrong Password)",
            "Attempted authentication with an incorrect password.",
            loginFailStatus == 401,
            $"Status: {loginFailStatus}");

        // Clean up staff
        FindStaffRequest findRequest = new(
            Id: null,
            Username: "auth_test_user",
            FirstName: null,
            LastName: null,
            Position: null,
            PayFrequency: null
        );
        IResult findResult = await StaffController.FindStaff(findRequest);
        if (findResult is Ok<List<Staff>> okList)
        {
            List<Staff>? staffList = okList.Value;
            if (staffList is not null && staffList.Count > 0)
            {
                ulong staffId = staffList[0].Id;
                await StaffController.DeleteStaff(new(Id: staffId));
            }
        }
    }

    private static async Task RunInventoryTests()
    {
        resultsBuilder.AppendLine("## InventoryController Endpoints");
        resultsBuilder.AppendLine();

        // Create Item
        CreateItemRequest createRequest = new(
            UnitOfMeasurement: Item.Unit.Count,
            IsForSale: true,
            SalePrice: 120f,
            Name: "Burger Combo",
            Description: "Burger with regular fries and drink",
            Tags: new() { "Food" },
            Quantity: 10f,
            ExpirationDate: null,
            Cost: 80f,
            ImageUrl: null
        );

        IResult createResult = await InventoryController.CreateItem(createRequest);
        int? createStatus = GetStatusCode(createResult);

        // Check Actionable Tasks bug - CreateItem returns empty 200 OK response on success instead of created item
        bool createItemDiscrepancy = false;
        string createDetails = "";
        if (createResult is Ok okResult)
        {
            createItemDiscrepancy = true; // Confirmed: returns empty Ok (no value)
            createDetails = "Returned empty Ok (no created item payload).";
        }
        else
        {
            createDetails = $"Result type: {createResult?.GetType().Name}, Status: {createStatus}";
        }

        LogResult("Create Item (Discrepancy)",
            "Creates a new inventory item. Discrepancy confirmed: returns an empty Ok response (200) instead of the created item object with its generated database ID.",
            !createItemDiscrepancy,
            createDetails);

        // Find the created item to get its ID
        FindItemRequest findRequest = new(
            Id: null,
            Name: "Burger Combo",
            Description: null,
            IsForSale: null,
            MinCost: null,
            MaxCost: null,
            MinSalePrice: null,
            MaxSalePrice: null,
            MinQuantity: null,
            MaxQuantity: null,
            UnitOfMeasurement: null,
            MinExpirationDate: null,
            MaxExpirationDate: null,
            Tags: null
        );
        IResult findResult = await InventoryController.FindItems(findRequest);
        ulong itemId = 0;
        if (findResult is Ok<List<Item>> okList)
        {
            List<Item>? itemList = okList.Value;
            if (itemList is not null && itemList.Count > 0)
            {
                itemId = itemList[0].Id;
            }
        }

        Ok<List<Item>>? okItemListResult = findResult as Ok<List<Item>>;
        int itemCount = okItemListResult?.Value?.Count ?? 0;
        LogResult("Find Items (Success)",
            $"Searched for items matching query. Found Item ID: {itemId}.",
            itemId > 0,
            $"Found {itemCount} records.");

        // GetItem
        GetItemRequest getRequest = new(Id: itemId);
        IResult getResult = await InventoryController.GetItem(getRequest);
        int? getStatus = GetStatusCode(getResult);
        LogResult("Get Item (Success)",
            $"Retrieved inventory item ID {itemId}.",
            getStatus == 200,
            $"Status: {getStatus}");

        // GetAllItems
        IResult getAllResult = await InventoryController.GetAllItems();
        int? getAllStatus = GetStatusCode(getAllResult);
        LogResult("Get All Items (Success)",
            "Retrieved all items in the inventory.",
            getAllStatus == 200,
            $"Status: {getAllStatus}");

        // Add Item Tags
        AddItemTagsRequest addTagsReq = new(Id: itemId, Tags: new() { "FastFood", "Combo" });
        IResult addTagsRes = await InventoryController.AddItemTags(addTagsReq);
        int? addTagsStatus = GetStatusCode(addTagsRes);
        IResult getResultForAdd = await InventoryController.GetItem(getRequest);
        bool tagsAddedSuccessfully = false;
        string addTagsDetails = $"Status: {addTagsStatus}";
        if (getResultForAdd is Ok<Item> okItemForAdd)
        {
            List<string>? tags = okItemForAdd.Value?.Tags;
            if (tags is not null && tags.Contains("Food") && tags.Contains("FastFood") && tags.Contains("Combo"))
            {
                tagsAddedSuccessfully = true;
                addTagsDetails = $"Tags are: {string.Join(", ", tags)}";
            }
        }
        LogResult("Add Item Tags (Success)",
            $"Added tags to item ID {itemId}.",
            addTagsStatus == 200 && tagsAddedSuccessfully,
            addTagsDetails);

        // Replace Item Tags
        ReplaceItemTagsRequest replaceTagsReq = new(Id: itemId, Tags: new() { "FastFoodOnly" });
        IResult replaceTagsRes = await InventoryController.ReplaceItemTags(replaceTagsReq);
        int? replaceTagsStatus = GetStatusCode(replaceTagsRes);
        IResult getResultForReplace = await InventoryController.GetItem(getRequest);
        bool tagsReplacedSuccessfully = false;
        string replaceTagsDetails = $"Status: {replaceTagsStatus}";
        if (getResultForReplace is Ok<Item> okItemForReplace)
        {
            List<string>? tags = okItemForReplace.Value?.Tags;
            if (tags is not null && tags.Count == 1 && tags[0] == "FastFoodOnly")
            {
                tagsReplacedSuccessfully = true;
                replaceTagsDetails = $"Tags are: {string.Join(", ", tags)}";
            }
        }
        LogResult("Replace Item Tags (Success)",
            $"Replaced tags for item ID {itemId}.",
            replaceTagsStatus == 200 && tagsReplacedSuccessfully,
            replaceTagsDetails);

        // Remove Item Tags
        RemoveItemTagsRequest removeTagsReq = new(Id: itemId, Tags: new() { "FastFoodOnly" });
        IResult removeTagsRes = await InventoryController.RemoveItemTags(removeTagsReq);
        int? removeTagsStatus = GetStatusCode(removeTagsRes);
        IResult getResultForRemove = await InventoryController.GetItem(getRequest);
        bool tagsRemovedSuccessfully = false;
        string removeTagsDetails = $"Status: {removeTagsStatus}";
        if (getResultForRemove is Ok<Item> okItemForRemove)
        {
            List<string>? tags = okItemForRemove.Value?.Tags;
            if (tags is not null && tags.Count == 0)
            {
                tagsRemovedSuccessfully = true;
                removeTagsDetails = "Tags list is empty.";
            }
        }
        LogResult("Remove Item Tags (Success)",
            $"Removed tags from item ID {itemId}.",
            removeTagsStatus == 200 && tagsRemovedSuccessfully,
            removeTagsDetails);


        // Create temporary staff to perform transaction
        CreateStaffRequest staffReq = new(
            Username: "inv_staff",
            PayFrequency: Staff.Frequency.Hourly,
            PayRate: 100f,
            Password: "password",
            FirstName: "Inv",
            LastName: "Staff",
            Email: "inv@example.com",
            PhoneNumber: "123",
            Position: "StockManager"
        );
        await StaffController.CreateStaff(staffReq);

        FindStaffRequest findStaffReq = new(
            Id: null,
            Username: "inv_staff",
            FirstName: null,
            LastName: null,
            Position: null,
            PayFrequency: null
        );
        IResult findStaffRes = await StaffController.FindStaff(findStaffReq);
        Ok<List<Staff>>? okInvStaffRes = findStaffRes as Ok<List<Staff>>;
        ulong staffId = okInvStaffRes?.Value?[0].Id ?? 0;

        // CreateItemTransaction
        CreateItemTransactionRequest txnRequest = new(
            ItemId: itemId,
            Type: ItemTransaction.Type.NewStock,
            SaleEntryId: null,
            Quantity: 5f,
            StaffId: staffId,
            Timestamp: DateTime.UtcNow,
            Note: "Adding new stock"
        );
        IResult txnResult = await InventoryController.CreateItemTransaction(txnRequest);
        int? txnStatus = GetStatusCode(txnResult);
        LogResult("Create Item Transaction (Success)",
            "Logged a stock transaction movement.",
            txnStatus == 200,
            $"Status: {txnStatus}");

        // FindItemTransactions
        FindItemTransactionRequest findTxnRequest = new(
            Id: null,
            MinQuantity: null,
            MaxQuantity: null,
            Type: null,
            MinTimestamp: null,
            MaxTimestamp: null,
            Note: null,
            ItemQuery: null
        );
        IResult findTxnResult = await InventoryController.FindItemTransactions(findTxnRequest);
        ulong txnId = 0;
        if (findTxnResult is Ok<List<ItemTransaction>> okTxnList)
        {
            List<ItemTransaction>? txnList = okTxnList.Value;
            if (txnList is not null && txnList.Count > 0)
            {
                txnId = txnList[0].Id;
            }
        }

        Ok<List<ItemTransaction>>? okTxnListResult = findTxnResult as Ok<List<ItemTransaction>>;
        int txnListCount = okTxnListResult?.Value?.Count ?? 0;
        LogResult("Find Item Transactions (Success)",
            $"Searched for stock transactions. Found Transaction ID: {txnId}.",
            txnId > 0,
            $"Found {txnListCount} records.");

        // GetItemTransaction
        GetItemTransactionRequest getTxnRequest = new(Id: txnId);
        IResult getTxnResult = await InventoryController.GetItemTransaction(getTxnRequest);
        int? getTxnStatus = GetStatusCode(getTxnResult);
        LogResult("Get Item Transaction (Success)",
            $"Retrieved stock transaction ID {txnId}.",
            getTxnStatus == 200,
            $"Status: {getTxnStatus}");

        // DeleteItemTransaction
        DeleteItemTransactionRequest deleteTxnRequest = new(Id: txnId);
        IResult deleteTxnResult = await InventoryController.DeleteItemTransaction(deleteTxnRequest);
        int? deleteTxnStatus = GetStatusCode(deleteTxnResult);
        LogResult("Delete Item Transaction (Success)",
            $"Deleted stock transaction ID {txnId}.",
            deleteTxnStatus == 200,
            $"Status: {deleteTxnStatus}");

        // DeleteItem
        DeleteItemRequest deleteRequest = new(Id: itemId);
        IResult deleteResult = await InventoryController.DeleteItem(deleteRequest);
        int? deleteStatus = GetStatusCode(deleteResult);
        LogResult("Delete Item (Success)",
            $"Deleted item ID {itemId}.",
            deleteStatus == 200,
            $"Status: {deleteStatus}");

        // Clean up staff
        await StaffController.DeleteStaff(new(Id: staffId));
    }

    private static async Task RunSalesTests()
    {
        resultsBuilder.AppendLine("## SalesController Endpoints");
        resultsBuilder.AppendLine();

        // Seed Staff and Item
        CreateStaffRequest staffReq = new(
            Username: "sales_staff",
            PayFrequency: Staff.Frequency.Hourly,
            PayRate: 100f,
            Password: "password",
            FirstName: "Sales",
            LastName: "Staff",
            Email: "sales@example.com",
            PhoneNumber: "123",
            Position: "Cashier"
        );
        await StaffController.CreateStaff(staffReq);

        FindStaffRequest findStaffReq = new(
            Id: null,
            Username: "sales_staff",
            FirstName: null,
            LastName: null,
            Position: null,
            PayFrequency: null
        );
        IResult findStaffRes = await StaffController.FindStaff(findStaffReq);
        Ok<List<Staff>>? okSalesStaffRes = findStaffRes as Ok<List<Staff>>;
        ulong staffId = okSalesStaffRes?.Value?[0].Id ?? 0;

        CreateItemRequest itemReq = new(
            UnitOfMeasurement: Item.Unit.Count,
            IsForSale: true,
            SalePrice: 120f,
            Name: "Burger Combo",
            Description: "Burger with regular fries and drink",
            Tags: new() { "Food" },
            Quantity: 10f,
            ExpirationDate: null,
            Cost: 80f,
            ImageUrl: null
        );
        await InventoryController.CreateItem(itemReq);

        FindItemRequest findItemReq = new(
            Id: null,
            Name: "Burger Combo",
            Description: null,
            IsForSale: null,
            MinCost: null,
            MaxCost: null,
            MinSalePrice: null,
            MaxSalePrice: null,
            MinQuantity: null,
            MaxQuantity: null,
            UnitOfMeasurement: null,
            MinExpirationDate: null,
            MaxExpirationDate: null,
            Tags: null
        );
        IResult findItemRes = await InventoryController.FindItems(findItemReq);
        Ok<List<Item>>? okBurgerItemRes = findItemRes as Ok<List<Item>>;
        ulong itemId = okBurgerItemRes?.Value?[0].Id ?? 0;

        // CreateSale
        CreateSaleRequest createRequest = new(
            StaffId: staffId,
            PaymentMethod: Transaction.PayMethod.Cash,
            TotalAmount: 120f,
            Notes: "Customer checkout",
            Items: new()
            {
                new(ItemId: itemId, Quantity: 1f, UnitPrice: 120f)
            }
        );

        IResult createResult = await SalesController.CreateSale(createRequest);
        int? createStatus = GetStatusCode(createResult);

        // Check Actionable Tasks bug - CreateSale returns empty 200 OK response on success instead of success & saleId
        bool createSaleDiscrepancy = false;
        string createDetails = "";
        if (createResult is Ok okResult)
        {
            createSaleDiscrepancy = true; // Confirmed: returns empty Ok (no value)
            createDetails = "Returned empty Ok (no saleId payload).";
        }
        else
        {
            createDetails = $"Result type: {createResult?.GetType().Name}, Status: {createStatus}";
        }

        LogResult("Create Sale (Discrepancy)",
            "Registers a sales transaction. Discrepancy confirmed: returns an empty Ok response (200) instead of a success payload containing the generated saleId.",
            !createSaleDiscrepancy,
            createDetails);

        // Find the created sale to get its ID
        FindSaleRequest findRequest = new(
            Id: null,
            InvoiceNumber: null,
            StaffId: staffId,
            MinTimestamp: null,
            MaxTimestamp: null,
            PaymentMethod: null,
            MinAmount: null,
            MaxAmount: null
        );
        IResult findResult = await SalesController.FindSales(findRequest);
        ulong saleId = 0;
        if (findResult is Ok<List<Sale>> okList)
        {
            List<Sale>? saleList = okList.Value;
            if (saleList is not null && saleList.Count > 0)
            {
                saleId = saleList[0].Id;
            }
        }

        Ok<List<Sale>>? okSaleListResult = findResult as Ok<List<Sale>>;
        int saleCount = okSaleListResult?.Value?.Count ?? 0;
        LogResult("Find Sales (Success)",
            $"Searched for sales transactions. Found Sale ID: {saleId}.",
            saleId > 0,
            $"Found {saleCount} records.");

        // GetSale
        GetSaleRequest getRequest = new(Id: saleId);
        IResult getResult = await SalesController.GetSale(getRequest);
        int? getStatus = GetStatusCode(getResult);
        LogResult("Get Sale (Success)",
            $"Retrieved sales transaction ID {saleId}.",
            getStatus == 200,
            $"Status: {getStatus}");

        // Find SaleEntry to get its ID
        FindSaleEntryRequest findEntryRequest = new(
            Id: null,
            SaleId: saleId,
            ItemId: null,
            MinQuantity: null,
            MaxQuantity: null,
            MinUnitPrice: null,
            MaxUnitPrice: null
        );
        IResult findEntryResult = await SalesController.FindSaleEntries(findEntryRequest);
        ulong entryId = 0;
        if (findEntryResult is Ok<List<SaleEntry>> okEntryList)
        {
            List<SaleEntry>? entryList = okEntryList.Value;
            if (entryList is not null && entryList.Count > 0)
            {
                entryId = entryList[0].Id;
            }
        }

        Ok<List<SaleEntry>>? okEntryListResult = findEntryResult as Ok<List<SaleEntry>>;
        int entryCount = okEntryListResult?.Value?.Count ?? 0;
        LogResult("Find Sale Entries (Success)",
            $"Searched for sale entry line items. Found Entry ID: {entryId}.",
            entryId > 0,
            $"Found {entryCount} records.");

        // GetSaleEntry
        GetSaleEntryRequest getEntryRequest = new(Id: entryId);
        IResult getEntryResult = await SalesController.GetSaleEntry(getEntryRequest);
        int? getEntryStatus = GetStatusCode(getEntryResult);
        LogResult("Get Sale Entry (Success)",
            $"Retrieved sale entry ID {entryId}.",
            getEntryStatus == 200,
            $"Status: {getEntryStatus}");

        // DeleteSaleEntry
        DeleteSaleEntryRequest deleteEntryRequest = new(Id: entryId);
        IResult deleteEntryResult = await SalesController.DeleteSaleEntry(deleteEntryRequest);
        int? deleteEntryStatus = GetStatusCode(deleteEntryResult);
        LogResult("Delete Sale Entry (Success)",
            $"Deleted sale entry ID {entryId}.",
            deleteEntryStatus == 200,
            $"Status: {deleteEntryStatus}");

        // DeleteSale
        DeleteSaleRequest deleteRequest = new(Id: saleId);
        IResult deleteResult = await SalesController.DeleteSale(deleteRequest);
        int? deleteStatus = GetStatusCode(deleteResult);
        LogResult("Delete Sale (Success)",
            $"Deleted sales transaction ID {saleId}.",
            deleteStatus == 200,
            $"Status: {deleteStatus}");

        // Clean up staff and item
        await InventoryController.DeleteItem(new(Id: itemId));
        await StaffController.DeleteStaff(new(Id: staffId));
    }

    private static async Task RunNewFeatureTests()
    {
        resultsBuilder.AppendLine("## Custom New Features Verification");
        resultsBuilder.AppendLine();

        using AppDbContext db = new();

        Customer user = new(
            id: Utils.GenerateEntityId(),
            firstName: "Lazy",
            lastName: "Tester"
        )
        {
            RfidCardId = "lazy_card",
            Permissions = new List<string> { "EnterStore", "Checkout" },
            PermissionsAfterExpiry = new List<string> { "GuestViewOnly" },
            ExpirationDate = DateTime.UtcNow.AddMinutes(-5)
        };
        db.Customer.Add(user);
        await db.SaveChangesAsync();

        List<string> activePerms = user.GetActivePermissions();
        bool lazyOk = activePerms.Count == 1 && activePerms[0] == "GuestViewOnly";
        LogResult("Lazy Permissions Expiry Override",
            "Verified that active permissions default to PermissionsAfterExpiry when ExpirationDate has passed.",
            lazyOk,
            $"Active permissions: {string.Join(", ", activePerms)}");

        user.FirstName = "LazyUpdated";
        await db.SaveChangesAsync();

        ActionLog? latestLog = await db.ActionLog
            .OrderByDescending(l => l.Timestamp)
            .FirstOrDefaultAsync();

        bool auditOk = latestLog is not null && latestLog.ActionType == "Edit" && latestLog.Details.Contains("LazyUpdated");
        LogResult("Audit Logs Automatic Capturing",
            "Verified that editing an entity automatically triggers and logs an ActionLog entry.",
            auditOk,
            latestLog is not null ? $"Log Details: {latestLog.Details}" : "No logs captured.");

        Staff staff = new(Utils.GenerateEntityId(), "test_mixed_cashier", Staff.Frequency.Hourly, 10f);
        Item item = new(Utils.GenerateEntityId(), Item.Unit.Count, true, 20f, "Mixed Test Apple");
        db.Staff.Add(staff);
        db.Item.Add(item);
        await db.SaveChangesAsync();

        List<SplitPaymentRequest> splitPayments = new()
        {
            new SplitPaymentRequest(Transaction.PayMethod.Cash, 10f),
            new SplitPaymentRequest(Transaction.PayMethod.EWallet, 10f)
        };

        CreateSaleRequest saleReq = new(
            StaffId: staff.Id,
            PaymentMethod: Transaction.PayMethod.Mixed,
            TotalAmount: 20f,
            Notes: "Mixed Payment Test",
            Items: new List<SaleItemRequest> { new SaleItemRequest(item.Id, 1f, 20f) },
            SplitPayments: splitPayments
        );

        Utils.RequestResult<ulong> saleRes = await SalesService.CreateSaleAsync(saleReq);
        bool mixedOk = false;
        if (saleRes.Type == Utils.Result.Success)
        {
            ulong parentSaleId = saleRes.Data;
            List<Transaction> childTxs = await db.Transaction
                .Where(t => t.ParentTransactionId == parentSaleId)
                .ToListAsync();

            mixedOk = childTxs.Count == 2 &&
                      childTxs.Any(t => t.PaymentMethod == Transaction.PayMethod.Cash && t.Amount == 10f) &&
                      childTxs.Any(t => t.PaymentMethod == Transaction.PayMethod.EWallet && t.Amount == 10f);
        }

        LogResult("Mixed Payment Split Record Decoupling",
            "Verified that mixed payment checkouts correctly register parent Sale and multiple child split payment records.",
            mixedOk,
            $"Parent Sale Status: {saleRes.Type}. Child Payment Records count: {(saleRes.Type == Utils.Result.Success ? db.Transaction.Count(t => t.ParentTransactionId == saleRes.Data).ToString() : "N/A")}");

        StoreSettings settings = SettingsService.LoadSettings();
        DateTime oldOpeningDate = settings.StoreOpeningDate;
        settings.StoreOpeningDate = DateTime.UtcNow;
        SettingsService.SaveSettings(settings);

        CreateSaleRequest promoReqValid = new(
            StaffId: staff.Id,
            PaymentMethod: Transaction.PayMethod.Cash,
            TotalAmount: 20f,
            Notes: "Promo code valid",
            Items: new List<SaleItemRequest> { new SaleItemRequest(item.Id, 1f, 20f) },
            PromoCode: "FREEWEEK"
        );

        Utils.RequestResult<ulong> promoResValid = await SalesService.CreateSaleAsync(promoReqValid);
        
        settings.StoreOpeningDate = oldOpeningDate;
        SettingsService.SaveSettings(settings);

        bool promoOk = promoResValid.Type == Utils.Result.Success;
        if (promoOk)
        {
            Sale? verifiedSale = await db.Sale.FindAsync(promoResValid.Data);
            promoOk = verifiedSale is not null && verifiedSale.Amount == 0f;
        }

        LogResult("FREEWEEK Promotion Validity Offset",
            "Verified that applying 'FREEWEEK' within 7 days of store opening discounts the total amount to $0.",
            promoOk,
            $"Promo checkout result: {promoResValid.Type}. Final Sale Amount charged: {(promoOk ? "0.00" : "error")}");
    }

    #endregion
}
