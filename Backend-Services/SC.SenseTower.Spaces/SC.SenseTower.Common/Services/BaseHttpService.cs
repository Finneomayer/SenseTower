using AutoMapper;
using Flurl;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Extensions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SC.SenseTower.Common.Services
{
    public class BaseHttpService : BaseService, IBaseHttpService
    {
        protected readonly HttpClient httpClient;

        public BaseHttpService(
            ILogger<BaseHttpService> logger,
            IMapper mapper,
            HttpClient httpClient) : base(logger, mapper)
        {
            this.httpClient = httpClient;
        }

        public async Task<T?> Get<T>(string? token, string url, object? data, CancellationToken cancellationToken)
        {
            httpClient.SetBearerToken(token);
            if (httpClient.DefaultRequestHeaders.Accept.All(r => r.MediaType != "application/json"))
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (data != null)
                url = url.SetQueryParams(data);
            url = url.SetQueryParam("_", DateTime.Now.Ticks);
            var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            return await GetResponseContent<T>(response);
        }

        public async Task<T?> Post<T>(string? token, string url, object? data, CancellationToken cancellationToken)
        {
            httpClient.SetBearerToken(token);
            if (httpClient.DefaultRequestHeaders.Accept.All(r => r.MediaType != "application/json"))
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = data != null ? new FormUrlEncodedContent(data.ToDictionary()) : null;
            var response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
            return await GetResponseContent<T>(response);
        }

        public async Task<T?> PostAsJson<T>(string? token, string url, string data, CancellationToken cancellationToken)
        {
            httpClient.SetBearerToken(token);
            if (httpClient.DefaultRequestHeaders.Accept.All(r => r.MediaType != "application/json"))
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content, cancellationToken);
            return await GetResponseContent<T>(response);
        }

        private static async Task<T?> GetResponseContent<T>(HttpResponseMessage? response)
        {
            if (response == null)
                throw new Exception("Не получен ответ с удаленного ресурса");

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new ScException(text);
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                var result = JsonSerializer.Deserialize<T?>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                return result;
            }
            catch
            {
                return default;
            }
        }
    }
}
