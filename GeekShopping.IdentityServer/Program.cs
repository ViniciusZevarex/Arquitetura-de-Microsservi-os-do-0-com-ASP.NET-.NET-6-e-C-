using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Initializer;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<MySQLContext>(options => options.
    UseMySql(builder.Configuration["ConnectionStrings:MySQLConnectionString"],
                new MySqlServerVersion(new Version(8, 0, 29))));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MySQLContext>()
    .AddDefaultTokenProviders();

var builderIdentity = builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;
}).AddInMemoryIdentityResources(
    IdentityConfiguration.IdentityResources
).AddInMemoryClients(
    IdentityConfiguration.Clients
).AddInMemoryApiScopes(
    IdentityConfiguration.ApiScopes
)
.AddAspNetIdentity<ApplicationUser>();


builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builderIdentity.AddDeveloperSigningCredential();


builder.Services.AddControllersWithViews();

var app = builder.Build();
var scope = app.Services.CreateScope();

var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();


dbInitializer.Initialize();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
