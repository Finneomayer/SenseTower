namespace SC.SenseTower.TowerEvents.Settings
{
    public class ServiceEndpointsSettings
    {
        public string SpacesRootUrl { get; set; } = null!;

        public string GetAllSpacesUrl { get; set; } = null!;

        public string GetSpaceUrl { get; set; } = null!;

        public string ImagesRootUrl { get; set; } = null!;

        public string GetImageInfoUrl { get; set; } = null!;

        public string GetImageInfoByIdUrl { get; set; } = null!;
    }
}
