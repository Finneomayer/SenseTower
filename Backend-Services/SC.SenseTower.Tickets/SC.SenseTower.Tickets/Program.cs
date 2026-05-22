using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.OpenApi.Models;
using SC.SenseTower.Tickets.Data;
using SC.SenseTower.Tickets.Dto;
using SC.SenseTower.Tickets.Extensions;
using SC.SenseTower.Tickets.Settings;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.HttpClientLogging;
using SC.SenseTower.Common.Middlewares;
using SC.SenseTower.Common.Models;
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

services.Configure<MongoDbConfig>(configuration.GetSection(nameof(MongoDbConfig)));
services.AddDatabaseContext<TicketsDbContext>();

services.AddAutoMapper(new[]
{
    typeof(MappingProfile),
    typeof(SC.SenseTower.Tickets.Cqrs.MappingProfile)
});
services.AddMediatR(Assembly.GetCallingAssembly());
services.AddHttpServices(configuration);
services.AddValidators();
services.AddApplicationServices();
services.AddRabbitMQ(configuration);

services.AddCors();
services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

services.TryAddEnumerable(
    ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, CustomLoggingFilter>());

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Tickets API", Version = "v1" });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    o.IgnoreObsoleteActions();
    o.IgnoreObsoleteProperties();

    o.OperationFilter<SecurityRequirementsOperationFilter>();
    o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "Стандартный заголовок авторизации, использующий JWT.",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Type = SecuritySchemeType.Http
    });
});

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var parsedPort))
{
    app.Urls.Add($"http://0.0.0.0:{parsedPort}/");
}

app.UseMiddleware<HttpRequestLogging>();

app.UseCors(o =>
    o.AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(a => true));
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("swagger/v1/swagger.json", "Tickets API v1");
    o.RoutePrefix = string.Empty;
});

app.MapControllers();

app.UseRabbitMQ();

app.Run();
