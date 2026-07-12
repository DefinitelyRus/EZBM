using EZBM.DesktopHost.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddSingleton<EZBM.Core.Services.ICashRegisterService, EZBM.Core.Services.MockCashRegisterService>();

// Enable CORS for port 5173
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

WebApplication app = builder.Build();

EZBM.Core.Data.DbManager.Initialize();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-Operator-Username", out var username))
    {
        EZBM.Core.Data.CurrentUserContext.Username = username.ToString();
    }
    else
    {
        EZBM.Core.Data.CurrentUserContext.Username = "System";
    }
    await next();
});

// Authentication Endpoints
app.MapPost("/api/auth/login", AuthController.Login);
app.MapPost("/api/auth/setup", AuthController.Setup);

// Inventory Endpoints
app.MapGet("/api/items", InventoryController.GetAllItems);
app.MapPost("/api/items/get", InventoryController.GetItem);
app.MapPost("/api/items/find", InventoryController.FindItems);
app.MapPost("/api/items/create", InventoryController.CreateItem);
app.MapPost("/api/items/delete", InventoryController.DeleteItem);
app.MapGet("/api/items/barcode/{code}", InventoryController.LookupBarcode);

// Inventory Transaction Endpoints
app.MapPost("/api/items/transactions/create", InventoryController.CreateItemTransaction);
app.MapPost("/api/items/transactions/get", InventoryController.GetItemTransaction);
app.MapPost("/api/items/transactions/find", InventoryController.FindItemTransactions);
app.MapPost("/api/items/transactions/delete", InventoryController.DeleteItemTransaction);

// Sales Endpoints
app.MapPost("/api/sales", SalesController.CreateSale);
app.MapPost("/api/sales/create", SalesController.CreateSale);
app.MapPost("/api/sales/get", SalesController.GetSale);
app.MapPost("/api/sales/find", SalesController.FindSales);
app.MapPost("/api/sales/delete", SalesController.DeleteSale);

// Sale Entry Endpoints
app.MapPost("/api/sales/entries/create", SalesController.CreateSaleEntry);
app.MapPost("/api/sales/entries/get", SalesController.GetSaleEntry);
app.MapPost("/api/sales/entries/find", SalesController.FindSaleEntries);
app.MapPost("/api/sales/entries/delete", SalesController.DeleteSaleEntry);

// Staff Endpoints
app.MapPost("/api/staff/create", StaffController.CreateStaff);
app.MapPost("/api/staff/get", StaffController.GetStaff);
app.MapPost("/api/staff/find", StaffController.FindStaff);
app.MapPost("/api/staff/update", StaffController.UpdateStaff);
app.MapPost("/api/staff/delete", StaffController.DeleteStaff);

// Staff Adjustment Endpoints
app.MapPost("/api/staff/adjustments/create", StaffAdjustmentController.CreateStaffAdjustment);
app.MapPost("/api/staff/adjustments/get", StaffAdjustmentController.GetStaffAdjustment);
app.MapPost("/api/staff/adjustments/find", StaffAdjustmentController.FindStaffAdjustments);
app.MapPost("/api/staff/adjustments/delete", StaffAdjustmentController.DeleteStaffAdjustment);

// Customer Endpoints
app.MapPost("/api/customers/create", CustomerController.CreateCustomer);
app.MapPost("/api/customers/get", CustomerController.GetCustomer);
app.MapPost("/api/customers/find", CustomerController.FindCustomers);
app.MapPost("/api/customers/update", CustomerController.UpdateCustomer);
app.MapPost("/api/customers/delete", CustomerController.DeleteCustomer);

// Role Endpoints
app.MapPost("/api/roles/create", RoleController.CreateRole);
app.MapPost("/api/roles/get", RoleController.GetRole);
app.MapPost("/api/roles/find", RoleController.FindRoles);
app.MapPost("/api/roles/update", RoleController.UpdateRole);
app.MapPost("/api/roles/delete", RoleController.DeleteRole);

// Attendance Endpoints
app.MapPost("/api/attendance", StaffController.LogAttendance);
app.MapPost("/api/attendance/create", StaffController.CreateAttendance);
app.MapPost("/api/attendance/get", StaffController.GetAttendance);
app.MapPost("/api/attendance/find", StaffController.FindAttendance);
app.MapPost("/api/attendance/update", StaffController.UpdateAttendance);
app.MapPost("/api/attendance/delete", StaffController.DeleteAttendance);

// Payroll Endpoints
app.MapPost("/api/payroll/create", StaffController.CreatePayroll);
app.MapPost("/api/payroll/get", StaffController.GetPayroll);
app.MapPost("/api/payroll/find", StaffController.FindPayroll);
app.MapPost("/api/payroll/delete", StaffController.DeletePayroll);
app.MapGet("/api/payroll/calculate", StaffController.CalculatePayroll);

// Action Audit Logs Endpoint
app.MapGet("/api/logs", LogsController.GetActionLogs);

// Enums Endpoints
app.MapGet("/api/enums", EnumsController.GetEnums);
app.MapGet("/api/enums/units", EnumsController.GetUnits);
app.MapGet("/api/enums/tags", EnumsController.GetTags);
app.MapGet("/api/enums/access-cards", EnumsController.GetAccessCardTypes);
app.MapGet("/api/enums/frequencies", EnumsController.GetPayFrequencies);
app.MapGet("/api/enums/stock-transaction-types", EnumsController.GetStockTransactionTypes);
app.MapGet("/api/enums/transaction-types", EnumsController.GetTransactionTypes);
app.MapGet("/api/enums/payment-methods", EnumsController.GetPaymentMethods);

// Settings Endpoints
app.MapGet("/api/settings", SettingsController.GetSettings);
app.MapPost("/api/settings", SettingsController.SaveSettings);
app.MapGet("/api/settings/user/{id}", SettingsController.GetUserSettings);
app.MapPost("/api/settings/user/{id}", SettingsController.SaveUserSettings);

// Dashboard Endpoints
app.MapGet("/api/dashboard/analytics", AnalyticsController.GetAnalytics);

// Backup Sync Endpoints
app.MapPost("/api/sync/backup", SyncController.SyncBackup);
app.MapPost("/api/sync/restore", SyncController.SyncRestore);

app.Run();
