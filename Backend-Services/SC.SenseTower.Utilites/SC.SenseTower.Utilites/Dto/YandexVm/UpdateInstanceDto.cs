using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class UpdateInstanceDto
    {
        [JsonPropertyName("updateMask")]
        public string UpdateMask { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("labels")]
        public string Labels { get; set; }

        [JsonPropertyName("platformId")]
        public string PlatformId { get; set; }

        [JsonPropertyName("resourcesSpec")]
        public Resources Resources { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonPropertyName("metadataOptions")]
        public MetadataOptions MetadataOptions { get; set; }

        [JsonPropertyName("serviceAccountId")]
        public string ServiceAccountId { get; set; }

        [JsonPropertyName("networkSettings")]
        public NetworkSettings NetworkSettings { get; set; }

        [JsonPropertyName("placementPolicy")]
        public PlacementPolicy PlacementPolicy { get; set; }

        [JsonPropertyName("schedulingPolicy")]
        public SchedulingPolicy SchedulingPolicy { get; set; }
    }
}
