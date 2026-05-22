namespace SC.SenseTower.TowerEvents.Dto.Images
{
    public class ImageInfoResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string FileUrl { get; set; } = null!;

        public string PreviewUrl { get; set; } = null!;
    }
}
