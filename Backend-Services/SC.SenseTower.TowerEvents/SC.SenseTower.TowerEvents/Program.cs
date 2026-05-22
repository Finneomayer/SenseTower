using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using SC.SenseTower.Accounts.Extensions;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Middlewares;
using SC.SenseTower.Common.Models;
using SC.SenseTower.TowerEvents.Data;
using SC.SenseTower.TowerEvents.Dto;
using SC.SenseTower.TowerEvents.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddConfigurations(configuration);

var isConfig = configuration.GetSection(nameof(IS4Configuration)).Get<IS4Configuration>();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
    {
        o.Authority = isConfig.Authority;
        o.Audience = isConfig.Audience;
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters.ValidIssuer = isConfig.Authority;
        o.TokenValidationParameters.ValidAudience = isConfig.Audience;
        o.TokenValidationParameters.ValidateAudience = true;
        o.TokenValidationParameters.ValidateIssuer = true;
        o.TokenValidationParameters.ValidateActor = false;
        o.TokenValidationParameters.ValidateIssuerSigningKey = false;
        o.TokenValidationParameters.ValidateLifetime = true;
        o.TokenValidationParameters.ValidateTokenReplay = false;
        o.TokenValidationParameters.RequireSignedTokens = false;
    });

services.AddDatabaseContext<TowerEventsDbContext>();

services.AddAutoMapper(new[]
{
    typeof(MappingProfile),
    typeof(SC.SenseTower.TowerEvents.Cqrs.MappingProfile)
});
services.AddMediatR(Assembly.GetCallingAssembly());
services.AddHttpServices(configuration);
services.AddApplicationServices();
services.AddValidators();
services.AddRabbitMQ(configuration);

services.AddMemoryCache();

services.AddCors();
services.AddControllersWithViews()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(o =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    o.IgnoreObsoleteActions();
    o.IgnoreObsoleteProperties();

    o.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    o.AddSecurityDefinition("Токен", new OpenApiSecurityScheme
    {
        Description = "Стандартный заголовок авторизации, использующий JWT.",
        In = ParameterLocation.Header,
        Name = "Bearer",
        Scheme = "bearer",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT"
    });
});

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var parsedPort))
{
    app.Urls.Add($"http://0.0.0.0:{parsedPort}/");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<HttpRequestLogging>();

app.UseStaticFiles();

app.UseRouting();

app.UseCors(o =>
    o.AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(a => true));
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("swagger/v1/swagger.json", "Tower Events API v1");
    o.RoutePrefix = string.Empty;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseRabbitMQ();

app.Run();
