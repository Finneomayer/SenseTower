using FluentValidation;
using MediatR;
using Polly;
using Polly.Extensions.Http;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services.EmailSender;
using SC.SenseTower.Common.Validators;
using System.Reflection;

namespace SC.SenseTower.Admin.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
            services.AddScoped<IS4Service>();
            services.AddHttpClient<IS4Service>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.IS4RootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));
            services.AddScoped<AccountsHttpService>();
            services.AddHttpClient<AccountsHttpService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.AccountsRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));
            services.AddScoped<GalleriesService>();
            services.AddHttpClient<GalleriesService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.GalleriesRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));
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
            services.AddScoped<ImagesService>();
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
            services.AddScoped<TicketsService>();
            services.AddHttpClient<TicketsService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.TicketsRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(pollySettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(pollySettings.BreakAfter, TimeSpan.FromSeconds(pollySettings.BreakForSeconds)));
            services.AddScoped<PlacesService>();
            services.AddHttpClient<PlacesService>(client =>
            {
                client.BaseAddress = new Uri(urlSettings.PlacesRootUrl);
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
            services.AddScoped<IdentityService>();
            services.AddScoped<InvitesService>();
            services.AddScoped<GuestInvitesService>();
            services.AddScoped<AccountsService>();
            services.AddScoped<WalletsService>();
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
