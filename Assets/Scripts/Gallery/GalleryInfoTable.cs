using JetBrains.Annotations;

namespace Assets.Scripts.Gallery
{
    public class GalleryInfoTable
    {
        public string Description { get; set; }
        [CanBeNull] public MyImage Image { get; set; }
        public bool ShowInformation { get; set; }
    }
}