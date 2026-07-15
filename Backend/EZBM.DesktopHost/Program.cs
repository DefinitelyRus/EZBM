using EZBM.DesktopHost.Endpoints;
using EZBM.DesktopHost.Tools;
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

EZBM.Core.Data.DbManager.ConfigureFromArgs(args);
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
app.MapCrud("/api/items", InventoryController.CreateItem, InventoryController.GetItem, InventoryController.FindItems, null, InventoryController.DeleteItem);
app.MapGet("/api/items/barcode/{code}", InventoryController.LookupBarcode);
app.MapPost("/api/items/tags/add", InventoryController.AddItemTags);
app.MapPost("/api/items/tags/replace", InventoryController.ReplaceItemTags);
app.MapPost("/api/items/tags/remove", InventoryController.RemoveItemTags);

// Inventory Transaction Endpoints
app.MapCrud("/api/items/transactions", InventoryController.CreateItemTransaction, InventoryController.GetItemTransaction, InventoryController.FindItemTransactions, null, InventoryController.DeleteItemTransaction);

// Sales Endpoints
app.MapPost("/api/sales", SalesController.CreateSale);
app.MapCrud("/api/sales", SalesController.CreateSale, SalesController.GetSale, SalesController.FindSales, null, SalesController.DeleteSale);

// Sale Entry Endpoints
app.MapCrud("/api/sales/entries", SalesController.CreateSaleEntry, SalesController.GetSaleEntry, SalesController.FindSaleEntries, null, SalesController.DeleteSaleEntry);

// Staff Endpoints
app.MapCrud("/api/staff", StaffController.CreateStaff, StaffController.GetStaff, StaffController.FindStaff, StaffController.UpdateStaff, StaffController.DeleteStaff);

// Staff Adjustment Endpoints
app.MapCrud("/api/staff/adjustments", StaffAdjustmentController.CreateStaffAdjustment, StaffAdjustmentController.GetStaffAdjustment, StaffAdjustmentController.FindStaffAdjustments, null, StaffAdjustmentController.DeleteStaffAdjustment);

// Customer Endpoints
app.MapCrud("/api/customers", CustomerController.CreateCustomer, CustomerController.GetCustomer, CustomerController.FindCustomers, CustomerController.UpdateCustomer, CustomerController.DeleteCustomer);

// Role Endpoints
app.MapCrud("/api/roles", RoleController.CreateRole, RoleController.GetRole, RoleController.FindRoles, RoleController.UpdateRole, RoleController.DeleteRole);

// Attendance Endpoints
app.MapPost("/api/attendance", StaffController.LogAttendance);
app.MapCrud("/api/attendance", StaffController.CreateAttendance, StaffController.GetAttendance, StaffController.FindAttendance, StaffController.UpdateAttendance, StaffController.DeleteAttendance);

// Payroll Endpoints
app.MapCrud("/api/payroll", StaffController.CreatePayroll, StaffController.GetPayroll, StaffController.FindPayroll, null, StaffController.DeletePayroll);
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
