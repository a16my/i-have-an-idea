using BirFikrimVar.Data;
using BirFikrimVar.Models; 
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity setup
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    // Disable email confirmation
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddRazorPages();

// ✅ Configure cookie paths (must be here, not inside MapGet)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

// Middleware
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

// Default redirect for "/"
app.MapGet("/", context =>
{
    if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
    {
        context.Response.Redirect("/Posts");
    }
    else
    {
        context.Response.Redirect("/Identity/Account/Login");
    }
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); 

app.Run();