namespace SC.SenseTower.Tickets.Settings
{
    public class ServiceEndpointsSettings
    {
        public string TowerEventsRootUrl { get; set; } = null!;

        public string GetTowerEventUrl { get; set; } = null!;

        public string AccountsRootUrl { get; set; } = null!;

        public string AccountsGetInfoByIdsUrl { get; set; } = null!;
    }
}
