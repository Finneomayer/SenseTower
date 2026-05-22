using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using YcAutomation.GithubBackups.Services.EmailSender.Dto;
using YcAutomation.GithubBackups.Services.EmailSender.Interfaces;

namespace YcAutomation.GithubBackups.Services.EmailSender
{
    public class EmailSenderService
    {
        private readonly JsonSerializerOptions jsonOptions;

        private const string ENV_LIST_ID = "ContactListId";
        private const string ENV_SENDER_EMAIL = "SenderEmail";
        private const string ENV_SEND_EMAIL_URL = "SendEmailUrl";
        private const string ENV_MAILER_ROOT_URL = "MailerRootUrl";
        private const string ENV_MAILER_API_KEY = "MailerApiKey";

        private readonly long listId;
        private readonly string senderEmail;
        private readonly string sendEmailUrl;
        private readonly string mailerRootUrl;
        private readonly string apiKey;

        public EmailSenderService()
        {
            jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            if (!long.TryParse(Environment.GetEnvironmentVariable(ENV_LIST_ID), out listId))
                listId = 2;
            senderEmail = Environment.GetEnvironmentVariable(ENV_SENDER_EMAIL) ?? "ivar.l@sensecapital.vc";
            sendEmailUrl = Environment.GetEnvironmentVariable(ENV_SEND_EMAIL_URL) ?? "sendEmail";
            mailerRootUrl = Environment.GetEnvironmentVariable(ENV_MAILER_ROOT_URL) ?? "https://api.unisender.com/ru/api/";
            apiKey = Environment.GetEnvironmentVariable(ENV_MAILER_API_KEY) ?? "6nsrnpg5fqyhn8o9coa1dowwkgm5skq18ihz1rho";
        }

        public async Task SendEmailNotification(string email, string subject, string text)
        {
            var payload = new PersonalEmail
            {
                api_key = apiKey,
                email = email,
                subject = subject,
                body = text,
                list_id = listId,
                sender_email = senderEmail,
                sender_name = "SenseTower"
            };
            Console.WriteLine("Отправляется уведомление {0}", JsonSerializer.Serialize(payload));
            var result = await Post<PersonalEmailResponse[]>(sendEmailUrl, payload);
            if (result == null)
                return;
            if (result.Any(r => (r.Errors?.Length ?? 0) > 0))
            {
                var errors = result
                    .Where(r => (r.Errors?.Length ?? 0) > 0)
                    .GroupBy(r => r.Email)
                    .Select(r => $"{r.Key}: {string.Join("; ", r.SelectMany(e => e.Errors).Select(e => $"{e.Message} ({e.Code})"))}")
                    .ToArray();
                Console.WriteLine(string.Join("\n", errors));
            }
        }

        private async Task<T> Post<T>(string url, object data)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(mailerRootUrl);
            if (!httpClient.DefaultRequestHeaders.Accept.Any(r => r.MediaType == "application/json"))
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = data != null ? new FormUrlEncodedContent(ToDictionary(data)) : null;
            var formData = await content.ReadAsStringAsync();
            var response = await httpClient.PostAsync(url, content);
            var result = await GetResponseContent<T>(response);
            return result;
        }

        private async Task<T> GetResponseContent<T>(HttpResponseMessage response)
        {
            if (response == null)
            {
                Console.WriteLine("Не получен ответ с удаленного ресурса");
                return default;
            }

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
                Console.WriteLine("Ошибка разбора ответа сервера рассылки: {0}", ex.Message);
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
