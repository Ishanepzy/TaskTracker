using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskTracker.Models;
using TaskTracker.Services;
using TaskTracker.Views.Shared.Components.CompletedTasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ToDoListDbContext>(options =>
{
    options.UseSqlServer("Server=localhost;Database=TasktryDb;Trusted_Connection=True;TrustServerCertificate=True;");
});

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole(options =>
    {
        options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
    });
    loggingBuilder.AddDebug();
    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
});

builder.Services.AddTransient<TaskPrioritizer>();
builder.Services.AddTransient<CompletedTasks>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ToDoListDbContext>()
    .AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure cookies are sent only over HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict; // Set SameSite policy
    options.Cookie.HttpOnly = true; // Prevent JavaScript access to cookies
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();