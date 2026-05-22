namespace SC.SenseTower.Spaces.Data.Models
{
    public class Picture
    {
        public int Location { get; set; }

        public ImageInfo Image { get; set; } = new();
    }
}
