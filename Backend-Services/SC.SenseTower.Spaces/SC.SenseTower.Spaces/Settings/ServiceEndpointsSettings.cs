namespace SC.SenseTower.Spaces.Settings
{
    public class ServiceEndpointsSettings
    {
        public string AccountsRootUrl { get; set; } = null!;

        public string AccountsGetInfoUrl { get; set; } = null!;

        public string LookupUsersUrl { get; set; } = null!;

        public string MyPlacesRootUrl { get; set; } = null!;

        public string PlacesListByIds { get; set; } = null!;

        public string CreatePlaceUrl { get; set; } = null!;

        public string LookupPlacesUrl { get; set; } = null!;

        public string DeletePlaceUrl { get; set; } = null!;

        public string PlacesGetBySpaceUrl { get; set; } = null!;

        public string HallsRootUrl { get; set; } = null!;

        public string LookupHallsUrl { get; set; } = null!;

        public string ClearSpaceUrl { get; set; } = null!;

        public string ImagesRootUrl { get; set; } = null!;

        public string ImagesGetByIds { get; set; } = null!;
    }
}
