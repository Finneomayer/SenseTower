using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SC.SenseTower.Common.HttpClientLogging
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly ILogger logger;
        private readonly IWebHostEnvironment hostEnvironment;
        public HttpLoggingHandler(IWebHostEnvironment hostEnvironment, ILogger logger)
        {
            this.hostEnvironment = hostEnvironment;
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            if (requestMessage == null)
            {
                throw new ArgumentNullException(nameof(requestMessage));
            }

            logger.LogInformation($"Http Request: {(hostEnvironment.IsDevelopment() ? requestMessage.ToString() : requestMessage.ToString().RemoveHeader("Authorization"))}");

            if (requestMessage.Content != null)
            {
                logger.LogInformation(await requestMessage.Content.ReadAsStringAsync(cancellationToken));
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var responseMessage = await base.SendAsync(requestMessage, cancellationToken);
            sw.Stop();

            logger.LogInformation($"Http Response: {responseMessage}");
            logger.LogInformation(await responseMessage.Content.ReadAsStringAsync(cancellationToken));
            logger.LogInformation($"Completed in {sw.ElapsedMilliseconds} ms");

            return responseMessage;
        }
    }
}