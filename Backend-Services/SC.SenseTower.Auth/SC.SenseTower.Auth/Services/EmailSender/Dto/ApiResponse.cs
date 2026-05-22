using SC.SenseTower.Auth.Services.EmailSender.Enum;
using System.Text.Json.Serialization;

namespace SC.SenseTower.Auth.Services.EmailSender.Dto
{
    public class ApiResponse<T>
    {
        public T Result { get; set; }

        public ApiWarning[] Warnings { get; set; }

        [JsonPropertyName("code")]
        public ApiErrors? ErrorCode { get; set; }

        [JsonPropertyName("error")]
        public string ErrorMessage { get; set; }
    }

    public class ApiWarning
    {
        public string Warning { get; set; }
    }
}
