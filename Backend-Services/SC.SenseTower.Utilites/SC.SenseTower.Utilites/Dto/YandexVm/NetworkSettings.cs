using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class NetworkSettings
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;
    }
}
