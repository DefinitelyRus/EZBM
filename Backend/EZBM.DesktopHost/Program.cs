using EZBM.DesktopHost.Endpoints;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/auth/login", AuthController.Login);

// Inventory Endpoints
app.MapGet("/api/items", InventoryController.GetAllItems);
app.MapPost("/api/items/get", InventoryController.GetItem);
app.MapPost("/api/items/find", InventoryController.FindItems);
app.MapPost("/api/items/create", InventoryController.CreateItem);
app.MapPost("/api/items/delete", InventoryController.DeleteItem);
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
app.MapPost("/api/attendance", AttendanceController.LogAttendance);
app.MapPost("/api/attendance/create", AttendanceController.CreateAttendance);
app.MapPost("/api/attendance/get", AttendanceController.GetAttendance);
app.MapPost("/api/attendance/find", AttendanceController.FindAttendance);
app.MapPost("/api/attendance/update", AttendanceController.UpdateAttendance);
app.MapPost("/api/attendance/delete", AttendanceController.DeleteAttendance);

// Payroll Endpoints
app.MapPost("/api/payroll/create", PayrollController.CreatePayroll);
app.MapPost("/api/payroll/get", PayrollController.GetPayroll);
app.MapPost("/api/payroll/find", PayrollController.FindPayroll);
app.MapPost("/api/payroll/delete", PayrollController.DeletePayroll);

app.Run();

