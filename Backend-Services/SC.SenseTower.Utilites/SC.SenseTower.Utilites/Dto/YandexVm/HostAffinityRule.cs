using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class HostAffinityRule
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = null!;

        [JsonPropertyName("op")]
        public string Op { get; set; } = null!;

        [JsonPropertyName("values")]
        public string[]? Values { get; set; } = Array.Empty<string>();
    }
}
