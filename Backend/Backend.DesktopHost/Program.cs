using Backend.Core.Data;
using Backend.DesktopHost.Services;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

BusinessSettingsService settingsService = new();
builder.Services.AddSingleton(settingsService);

string resolvedDbPath = settingsService.GetResolvedDatabasePath();
string connectionString = $"Data Source={resolvedDbPath}";

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite(connectionString)
);

// Tell ASP.NET Core to look for the API controller classes within Backend.DesktopHost so they can handle incoming HTTP requests.
builder.Services.AddControllers();

builder.Services.AddScoped<ProductTransactionService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<PosService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<PayrollService>();

// Read the code and generate a visual test page (Swagger UI) for the API endpoints.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Create a runnable web app instance using the configurations provided above.
WebApplication app = builder.Build();

// Migrate the database on startup and ensure default admin user.
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    AuthService authService = scope.ServiceProvider.GetRequiredService<AuthService>();
    authService.EnsureAdminUserExistsAsync().GetAwaiter().GetResult();
}

// Turn on the dev-only Swagger UI and docs if in development mode.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP to HTTPs
app.UseHttpsRedirection();

// Serve default files (e.g., index.html) when visiting root
app.UseDefaultFiles();

// Look inside the `wwwroot` folder and serve static files (HTML, CSS, images, etc.)
app.UseStaticFiles();

// Connect URL paths to the controller methods configured above.
app.MapControllers();

// Start the server and keep it running to listen for incoming requests.
app.Run();
