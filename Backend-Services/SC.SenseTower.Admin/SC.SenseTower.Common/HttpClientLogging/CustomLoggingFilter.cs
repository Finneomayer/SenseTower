using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SC.SenseTower.Common.HttpClientLogging
{
    public class CustomLoggingFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly ILogger<CustomLoggingFilter> logger;
        public CustomLoggingFilter(IWebHostEnvironment hostEnvironment, ILogger<CustomLoggingFilter> logger)
        {
            this.hostEnvironment = hostEnvironment;
            this.logger = logger;
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            return builder =>
            {
                next(builder);
                
                builder.AdditionalHandlers.Insert(0, new HttpLoggingHandler(hostEnvironment, logger));
            };
        }
    }
}