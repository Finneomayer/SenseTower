using SC.SenseTower.Auth.Data;
using SC.SenseTower.Auth.Services.RabbitMQ;

namespace SC.SenseTower.Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
                try
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<InitialSeedService>();
                    seeder.SeedData().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger.LogError($"Seeding data error: {ex.Message}");
                }
            }

            try
            {
                var rmqService = host.Services.GetRequiredService<RabbitMQService>();
                rmqService.Start(default).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var logger = host.Services.GetService<ILogger<Program>>();
                if (logger != null)
                    logger.LogError(ex.Message);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    // yandex serverless containers specific
                    var port = Environment.GetEnvironmentVariable("PORT");
                    if (!string.IsNullOrEmpty(port) && ushort.TryParse(port, out var parsedPort))
                    {
                        webBuilder.UseUrls($"http://0.0.0.0:{parsedPort}/");
                    }
                });
    }
}
