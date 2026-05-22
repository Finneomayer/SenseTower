using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class InstanceDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("folderId")]
        public string FolderId { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("labels")]
        public object? Labels { get; set; }

        [JsonPropertyName("zoneId")]
        public string ZoneId { get; set; }

        [JsonPropertyName("platformId")]
        public string PlatformId { get; set; }

        [JsonPropertyName("resources")]
        public Resources Resources { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

        [JsonPropertyName("metadataOptions")]
        public MetadataOptions MetadataOptions { get; set; }

        [JsonPropertyName("bootDisk")]
        public DiskDescriptor BootDisk { get; set; }

        [JsonPropertyName("secondaryDisks")]
        public List<DiskDescriptor> SecondaryDisks { get; set; }

        [JsonPropertyName("localDisks")]
        public List<LocalDisk> LocalDisks { get; set; }

        [JsonPropertyName("filesystems")]
        public List<FileSystem> Filesystems { get; set; }

        [JsonPropertyName("networkInterfaces")]
        public List<NetworkInterface> NetworkInterfaces { get; set; }

        [JsonPropertyName("fqdn")]
        public string Fqdn { get; set; }

        [JsonPropertyName("schedulingPolicy")]
        public SchedulingPolicy SchedulingPolicy { get; set; }

        [JsonPropertyName("serviceAccountId")]
        public string ServiceAccountId { get; set; }

        [JsonPropertyName("networkSettings")]
        public NetworkSettings NetworkSettings { get; set; }

        [JsonPropertyName("placementPolicy")]
        public PlacementPolicy PlacementPolicy { get; set; }
    }
}
