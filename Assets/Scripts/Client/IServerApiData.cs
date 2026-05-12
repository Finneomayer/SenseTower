namespace Assets.Scripts.Server
{
    public interface IServerApiData
    {
        string AccessToken { get; set; }
        public bool IsRefreshing { get; set; }
    }
}
