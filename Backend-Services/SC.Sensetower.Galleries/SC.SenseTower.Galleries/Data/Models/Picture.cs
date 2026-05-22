namespace SC.SenseTower.Galleries.Data.Models
{
    public class Picture
    {
        public int Position { get; set; }

        public GalleryImage Image { get; set; } = new();
    }
}
