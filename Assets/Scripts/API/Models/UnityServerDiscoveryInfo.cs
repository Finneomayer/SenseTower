namespace Assets.Scripts.Models
{
    public class UnityServerDiscoveryInfo
    {
        public string ServerAuthUrl { get; set; } = null!;
        public string PlaceUsersPostUrl { get; set; } = null!;
        public string PlaceUserCheckUrl { get; set; } = null!;
        public string CheckClientsTokenUrl { get; set; } = null!;
        public string CheckClientsAccessToSpacesUrl { get; set; } = null!;
        public string GetPlaceBySpaceUrl { get; set; } = null!;
        public string GetUsersInSpaceUrl { get; set; } = null!;
        public string RemoteContent_Url { get; set; } = null!;
        public string RemoteServerContent_Url { get; set; } = null!;
        public string GetCinemaUrl { get; set; } = null!;
        public string GetTowerEventsUrl { get; set; } = null!;
        public string GetShopsUrl { get; set; } = null!;
        public string GetSpaceStaticObjectsUrl { get; set; } = null!;
        public string GetFriendsEndPoint { get; set; } = null!;
        public string GetMafiaEndPoint { get; set; } = null!;
        public string GetAvatarRecorderEndPoint { get; set; } = null!;
        public string GetSpaceObjectsNewEndPoint { get; set; } = null!;
    }
}