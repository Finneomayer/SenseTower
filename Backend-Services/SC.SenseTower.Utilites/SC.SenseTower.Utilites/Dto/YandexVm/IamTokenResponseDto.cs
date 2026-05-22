using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class IamTokenResponseDto
    {
        public string IamToken { get; set; } = null!;

        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
