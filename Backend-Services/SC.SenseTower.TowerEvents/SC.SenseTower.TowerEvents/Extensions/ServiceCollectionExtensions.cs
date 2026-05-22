using FluentValidation;
using MediatR;
using Polly;
using Polly.Extensions.Http;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using SC.SenseTower.Common.Validators;
using SC.SenseTower.TowerEvents.RabbitMQ;
using SC.SenseTower.TowerEvents.RabbitMQ.SpaceDelete;
using SC.SenseTower.TowerEvents.RabbitMQ.SpaceUpdate;
using SC.SenseTower.TowerEvents.RabbitMQ.TicketBought;
using SC.SenseTower.TowerEvents.RabbitMQ.TotalTickets;
using SC.SenseTower.TowerEvents.RabbitMQ.UserDelete;
using SC.SenseTower.TowerEvents.Services;
using SC.SenseTower.TowerEvents.Settings;
using SC.SenseTower.TowerEvents.Settings.RabbitMQ;
using System.Reflection;

namespace SC.SenseTower.Accounts.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<ServiceEndpointsSettings>(configuration.GetSection(nameof(ServiceEndpointsSettings)));
            services.Configure<IS4Configuration>(configuration.GetSection(nameof(IS4Configuration)));
            services.Configure<MongoDbConfig>(configuration.GetSection(nameof(MongoDbConfig)));

            services.Configure<TicketBoughtBindingSettings>(configuration.GetSection(nameof(TicketBoughtBindingSettings)));
            services.Configure<TotalTicketsBindingSettings>(configuration.GetSection(nameof(TotalTicketsBindingSettings)));
            services.Configure<SpaceUpdateBindingSettings>(configuration.GetSection(nameof(SpaceUpdateBindingSettings)));
            services.Configure<SpaceDeleteBindingSettings>(configuration.GetSection(nameof(SpaceDeleteBindingSettings)));
            services.Configure<UserDeleteBindingSettings>(configuration.GetSection(nameof(UserDeleteBindingSettings)));

            return services;
        }

        public static IServiceCollection AddHttpServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var endpoints = configuration.GetSection(nameof(ServiceEndpointsSettings)).Get<ServiceEndpointsSettings>();
            var pollySettings = configuration.GetSection(nameof(HttpConnectionSettings)).Get<HttpConnectionSettings>();
            services.AddScoped<SpacesService>();
            services.AddHttpClient<SpacesService>(client =>
            {
                client.BaseAddress = new Uri(endpoints.SpacesRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));
            services.AddScoped<ImagesService>();
            services.AddHttpClient<ImagesService>(client =>
            {
                client.BaseAddress = new Uri(endpoints.ImagesRootUrl);
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
            services.AddScoped<TowerEventsService>();
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

            services.AddSingleton<TicketBoughtSubscriber>();
            services.AddSingleton<TotalTicketsSubscriber>();
            services.AddSingleton<SpaceUpdateSubscriber>();
            services.AddSingleton<SpaceDeleteSubscriber>();
            services.AddSingleton<UserDeleteSubscriber>();

            return services;
        }
    }
}
