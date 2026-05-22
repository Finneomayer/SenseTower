using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class ErrorDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = null!;

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;

        [JsonPropertyName("details")]
        public object[] Details { get; set; } = Array.Empty<object>();
    }
}
