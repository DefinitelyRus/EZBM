using Microsoft.EntityFrameworkCore;
using Backend.Core.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Tell ASP.NET Core to look for the API controller classes within Backend.DesktopHost so they can handle incoming HTTP requests.
builder.Services.AddControllers();

// Automatically read the code and generate a visual test page (Swagger UI) for the API endpoints.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Creates a runnable web app instance using the configurations provided above.
WebApplication app = builder.Build();

// Turn on the dev-only Swagger UI and docs if in development mode.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Automatically redirect any plain HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Look inside the `wwwroot` folder and serve static files (HTML, CSS, images, etc.)
app.UseStaticFiles();
// What happens when this isn't here? What's the alternative?

// Check if users have permission to access specific endpoints.
app.UseAuthorization();

// Connect the incoming HTTP request URLs to the right C# methods.
app.MapControllers();

// Start the web server and begin listening for incoming HTTP requests.
app.Run();