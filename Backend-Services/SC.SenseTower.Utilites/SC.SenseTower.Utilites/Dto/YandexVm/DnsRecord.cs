using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class DnsRecord
    {
        [JsonPropertyName("fqdn")]
        public string Fqdn { get; set; } = null!;

        [JsonPropertyName("dnsZoneId")]
        public string DnsZoneId { get; set; } = null!;

        [JsonPropertyName("ttl")]
        public string Ttl { get; set; } = null!;

        [JsonPropertyName("ptr")]
        public bool Ptr { get; set; }
    }
}
