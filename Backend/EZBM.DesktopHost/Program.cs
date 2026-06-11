using EZBM.DesktopHost.Endpoints;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
WebApplication app = builder.Build();

EZBM.Core.Data.DbManager.Initialize();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.MapPost("/api/auth/login", AuthController.Login);

// Inventory Endpoints
app.MapGet("/api/items", InventoryController.GetAllItems);
app.MapPost("/api/items/get", InventoryController.GetItem);
app.MapPost("/api/items/find", InventoryController.FindItems);
app.MapPost("/api/items/create", InventoryController.CreateItem);
app.MapPost("/api/items/delete", InventoryController.DeleteItem);

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

app.Run();

