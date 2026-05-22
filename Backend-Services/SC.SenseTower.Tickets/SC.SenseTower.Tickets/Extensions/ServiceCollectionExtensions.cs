using FluentValidation;
using MediatR;
using Polly;
using Polly.Extensions.Http;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using SC.SenseTower.Common.Validators;
using SC.SenseTower.Tickets.RabbitMQ;
using SC.SenseTower.Tickets.RabbitMQ.TicketsCreate;
using SC.SenseTower.Tickets.RabbitMQ.TowerEventDelete;
using SC.SenseTower.Tickets.RabbitMQ.TowerEventUpdate;
using SC.SenseTower.Tickets.RabbitMQ.UserDelete;
using SC.SenseTower.Tickets.Services;
using SC.SenseTower.Tickets.Settings;
using SC.SenseTower.Tickets.Settings.RabbitMQ;
using System.Reflection;

namespace SC.SenseTower.Tickets.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<ServiceEndpointsSettings>(configuration.GetSection(nameof(ServiceEndpointsSettings)));
            services.Configure<IS4Configuration>(configuration.GetSection(nameof(IS4Configuration)));
            services.Configure<MongoDbConfig>(configuration.GetSection(nameof(MongoDbConfig)));

            services.Configure<UserDeleteBindingSettings>(configuration.GetSection(nameof(UserDeleteBindingSettings)));
            services.Configure<TicketsCreateBindingSettings>(configuration.GetSection(nameof(TicketsCreateBindingSettings)));
            services.Configure<TowerEventDeleteBindingSettings>(configuration.GetSection(nameof(TowerEventDeleteBindingSettings)));
            services.Configure<TowerEventUpdateBindingSettings>(configuration.GetSection(nameof(TowerEventUpdateBindingSettings)));

            return services;
        }

        public static IServiceCollection AddHttpServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var urlSettings = configuration.GetSection(nameof(ServiceEndpointsSettings)).Get<ServiceEndpointsSettings>();
            var pollySettings = configuration.GetSection(nameof(HttpConnectionSettings)).Get<HttpConnectionSettings>();
            services.AddScoped<TowerEventsService>();
            services.AddHttpClient<TowerEventsService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.TowerEventsRootUrl);
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
            services.AddScoped<TicketsService>();
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
            services.AddSingleton<RabbitMQService>();
            services.AddSingleton<RabbitMQReceiver>();

            services.AddSingleton<UserDeleteSubscriber>();
            services.AddSingleton<TicketsCreateSubscriber>();
            services.AddSingleton<TowerEventUpdateSubscriber>();
            services.AddSingleton<TowerEventDeleteSubscriber>();

            return services;
        }
    }
}
