using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDbGenericRepository;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Data.Contexts;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Extensions;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<IS4Configuration>(configuration.GetSection(nameof(IS4Configuration)));
services.Configure<ServiceEndpointsSettings>(configuration.GetSection(nameof(ServiceEndpointsSettings)));
services.Configure<AccountsDbConfig>(configuration.GetSection(nameof(AccountsDbConfig)));
services.Configure<IdentityDbConfig>(configuration.GetSection(nameof(IdentityDbConfig)));
services.Configure<PlacesDbConfig>(configuration.GetSection(nameof(PlacesDbConfig)));
services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
services.AddDatabaseContext<IdentityDbContext>();
services.AddDatabaseContext<AccountsDbContext>();

var identityDbConfig = configuration.GetSection(nameof(IdentityDbConfig)).Get<IdentityDbConfig>();
var settings = MongoClientSettings.FromConnectionString(identityDbConfig.ConnectionString);
var client = new MongoClient(settings);
var mongoDbContext = new MongoDbContext(client, identityDbConfig.DatabaseName);
services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
{
    o.User.RequireUniqueEmail = true;
    o.Password.RequireDigit = true;
    o.Password.RequiredLength = 12;
    o.Password.RequireLowercase = true;
    o.Password.RequireNonAlphanumeric = true;
    o.Password.RequireUppercase = true;
})
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(mongoDbContext);
services.AddAuthorization(o =>
{
    o.AddPolicy("OnlyAdmins", p => p.RequireRole(RoleNames.VR_ADMIN));
    o.DefaultPolicy = o.GetPolicy("OnlyAdmins") ?? new AuthorizationPolicy(Array.Empty<IAuthorizationRequirement>(), Array.Empty<string>());
});
var rootUrl = configuration.GetValue<string>(ConfigKeys.STATIC_ROOT_URL);
if (rootUrl.IndexOf("://") > 0)
{
    rootUrl = rootUrl[(rootUrl.IndexOf("://") + 3)..];
    rootUrl = rootUrl[rootUrl.IndexOf("/")..];
    if (rootUrl.Last() != '/')
        rootUrl += '/';
}
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = rootUrl + "account/logon";
        o.AccessDeniedPath = rootUrl + "account/accessdenied";
    });
services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, o =>
    {
        o.LoginPath = rootUrl + "account/logon";
        o.AccessDeniedPath = rootUrl + "account/accessdenied";
    });

services.AddAutoMapper(new[]
{
    typeof(MappingProfile),
});
services.AddMediatR(Assembly.GetCallingAssembly());
services.AddHttpServices(configuration);
services.AddValidators();
services.AddApplicationServices();

services.AddDistributedMemoryCache();
services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromHours(12);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

services.AddCors();
services
    .AddControllersWithViews()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(rootUrl + "Home/Error");
}

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var parsedPort))
{
    app.Urls.Add($"http://0.0.0.0:{parsedPort}/");
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors(o =>
{
    o.AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(a => true)
        .AllowCredentials();
});
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
