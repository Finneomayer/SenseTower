namespace Assets.Scripts.Server
{
    public class ServerApiData : IServerApiData
    {
        public string AccessToken { get; set; }

        public bool IsRefreshing { get; set; }
    }
}
