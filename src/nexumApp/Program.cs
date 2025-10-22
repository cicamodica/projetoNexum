using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddErrorDescriber<DuplicateUserDescriber>();

builder.Services.AddControllersWithViews();

// Filtro para não permitir acesso ao perfil enquanto cadastro não é aprovado
builder.Services.AddScoped<RequireOngApproved>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Ong", p => p.RequireRole("Ong"));
});

builder.Services.AddRazorPages(options =>
{
        options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage", "Ong");

       
        options.Conventions.AddAreaFolderApplicationModelConvention(
            "Identity",
            "/Account/Manage",
            model => model.Filters.Add(new ServiceFilterAttribute(typeof(RequireOngApproved)))
        );

        options.Conventions.AddAreaFolderApplicationModelConvention(
            "Identity",
            "/Account",
            model => model.Filters.Add(new ServiceFilterAttribute(typeof(RequireOngApproved)))
        );
});


builder.Services.AddSingleton<IAuthorizationHandler, OngIsOwnerRequirementHandler>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "Ong"};
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
