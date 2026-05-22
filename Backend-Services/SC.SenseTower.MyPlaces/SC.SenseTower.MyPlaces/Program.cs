using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.OpenApi.Models;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.HttpClientLogging;
using SC.SenseTower.Common.Middlewares;
using SC.SenseTower.Common.Models;
using SC.SenseTower.MyPlaces.Data;
using SC.SenseTower.MyPlaces.Dto;
using SC.SenseTower.MyPlaces.Extensions;
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

services.AddDatabaseContext<MyPlacesDbContext>();

services.AddAutoMapper(new[]
{
    typeof(MappingProfile),
    typeof(SC.SenseTower.MyPlaces.RabbitMQ.MappingProfile)
});
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
    o.SwaggerEndpoint("swagger/v1/swagger.json", "User Places API v1");
    o.RoutePrefix = string.Empty;
});

app.MapControllers();

app.UseRabbitMQ();

app.Run();
