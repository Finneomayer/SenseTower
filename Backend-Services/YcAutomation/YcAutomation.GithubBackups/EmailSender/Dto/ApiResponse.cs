using System.Text.Json.Serialization;

namespace YcAutomation.GithubBackups.Services.EmailSender.Dto
{
    public class ApiResponse<T>
    {
        public T Result { get; set; }

        public ApiWarning[] Warnings { get; set; }

        [JsonPropertyName("code")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("error")]
        public string ErrorMessage { get; set; }
    }

    public class ApiWarning
    {
        public string Warning { get; set; }
    }
}
