using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace SC.SenseTower.Common.Middlewares
{
    public class HttpRequestLogging
    {
        private readonly RequestDelegate next;
        private readonly ILogger<HttpRequestLogging> logger;
        private readonly IWebHostEnvironment env;

        public HttpRequestLogging(RequestDelegate next, IWebHostEnvironment hostEnvironment, ILogger<HttpRequestLogging> logger)
        {
            this.next = next;
            this.logger = logger;
            env = hostEnvironment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var sb = new StringBuilder();

            var info = $"[{request.Method}] {request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            sb.AppendLine($"Входящий запрос {context.TraceIdentifier}: {info}");
            foreach (var header in request.Headers.Where(r => r.Key != "Authorization"))
            {
                sb.AppendLine($"{header}");
            }

            if (request.HasFormContentType)
            {
                var form = string.Join('&', request.Form.Select(r => $"{r.Key}={r.Value}"));
                sb.AppendLine($"Данные формы: {form}");
            }
            else
            {
                var st = new MemoryStream();
                await request.Body.CopyToAsync(st);
                st.Position = 0;
                var sr = new StreamReader(st);
                if (request.ContentLength > 0)
                {
                    var inBody = sr.ReadToEnd();
                    sb.AppendLine($"Тело запроса: {inBody}");
                    st.Position = 0;
                }
                request.Body = st;
            }
            logger.LogInformation(sb.ToString());
            sb.Clear();

            var sw = new Stopwatch();
            sw.Start();
            await next(context);
            sw.Stop();

            var response = context.Response;
            sb.AppendLine($"Ответ на входящий запрос {context.TraceIdentifier}: {response.StatusCode}, {response.ContentType}");

            if (response.Body.CanRead)
            {
                var outst = new MemoryStream();
                await response.Body.CopyToAsync(outst);
                outst.Position = 0;
                var outsr = new StreamReader(outst);
                if (response.ContentLength > 0)
                {
                    var outBody = outsr.ReadToEnd();
                    sb.AppendLine($"Тело ответа: {outBody}");
                    outst.Position = 0;
                }
                response.Body = outst;
            }

            sb.AppendLine($"Запрос {context.TraceIdentifier} выполнен за {sw.ElapsedMilliseconds} мс");
            logger.LogInformation(sb.ToString());
        }
    }
}
