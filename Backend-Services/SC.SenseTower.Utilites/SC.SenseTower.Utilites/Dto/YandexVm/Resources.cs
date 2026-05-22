using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class Resources
    {
        [JsonPropertyName("memory")]
        public string Memory { get; set; } = null!;

        [JsonPropertyName("cores")]
        public string Cores { get; set; } = null!;

        [JsonPropertyName("coreFraction")]
        public string CoreFraction { get; set; } = null!;

        [JsonPropertyName("gpus")]
        public string Gpus { get; set; } = null!;
    }
}
