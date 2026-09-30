using Backend.Core.Data;
using Backend.DesktopHost.Services;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Tell ASP.NET Core to look for the API controller classes within Backend.DesktopHost so they can handle incoming HTTP requests.
builder.Services.AddControllers();

builder.Services.AddScoped<ProductTransactionService>();

// Read the code and generate a visual test page (Swagger UI) for the API endpoints.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Create a runnable web app instance using the configurations provided above.
WebApplication app = builder.Build();

// Migrate the database on startup.
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
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
// What happens when this isn't here? What's the alternative?

// Check if users have permission to access specific endpoints.
app.UseAuthorization();

// Connect the incoming HTTP request URLs to the right C# methods.
app.MapControllers();

// Start web server
app.Run();