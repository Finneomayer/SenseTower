using Microsoft.Extensions.DependencyInjection;

namespace SC.SenseTower.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseContext<T>(this IServiceCollection services) where T : class
        {
            services.AddScoped<T>();
            return services;
        }
    }
}
