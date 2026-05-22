using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class MetadataOptions
    {
        [JsonPropertyName("gceHttpEndpoint")]
        public string GceHttpEndpoint { get; set; } = null!;

        [JsonPropertyName("awsV1HttpEndpoint")]
        public string AwsV1HttpEndpoint { get; set; } = null!;

        [JsonPropertyName("gceHttpToken")]
        public string GceHttpToken { get; set; } = null!;

        [JsonPropertyName("awsV1HttpToken")]
        public string AwsV1HttpToken { get; set; } = null!;
    }
}
