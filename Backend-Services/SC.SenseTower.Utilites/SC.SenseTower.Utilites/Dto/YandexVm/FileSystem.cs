using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class FileSystem
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("deviceName")]
        public string DeviceName { get; set; }

        [JsonPropertyName("filesystemId")]
        public string FileSystemId { get; set; }
    }
}
