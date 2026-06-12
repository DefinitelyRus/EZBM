using EZBM.Core.Data;
using EZBM.DesktopClient.Helpers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

// Initialize the database lifecycle (ensures schema exists on startup).
DbManager.Initialize();

using (AppDbContext context = new())
{
    DataSeeder.Seed();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
