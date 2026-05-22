using System.Net;
using System.Text;
using Newtonsoft.Json;
using SenseWebApi1.Features.EventFeature;

namespace SenseWebApi1.Services;

public class HttpService:IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpService> _logger;
    // ReSharper disable once NotAccessedField.Local
    private IHttpContextAccessor _contextAccessor;
    public HttpService(IHttpContextAccessor contextAccessor,ILogger<HttpService> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        
#pragma warning disable CS8602
        // ReSharper disable once RedundantSuppressNullableWarningExpression
        var jwt= contextAccessor.HttpContext.Request.Headers["Authorization"]!;
#pragma warning restore CS8602
        _httpClient.DefaultRequestHeaders.Add("Authorization","" +jwt);
        _contextAccessor = contextAccessor;
    }

    public async Task<string?> GetImages(Guid id)
    {
        var headerString = _httpClient.DefaultRequestHeaders.Aggregate("", (current, item) => current + $"{item.Key}={item.Value}\n");
        _logger.LogInformation("Request GET\n {0}   ", headerString);
        var response = await _httpClient.GetAsync(new Uri($"http://imageapi:80/images/{id}"));
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        var result =await response.Content.ReadAsStringAsync();
        return result;
       
    }

    public async Task<string?> GetSpaces(Guid id)
    {
        
        var headerString = _httpClient.DefaultRequestHeaders.Aggregate("", (current, item) => current + $"{item.Key}={item.Value}\n");
        _logger.LogInformation("Request GET\n {0}   ", headerString);
        var response = await _httpClient.GetAsync(new Uri($"http://spaceapi:80/spaces/{id}"));
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        var result =await response.Content.ReadAsStringAsync();
        return result;
    }

    public async Task<HttpResponseMessage> CreateTransaction(Guid ownerId,CancellationToken cancellationToken)
    {
        var headerString = _httpClient.DefaultRequestHeaders.Aggregate("", (current, item) => current + $"{item.Key}={item.Value}\n");
        _logger.LogInformation("Request POST\n {0}   ", headerString);
        var eventObj = new Event();
        var response = await _httpClient.PostAsync($"http://paymentapi:80/payment/{ownerId}",JsonContent.Create(eventObj),cancellationToken);
        return response;
        
    }
    public async Task<HttpResponseMessage> ConfirmTransaction(Guid id,CancellationToken cancellationToken)
    {
        var headerString = _httpClient.DefaultRequestHeaders.Aggregate("", (current, item) => current + $"{item.Key}={item.Value}\n");
        _logger.LogInformation("Request PUT\n {0}   ", headerString);
        var json = JsonConvert.SerializeObject("Confirmed");
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"http://paymentapi:80/payment/{id}/Confirmed",stringContent,cancellationToken);
        return response;
        
    }
    
    public async Task<HttpResponseMessage> CancelTransaction(Guid id,CancellationToken cancellationToken)
    {
        var headerString = _httpClient.DefaultRequestHeaders.Aggregate("", (current, item) => current + $"{item.Key}={item.Value}\n");
        _logger.LogInformation("Request PUT\n {0}   ", headerString);
        var json = JsonConvert.SerializeObject("Canceled");
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"http://paymentapi:80/payment/{id}/Canceled",stringContent,cancellationToken);
        return response;
        
    }
}