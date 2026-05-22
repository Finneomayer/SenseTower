using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class OneToOneNat
    {
        [JsonPropertyName("address")]
        public string Address { get; set; } = null!;

        [JsonPropertyName("ipVersion")]
        public string IpVersion { get; set; } = null!;

        [JsonPropertyName("dnsRecords")]
        public DnsRecord[] DnsRecords { get; set; } = Array.Empty<DnsRecord>();
    }
}
