using FluentValidation;
using MediatR;
using Octokit.Webhooks;
using Polly;
using Polly.Extensions.Http;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services.EmailSender;
using SC.SenseTower.Common.Validators;
using SC.SenseTower.Utilities.Services;
using SC.SenseTower.Utilities.Settings;
using System.Reflection;

namespace SC.SenseTower.Utilities.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<WebhookEventProcessor, GitHubHookService>();
            services.AddScoped<RepositoryService>();
            return services;
        }

        public static IServiceCollection AddHttpServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var mailerSettings = configuration.GetSection(nameof(MailerSettings)).Get<MailerSettings>();
            services.AddScoped<EmailSenderService>();
            services.AddHttpClient<EmailSenderService>(client =>
            {
                client.BaseAddress = new Uri(mailerSettings.RootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(mailerSettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(mailerSettings.BreakAfter, TimeSpan.FromSeconds(mailerSettings.BreakForSeconds)));
            var urlSettings = configuration.GetSection(nameof(ServiceEndpointsSettings)).Get<ServiceEndpointsSettings>();
            var pollySettings = configuration.GetSection(nameof(HttpConnectionSettings)).Get<HttpConnectionSettings>();
            services.AddScoped<YandexVmService>();
            services.AddHttpClient<YandexVmService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.YandexComputeRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
