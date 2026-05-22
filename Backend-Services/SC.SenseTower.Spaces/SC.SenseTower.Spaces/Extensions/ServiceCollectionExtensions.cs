using FluentValidation;
using MediatR;
using Polly;
using Polly.Extensions.Http;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using SC.SenseTower.Common.Validators;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.RabbitMQ.UserDelete;
using SC.SenseTower.Spaces.Services;
using SC.SenseTower.Spaces.Settings;
using System.Reflection;

namespace SC.SenseTower.Spaces.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpServices(this IServiceCollection services)
        {
            services.AddScoped<PlacesService>();
            services.AddScoped<HallsService>();
            services.AddScoped<AccountsService>();
            services.AddScoped<ImagesService>();
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<ISpacesService, SpacesService>();
            services.AddSingleton<IUserLocationService, UserLocationService>();
            services.AddTransient<ISpaceService, SpaceServiceStub>();
            services.AddTransient<IBaseHttpService, BaseHttpService>();
            services.AddSingleton<UserInSpaceService>();
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }

        public static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration)
        {
            var urlSettings = configuration.GetSection(nameof(ServiceEndpointsSettings)).Get<ServiceEndpointsSettings>();
            var pollySettings = configuration.GetSection(nameof(HttpConnectionSettings)).Get<HttpConnectionSettings>();

            services.AddHttpClient<BaseHttpService>()
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));


            services.AddHttpClient<AccountsService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.AccountsRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));

            services.AddHttpClient<PlacesService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.MyPlacesRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));
            services.AddHttpClient<HallsService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.HallsRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));
            services.AddHttpClient<ImagesService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.ImagesRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));
            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<RabbitMQConnectionSettings>(configuration.GetSection(nameof(RabbitMQConnectionSettings)));
            services.AddSingleton<IRabbitMQService, RabbitMQService>();
            services.AddSingleton<RabbitMQReceiver>();

            services.AddSingleton<UserDeleteSubscriber>();

            return services;
        }
    }
}
