using System.Text.Json.Serialization;

namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class OperationResultDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; } = null!;

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = null!;

        [JsonPropertyName("modifiedAt")]
        public string ModifiedAt { get; set; } = null!;

        [JsonPropertyName("done")]
        public bool Done { get; set; }

        [JsonPropertyName("metadata")]
        public object? Metadata { get; set; }

        [JsonPropertyName("error")]
        public ErrorDto? Error { get; set; }

        [JsonPropertyName("response")]
        public object? Response { get; set; }
    }
}
