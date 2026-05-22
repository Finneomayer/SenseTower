namespace SC.SenseTower.MyPlaces.Settings
{
    public class ServiceEndpointsSettings
    {
        public string AccountsRootUrl { get; set; } = null!;

        public string AccountsGetInfoUrl { get; set; } = null!;

        public string AccountsLookupUrl { get; set; } = null!;
        
        public string AuthRootUrl { get; set; } = null!;

        public string ImagesRootUrl { get; set; } = null!;

        public string ImagesGetByIds { get; set; } = null!;

        public string SpacesRootUrl { get; set; } = null!;

        public string GetSpaceUrl { get; set; } = null!;

        public string GetSpacesByIdsUrl { get; set; } = null!;

        public string HallsRootUrl { get; set; } = null!;

        public string HallsUpdatePlaceUrl { get; set; } = null!;

        public string HallsDeletePlaceUrl { get; set; } = null!;
    }
}
