using MediatR;
using Octokit.Webhooks.AspNetCore;
using SC.SenseTower.Common.Middlewares;
using SC.SenseTower.Utilities.Dto;
using SC.SenseTower.Utilities.Extensions;
using SC.SenseTower.Utilities.Settings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<ServiceEndpointsSettings>(configuration.GetSection(nameof(ServiceEndpointsSettings)));

services.AddAutoMapper(new Type[]
{
    typeof(MappingProfile)
});
services.AddMediatR(Assembly.GetCallingAssembly());
services.AddHttpServices(configuration);
services.AddValidators();
services.AddApplicationServices();

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var parsedPort))
{
    app.Urls.Add($"http://0.0.0.0:{parsedPort}/");
}

app.UseMiddleware<HttpRequestLogging>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGitHubWebhooks();
});

app.Run();
