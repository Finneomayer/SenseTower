using FluentValidation;
using MediatR;
using Polly;
using Polly.Extensions.Http;
using SC.SenseTower.Cinemas.RabbitMQ.UserDelete;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Cinemas.Settings;
using SC.SenseTower.Cinemas.Settings.RabbitMQ;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Validators;
using System.Reflection;
using SC.SenseTower.Cinemas.RabbitMQ.SpaceUpdate;
using SC.SenseTower.Cinemas.RabbitMQ.SpaceDelete;

namespace SC.SenseTower.Cinemas.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<ServiceEndpointsSettings>(configuration.GetSection(nameof(ServiceEndpointsSettings)));
            services.Configure<IS4Configuration>(configuration.GetSection(nameof(IS4Configuration)));
            services.Configure<MongoDbConfig>(configuration.GetSection(nameof(MongoDbConfig)));

            services.Configure<UserDeleteBindingSettings>(configuration.GetSection(nameof(UserDeleteBindingSettings)));
            services.Configure<SpaceUpdateBindingSettings>(configuration.GetSection(nameof(SpaceUpdateBindingSettings)));
            services.Configure<SpaceDeleteBindingSettings>(configuration.GetSection(nameof(SpaceDeleteBindingSettings)));

            return services;
        }

        public static IServiceCollection AddHttpServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var urlSettings = configuration.GetSection(nameof(ServiceEndpointsSettings)).Get<ServiceEndpointsSettings>();
            var pollySettings = configuration.GetSection(nameof(HttpConnectionSettings)).Get<HttpConnectionSettings>();
            services.AddScoped<SpacesService>();
            services.AddHttpClient<SpacesService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.SpacesRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));
            services.AddScoped<AccountsService>();
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
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<CinemasService>();
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<RabbitMQConnectionSettings>(configuration.GetSection(nameof(RabbitMQConnectionSettings)));
            services.AddSingleton<RabbitMQSender>();
            services.AddSingleton<RabbitMQReceiver>();

            services.AddSingleton<UserDeleteSubscriber>();
            services.AddSingleton<SpaceUpdateSubscriber>();
            services.AddSingleton<SpaceDeleteSubscriber>();

            return services;
        }
    }
}
