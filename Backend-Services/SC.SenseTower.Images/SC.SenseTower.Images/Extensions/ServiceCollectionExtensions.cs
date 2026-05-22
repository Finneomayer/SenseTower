using FluentValidation;
using MediatR;
using Polly;
using Polly.Extensions.Http;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Validators;
using SC.SenseTower.Images.RabbitMQ.UserDelete;
using SC.SenseTower.Images.Services;
using SC.SenseTower.Images.Settings;
using SC.SenseTower.Images.Settings.RabbitMQ;
using System.Reflection;
using SC.SenseTower.Images.RabbitMQ.ImagesDelete;

namespace SC.SenseTower.Images.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<ServiceEndpointsSettings>(configuration.GetSection(nameof(ServiceEndpointsSettings)));
            services.Configure<IS4Configuration>(configuration.GetSection(nameof(IS4Configuration)));
            services.Configure<ImageSettings>(configuration.GetSection(nameof(ImageSettings)));
            services.Configure<StorageSettings>(configuration.GetSection(nameof(StorageSettings)));
            services.Configure<MongoDbConfig>(configuration.GetSection(nameof(MongoDbConfig)));

            services.Configure<UserDeleteBindingSettings>(configuration.GetSection(nameof(UserDeleteBindingSettings)));
            services.Configure<ImagesDeleteBindingSettings>(configuration.GetSection(nameof(ImagesDeleteBindingSettings)));

            return services;
        }

        public static IServiceCollection AddHttpServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            //var settings = configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();
            //services.AddScoped<IdentityService>();
            //services.AddHttpClient<IdentityService>(client =>
            //{
            //    client.BaseAddress = new Uri(settings.BaseUrl);
            //})
            //    .AddPolicyHandler(_ => HttpPolicyExtensions
            //        .HandleTransientHttpError()
            //        .WaitAndRetryAsync(settings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            //    .AddPolicyHandler(_ => HttpPolicyExtensions
            //        .HandleTransientHttpError()
            //        .CircuitBreakerAsync(settings.BreakAfter, TimeSpan.FromSeconds(settings.BreakForSeconds)));
            var endpoints = configuration.GetSection(nameof(ServiceEndpointsSettings)).Get<ServiceEndpointsSettings>();
            var httpSettings = configuration.GetSection(nameof(HttpConnectionSettings)).Get<HttpConnectionSettings>();
            services.AddScoped<PlacesService>();
            services.AddHttpClient<PlacesService>(client =>
            {
                client.BaseAddress = new Uri(endpoints.MyPlacesRootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(httpSettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(httpSettings.BreakAfter, TimeSpan.FromSeconds(httpSettings.BreakForSeconds)));
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<ImageProcessingService>();
            services.AddScoped<ImageStorageService>();
            services.AddScoped<ImageFilesService>();
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
            services.AddSingleton<ImagesDeleteSubscriber>();

            return services;
        }
    }
}
