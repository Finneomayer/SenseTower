using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class LocalDisk
    {
        [JsonPropertyName("size")]
        public string Size { get; set; } = null!;

        [JsonPropertyName("deviceName")]
        public string DeviceName { get; set; } = null!;
    }
}
