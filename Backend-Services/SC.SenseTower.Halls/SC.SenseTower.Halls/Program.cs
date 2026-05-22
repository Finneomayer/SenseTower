using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.OpenApi.Models;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.HttpClientLogging;
using SC.SenseTower.Common.Middlewares;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Halls.Data;
using SC.SenseTower.Halls.Dto;
using SC.SenseTower.Halls.Extensions;
using SC.SenseTower.Halls.Settings;
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
    });

services.AddDatabaseContext<HallsDbContext>();

services.AddAutoMapper(new[]
{
    typeof(MappingProfile),
    typeof(SC.SenseTower.Halls.Cqrs.MappingProfile),
    typeof(SC.SenseTower.Halls.RabbitMQ.MappingProfile)
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
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Halls API", Version = "v1" });

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
    o.SwaggerEndpoint("swagger/v1/swagger.json", "Halls API v1");
    o.RoutePrefix = string.Empty;
});

app.MapControllers();

app.UseRabbitMQ();

app.Run();
