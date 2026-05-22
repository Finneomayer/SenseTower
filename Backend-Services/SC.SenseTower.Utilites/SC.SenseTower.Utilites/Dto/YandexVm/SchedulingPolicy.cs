using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class SchedulingPolicy
    {
        [JsonPropertyName("preemptible")]
        public bool Preemptible { get; set; }
    }
}
