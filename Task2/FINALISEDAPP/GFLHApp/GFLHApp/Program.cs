// ----- Imports -----
using GFLHApp.Data; // Imports GFLHApp.Data types used by this file.
using Microsoft.AspNetCore.Identity; // Imports Microsoft.AspNetCore.Identity types used by this file.
using Microsoft.EntityFrameworkCore; // Imports Microsoft.EntityFrameworkCore types used by this file.

// ----- Application Builder -----
var builder = WebApplication.CreateBuilder(args); // Creates the ASP.NET Core application builder.

// ----- Database configuration -----
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."); // Reads the SQL Server connection string from configuration.
builder.Services.AddDbContext<ApplicationDbContext>(options => // Registers the application database context with dependency injection.
    options.UseSqlServer(connectionString)); // Configures EF Core to use SQL Server with the configured connection.
builder.Services.AddDatabaseDeveloperPageExceptionFilter(); // Adds detailed database error pages for development.

// ----- Identity configuration -----
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true) // Registers ASP.NET Core Identity with confirmed-account sign-in.
    .AddRoles<IdentityRole>() // Adds role support to the Identity system.
    // ----- Database configuration -----
    .AddEntityFrameworkStores<ApplicationDbContext>(); // Stores Identity data in the application database context.
builder.Services.AddControllersWithViews(); // Registers MVC controllers and Razor views.
// ----- Google authentication -----
builder.Services.AddAuthentication().AddGoogle(options => // Registers Google authentication for external login.
{
    options.ClientId = builder.Configuration["Google:ClientId"]; // Reads the Google OAuth client id from configuration.
    options.ClientSecret = builder.Configuration["Google:ClientSecret"]; // Reads the Google OAuth client secret from configuration.
});

var app = builder.Build(); // Builds the configured web application.


// ----- Imports -----
using (var scope = app.Services.CreateScope()) // Imports (var scope = app.Services.CreateScope()) types used by this file.
{
    // ----- Service scope -----
    var services = scope.ServiceProvider; // Resolves a required service from dependency injection.
    // ----- Identity configuration -----
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>(); // Resolves a required service from dependency injection.
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>(); // Resolves a required service from dependency injection.
    // ----- Database configuration -----
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Resolves a required service from dependency injection.
    // ----- Identity configuration -----
    await SeedData.SeedUsersAndRoles(services, userManager, roleManager); // Creates the default users and assigns their roles.
    // ----- Seed producers -----
    await SeedData.SeedProducers(services); // Creates the default producer records.
    // ----- Seed products -----
    await SeedData.SeedProducts(services); // Creates the default product records.
}

// ----- Request pipeline -----
if (app.Environment.IsDevelopment()) // Checks whether the app is running in development.
{
    app.UseMigrationsEndPoint(); // Enables the migrations endpoint during development.
}
else // Handles the fallback branch for the setup step.
{
    app.UseExceptionHandler("/Home/Error"); // Routes production errors to the shared error page.
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); // Enables HTTP Strict Transport Security for production.
}

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS.
app.UseRouting(); // Enables endpoint routing.

app.UseAuthorization(); // Applies authorization checks to incoming requests.

// ----- Route mapping -----
app.MapStaticAssets(); // Maps optimized static asset endpoints.

app.MapControllerRoute( // Maps the default MVC controller route.
    name: "default", // Runs this setup or seeding step.
    pattern: "{controller=Home}/{action=Index}/{id?}") // Sets the default controller, action, and optional id route pattern.
    .WithStaticAssets(); // Adds static asset support to the mapped endpoint.

app.MapRazorPages() // Maps Razor Pages, including Identity pages.
   .WithStaticAssets(); // Adds static asset support to the mapped endpoint.

app.Run(); // Starts the web application request loop.
