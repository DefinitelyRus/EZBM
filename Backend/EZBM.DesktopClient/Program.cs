using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.DesktopClient.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<ICashRegisterService, MockCashRegisterService>();

WebApplication app = builder.Build();

DbManager.ConfigureFromArgs(args);
DbManager.Initialize();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) =>
{
    string? staffIdStr = context.Request.Cookies["ActiveStaffId"];
    if (ulong.TryParse(staffIdStr, out ulong staffId))
    {
        using AppDbContext db = new();
        Staff? staff = await db.Staff.FindAsync(staffId);
        if (staff is not null)
        {
            CurrentUserContext.Username = staff.Username;
        }
        else
        {
            CurrentUserContext.Username = "System";
        }
    }
    else
    {
        CurrentUserContext.Username = "System";
    }
    await next();
});

app.UseAuthorization();

app.MapRazorPages();

app.Run();
