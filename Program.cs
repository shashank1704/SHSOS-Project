using Microsoft.EntityFrameworkCore;
using SHSOS.Data;
using SHSOS.Services;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================

// MVC
builder.Services.AddControllersWithViews();

// DbContext (SQL Server)
builder.Services.AddDbContext<SHSOSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SHSOS")));

// Services
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<PredictionService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<SustainabilityService>();

var app = builder.Build();

// ================= MIDDLEWARE =================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ================= DATA SEEDING =================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SHSOSDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// ================= ROUTING =================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();