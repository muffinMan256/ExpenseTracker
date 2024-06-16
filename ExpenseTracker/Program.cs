using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);



// Dependecy Injection - parsing the Connection String for the DB

var connectionString = builder.Configuration.GetConnectionString("DevConnection") ?? throw new InvalidOperationException("Connections string not found");
builder.Services.AddDbContext<ExpenseTracker.Models.ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ExpenseTrackerUser, IdentityRole>
    (options =>
    {
        //setare parametrii Identity
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 5;
        options.Password.RequireUppercase = true;
        options.User.RequireUniqueEmail = true;

    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders();


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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
