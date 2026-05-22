using Microsoft.Extensions.Options;
using SC.SenseTower.Auth.Services.EmailSender.Dto;
using SC.SenseTower.Auth.Services.EmailSender.Interfaces;
using SC.SenseTower.Auth.Settings;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SC.SenseTower.Auth.Services.EmailSender
{
    public class EmailSenderService
    {
        private readonly ILogger<EmailSenderService> _logger;
        private readonly HttpClient _httpClient;
        private readonly MailerSettings _settings;

        private readonly JsonSerializerOptions jsonOptions;

        public EmailSenderService(
            ILogger<EmailSenderService> logger,
            HttpClient httpClient,
            IOptions<MailerSettings> options)
        {
            _logger = logger;
            _httpClient = httpClient;
            _settings = options.Value;
            jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        public async Task SendEmailConfirmation(string email, string userName, string subject, string text, CancellationToken cancellationToken)
        {
            var payload = new PersonalEmail
            {
                email = email,
                subject = subject,
                body = text,
                list_id = _settings.ContactListId,
                sender_email = _settings.AddressFrom,
                sender_name = _settings.NameFrom
            };
            var result = await Post<PersonalEmailResponse[]>(_settings.SendEmailUrl,payload, cancellationToken);
            if (result == null)
                return;
            if (result.Any(r => (r.Errors?.Length ?? 0) > 0))
            {
                var errors = result
                    .Where(r => (r.Errors?.Length ?? 0) > 0)
                    .GroupBy(r => r.Email)
                    .Select(r => $"{r.Key}: {string.Join("; ", r.SelectMany(e => e.Errors).Select(e => $"{e.Message} ({e.Code})"))}")
                    .ToArray();
                _logger.LogError(string.Join("\n", errors));
            }
        }

        private async Task<T> Post<T>(string url, object data, CancellationToken cancellationToken)
        {
            (data as IBaseRequest).api_key = _settings.ApiKey;
            if (_httpClient.DefaultRequestHeaders.Accept.All(r => r.MediaType != "application/json"))
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = data != null ? new FormUrlEncodedContent(ToDictionary(data)) : null;
            var formData = await content.ReadAsStringAsync(cancellationToken);
            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            var result = await GetResponseContent<T>(response);
            return result;
        }

        private async Task<T> GetResponseContent<T>(HttpResponseMessage response)
        {
            if (response == null)
                throw new Exception("Не получен ответ с удаленного ресурса");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(json, jsonOptions);
                if (apiResponse == null)
                    throw new Exception("Не получен ответ сервера рассылки.");
                if (apiResponse.ErrorCode != null)
                    throw new Exception(apiResponse.ErrorMessage);
                return apiResponse.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return default;
            }
        }

        private IDictionary<string, string> ToDictionary(object source, string parentName = "", BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var properties = source.GetType().GetProperties(bindingAttr);
            var result = properties.Where(r => !r.PropertyType.IsArray).ToDictionary
            (
                propInfo => parentName + propInfo.Name,
                propInfo => propInfo.GetValue(source, null)?.ToString()
            );
            foreach (var property in properties.Where(r => r.PropertyType.IsArray))
            {
                var i = 0;
                foreach (var value in property.GetValue(source) as Array)
                {
                    result.Add($"{parentName}{property.Name}[{i++}]", value.ToString());
                }
            }
            return result;
        }
    }
}
