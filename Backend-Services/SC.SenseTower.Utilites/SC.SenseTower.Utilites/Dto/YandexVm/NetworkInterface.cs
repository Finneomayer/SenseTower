using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class NetworkInterface
    {
        [JsonPropertyName("index")]
        public string Index { get; set; } = null!;

        [JsonPropertyName("macAddress")]
        public string MacAddress { get; set; } = null!;

        [JsonPropertyName("subnetId")]
        public string SubnetId { get; set; } = null!;

        [JsonPropertyName("primaryV4Address")]
        public PrimaryV4Address PrimaryV4Address { get; set; } = null!;

        [JsonPropertyName("primaryV6Address")]
        public PrimaryV6Address PrimaryV6Address { get; set; } = null!;

        [JsonPropertyName("securityGroupIds")]
        public string[] SecurityGroupIds { get; set; } = Array.Empty<string>();
    }
}
