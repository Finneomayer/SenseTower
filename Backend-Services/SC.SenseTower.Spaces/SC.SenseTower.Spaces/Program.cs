using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.OpenApi.Models;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.HttpClientLogging;
using SC.SenseTower.Common.Middlewares;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Spaces.Data;
using SC.SenseTower.Spaces.Dto;
using SC.SenseTower.Spaces.Extensions;
using SC.SenseTower.Spaces.Settings;
using SC.SenseTower.Spaces.Settings.RabbitMQ;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<ServiceEndpointsSettings>(configuration.GetSection(nameof(ServiceEndpointsSettings)));
var conf = configuration.GetSection(nameof(SpaceServiceSettings));
services.Configure<SpaceServiceSettings>(configuration.GetSection(nameof(SpaceServiceSettings)));

services.Configure<UserDeleteBindingSettings>(configuration.GetSection(nameof(UserDeleteBindingSettings)));

var isConfig = configuration.GetSection(nameof(IS4Configuration)).Get<IS4Configuration>();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
    {
        o.Authority = isConfig.Authority;
        o.Audience = isConfig.Audience;
        o.RequireHttpsMetadata = false;
    });

services.Configure<MongoDbConfig>(configuration.GetSection(nameof(MongoDbConfig)));
services.AddDatabaseContext<SpacesDbContext>();

services.AddAutoMapper(typeof(MappingProfile), typeof(SC.SenseTower.Spaces.Cqrs.MappingProfile));
services.AddMediatR(Assembly.GetCallingAssembly());
services.AddHttpServices();
services.AddValidators();
services.AddHttpClients(configuration);
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
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Spaces API", Version = "v1" });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    o.IgnoreObsoleteActions();
    o.IgnoreObsoleteProperties();

    //o.OperationFilter<SecurityRequirementsOperationFilter>();
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "",
    });

    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    //o.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
});

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var parsedPort))
    app.Urls.Add($"http://0.0.0.0:{parsedPort}/");

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
    o.SwaggerEndpoint("swagger/v1/swagger.json", "Spaces API v1");
    o.RoutePrefix = string.Empty;
});

app.MapControllers();

app.UseRabbitMQ();

app.Run();
