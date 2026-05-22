using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.OpenApi.Models;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.HttpClientLogging;
using SC.SenseTower.Common.Middlewares;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Images.Data;
using SC.SenseTower.Images.Dto;
using SC.SenseTower.Images.Extensions;
using SC.SenseTower.Images.Settings;
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
        //o.TokenValidationParameters.RequireSignedTokens = false;
        //o.TokenValidationParameters.ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 };
    });

services.AddDatabaseContext<ImagesDbContext>();

services.AddAutoMapper(new[]
{
    typeof(MappingProfile)
});
services.AddMediatR(Assembly.GetCallingAssembly());
services.AddHttpServices(configuration);
services.AddApplicationServices();
services.AddValidators();
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

app.UseMiddleware<HttpRequestLogging>();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("swagger/v1/swagger.json", "Image Files API v1");
    o.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseCors(o =>
    o.AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(a => true));
app.UseAuthorization();

app.MapControllers();

app.UseRabbitMQ();

app.Run();
