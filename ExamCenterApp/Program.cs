using ExamCenterApp.Database;
using ExamCenterApp.Helpers;
using ExamCenterApp.MiddleWare;
using ExamCenterApp.Models;
using ExamCenterApp.Services;
using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using System;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IEmail_Configuration>(builder.Configuration.GetSection("Email_Configuration").Get<Email_Configuration>());
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("ExamCenterAppHangFire")));

builder.Services.AddScoped<IUser_Helper, User_Helper>();
builder.Services.AddScoped<IEmail_Sender , Email_Sender>();
builder.Services.AddHttpContextAccessor();
GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Handle SSL certificate validation issues here
    var sqlConnectionBuilder = new SqlConnectionStringBuilder(connectionString);
    sqlConnectionBuilder.Encrypt = true; // This enables SSL encryption
    sqlConnectionBuilder.TrustServerCertificate = true; // Set to true if you want to trust all certificates (not recommended for production)

    options.UseSqlServer(sqlConnectionBuilder.ConnectionString);
});
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddIdentity<Application_Users,IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

}).AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
    x.MultipartHeadersLengthLimit = int.MaxValue;
});
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


var app = builder.Build();
using (var scope= app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services).Wait();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
UpdateDatabase(app);
HangFireConfiguration(app);


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
static void UpdateDatabase(IApplicationBuilder app)
{
    using (var serviceScope = app.ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope())
    {
        using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
        {
            context?.Database.Migrate();
        }
    }
}

void HangFireConfiguration(IApplicationBuilder app)
{
    var robotDashboardOptions = new DashboardOptions { Authorization = new[] { new MyAuthorizationFilter() } };
    //Enable Session.

    var robotOptions = new BackgroundJobServerOptions
    {
        ServerName = String.Format(
        "{0}.{1}",
        Environment.MachineName,
        Guid.NewGuid().ToString())
    };
    app.UseHangfireServer(robotOptions);
    var RobotStorage = new SqlServerStorage(builder.Configuration.GetConnectionString("ExamCenterAppHangFire"));
    JobStorage.Current = RobotStorage;
    app.UseHangfireDashboard("/ExamCenterApp", robotDashboardOptions, RobotStorage);
}
// This method delays successful and failed jobs on the hangfire dashboard  for 1 month(30 Days) 
class ExpirationTimeAttribute : JobFilterAttribute, IApplyStateFilter
{
    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        context.JobExpirationTimeout = TimeSpan.FromDays(30);
    }

    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        context.JobExpirationTimeout = TimeSpan.FromDays(30);
    }
}
class MyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var user = context.GetHttpContext().User;
        if (user != null && user.Identity.IsAuthenticated && user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Exam Manager"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
