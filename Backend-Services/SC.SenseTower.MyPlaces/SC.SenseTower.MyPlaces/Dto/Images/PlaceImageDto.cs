namespace SC.SenseTower.MyPlaces.Dto.Images
{
    public class PlaceImageDto
    {
        public int Location { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string FileUrl { get; set; } = null!;

        public string PreviewUrl { get; set; } = null!;
    }
}
