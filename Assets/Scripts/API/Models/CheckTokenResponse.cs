namespace API.Models
{
    public class CheckTokenResponse
    {
        public string userId { get; set; }
        public bool isTokenValid { get; set; }
        public bool isUserDeleted { get; set; }
        public bool isUserBlocked { get; set; }
        public string[] errors { get; set; }
    }
}