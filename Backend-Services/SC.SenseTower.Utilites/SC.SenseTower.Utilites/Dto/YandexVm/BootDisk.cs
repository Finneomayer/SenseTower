using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class DiskDescriptor
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = null!;

        [JsonPropertyName("deviceName")]
        public string DeviceName { get; set; } = null!;

        [JsonPropertyName("autoDelete")]
        public bool AutoDelete { get; set; }

        [JsonPropertyName("diskId")]
        public string DiskId { get; set; } = null!;
    }
}
