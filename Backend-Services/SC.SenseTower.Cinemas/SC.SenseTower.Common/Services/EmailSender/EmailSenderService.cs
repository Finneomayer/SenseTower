using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.EmailSender.Dto;
using SC.SenseTower.Common.Services.EmailSender.Interfaces;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using SC.SenseTower.Common.Exceptions;

namespace SC.SenseTower.Common.Services.EmailSender
{
    public class EmailSenderService : BaseHttpService
    {
        private readonly MailerSettings settings;

        private readonly JsonSerializerOptions jsonOptions;

        public EmailSenderService(
            ILogger<EmailSenderService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<MailerSettings> options) : base(logger, mapper, httpClient)
        {
            settings = options.Value;
            jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        public async Task SendEmail(string email, string subject, string text, CancellationToken cancellationToken)
        {
            var payload = new PersonalEmail
            {
                email = email,
                subject = subject,
                body = text,
                list_id = settings.ContactListId,
                sender_email = settings.AddressFrom,
                sender_name = settings.NameFrom
            };
            var result = await Post<PersonalEmailResponse[]>(settings.SendEmailUrl, payload, cancellationToken);
            if (result == null)
                return;
            if (result.Any(r => (r.Errors?.Length ?? 0) > 0))
            {
                var errors = result
                    .Where(r => (r.Errors?.Length ?? 0) > 0)
                    .GroupBy(r => r.Email)
                    .Select(r => $"{r.Key}: {string.Join("; ", r.SelectMany(e => e.Errors).Select(e => $"{e.Message} ({e.Code})"))}")
                    .ToArray();
                var errorMessage = string.Join("\n", errors);
                Logger.LogError(errorMessage);
                throw new ScException(errorMessage);
            }
        }

        private async Task<T?> Post<T>(string url, object data, CancellationToken cancellationToken)
        {
            ((IBaseRequest)data).api_key = settings.ApiKey;
            if (!httpClient.DefaultRequestHeaders.Accept.Any(r => r.MediaType == "application/json"))
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new FormUrlEncodedContent(data.ToDictionary());
            var formData = await content.ReadAsStringAsync(cancellationToken);
            var response = await httpClient.PostAsync(url, content, cancellationToken);
            var result = await GetResponseContent<T>(response, cancellationToken);
            return result;
        }

        private async Task<T?> GetResponseContent<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response == null)
                throw new Exception("Не получен ответ с удаленного ресурса");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
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
                Logger.LogError(ex.Message);
                return default;
            }
        }
    }
}
