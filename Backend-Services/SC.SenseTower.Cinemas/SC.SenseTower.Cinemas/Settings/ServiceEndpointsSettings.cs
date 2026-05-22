namespace SC.SenseTower.Cinemas.Settings
{
    public class ServiceEndpointsSettings
    {
        public string SpacesRootUrl { get; set; } = null!;

        public string GetSpaceUrl { get; set; } = null!;

        public string AccountsRootUrl { get; set; } = null!;

        public string GetUserInfoUrl { get; set; } = null!;

        public string AccountsLookupUrl { get; set; } = null!;

        public string AccountsGetByIdsUrl { get; set; } = null!;
    }
}
