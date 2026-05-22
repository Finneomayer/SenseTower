using SC.SenseTower.Admin.Dto.Images;

namespace SC.SenseTower.Admin.Dto.Galleries
{
    public class InfoTableResponseDto
    {
        public string? Description { get; set; }

        public ImageInfoDto Image { get; set; } = new();

        public bool ShowInformation { get; set; }
    }
}
