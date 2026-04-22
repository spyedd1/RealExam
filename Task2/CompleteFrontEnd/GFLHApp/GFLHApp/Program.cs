using GFLHApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Add role support to Identity
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
});

var app = builder.Build();


// Seed the database with initial data (users and roles)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider; // Get the service provider for the current scope
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>(); // Get the role manager service
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>(); // Get the user manager and role manager services
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Get the database context
    await SeedData.SeedUsersAndRoles(services, userManager, roleManager); // Seed users and roles
    await SeedData.SeedProducers(services); // Seed producers
    await SeedData.SeedProducts(services); // Seed products
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
