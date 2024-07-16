using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using ToastNotification;
using ToastNotification.Extensions;
using Serilog.Formatting.Json;
using Serilog;
using ExpenseTracker.Data;
using Microsoft.AspNetCore.Authorization.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.File(new JsonFormatter(), @"Logs/log-.json", shared: true, rollingInterval: RollingInterval.Day)
    .CreateLogger();

//log service
builder.Host.UseSerilog();

// Dependecy Injection - parsing the Connection String for the DB

var connectionString = builder.Configuration.GetConnectionString("DevConnection") ?? throw new InvalidOperationException("Connections string not found");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<AppUser, IdentityRole>
    (options =>
    {
        //setare parametrii Identity
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;


    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders();


//Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
    options.Cookie.Name = "Tracker";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = new PathString("/Account/Login");
    options.ReturnUrlParameter = new PathString("/Dashboard/Index");
    options.SlidingExpiration = true;
});

//Claims
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Test", policy => policy.RequireClaim("Authentication", "Demo"));
});



// Notificari
builder.Services.AddNotyf(config =>
    {
        config.DurationInSeconds = 3; 
        config.IsDismissable = true; 
        config.Position = NotyfPosition.BottomRight;
    });


builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddMvc(o => {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); o.Filters.Add(new AuthorizeFilter(policy));
    }).AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();


// Add services to the container.
builder.Services.AddControllersWithViews();

//Register Syncfusion license
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2U1hhQlJBfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hTX5Vd0FjUHtZdHNTQ2ZZ");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseNotyf();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

try
{
    Log.Information("Starting web host");
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(exception: ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
