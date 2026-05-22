using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class PlacementPolicy
    {
        [JsonPropertyName("placementGroupId")]
        public string PlacementGroupId { get; set; } = null!;

        [JsonPropertyName("hostAffinityRules")]
        public HostAffinityRule[] HostAffinityRules { get; set; } = Array.Empty<HostAffinityRule>();
    }
}
