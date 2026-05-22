namespace SC.SenseTower.Halls.Dto.Halls
{
    public class PictureDto
    {
        public int Location { get; set; }

        public ImageDto Image { get; set; } = new();
    }
}
