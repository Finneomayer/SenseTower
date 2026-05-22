namespace SC.SenseTower.Utilities.Dto.YandexVm
{
    public class UpdateMetadataRequestDto
    {
        public string[] Delete { get; set; } = Array.Empty<string>();

        public Dictionary<string, object> Upsert { get; set; } = new Dictionary<string, object>();
    }
}
