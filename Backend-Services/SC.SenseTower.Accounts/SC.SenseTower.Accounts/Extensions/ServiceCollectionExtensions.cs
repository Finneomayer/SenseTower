using FluentValidation;
using MediatR;
using Polly;
using Polly.Extensions.Http;
using SC.SenseTower.Accounts.RabbitMQ.UserDelete;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Accounts.Settings;
using SC.SenseTower.Accounts.Settings.RabbitMQ;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using SC.SenseTower.Common.Validators;
using System.Reflection;

namespace SC.SenseTower.Accounts.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<IdentityServerSettings>(configuration.GetSection(nameof(IdentityServerSettings)));
            services.Configure<ServiceEndpointsSettings>(configuration.GetSection(nameof(ServiceEndpointsSettings)));
            services.Configure<IS4Configuration>(configuration.GetSection(nameof(IS4Configuration)));

            services.Configure<UserDeleteBindingSettings>(configuration.GetSection(nameof(UserDeleteBindingSettings)));

            return services;
        }

        public static IServiceCollection AddHttpServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var settings = configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();
            services.AddScoped<IdentityService>();
            services.AddHttpClient<IdentityService>(client =>
            {
                client.BaseAddress = new Uri(settings.BaseUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(settings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(settings.BreakAfter, TimeSpan.FromSeconds(settings.BreakForSeconds)));
            var endpoints = configuration.GetSection(nameof(ServiceEndpointsSettings)).Get<ServiceEndpointsSettings>();
            services.AddScoped<PlacesService>();
            services.AddHttpClient<PlacesService>(client =>
            {
                client.BaseAddress = new Uri(endpoints.MyPlacesRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(settings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(settings.BreakAfter, TimeSpan.FromSeconds(settings.BreakForSeconds)));
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<WalletsService>();
            services.AddScoped<InvitesService>();
            services.AddScoped<TicketsService>();
            services.AddScoped<AccountsService>();
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

            return services;
        }
    }
}
