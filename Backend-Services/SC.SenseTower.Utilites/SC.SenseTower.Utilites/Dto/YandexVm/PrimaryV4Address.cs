using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class PrimaryV4Address
    {
        [JsonPropertyName("address")]
        public string Address { get; set; } = null!;

        [JsonPropertyName("oneToOneNat")]
        public OneToOneNat? OneToOneNat { get; set; }

        [JsonPropertyName("dnsRecords")]
        public DnsRecord[] DnsRecords { get; set; } = Array.Empty<DnsRecord>();
    }
}
