namespace SC.SenseTower.Images.Settings
{
    public class ImageSettings
    {
        public string RootUrl { get; set; } = null!;

        public PreviewSettings Preview { get; set; } = new();

        public OriginalSettings Original { get; set; } = new();
    }

    public class OriginalSettings
    {
        public int Size { get; set; } = 1000;

        public int Weight { get; set; } = 200000;
    }

    public class PreviewSettings
    {
        public int Size { get; set; } = 200;

        public string Resampler { get; set; } = "Bicubic";
    }
}
