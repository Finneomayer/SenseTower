namespace API.Models
{
    public class TokenDetails
    {
        public long auth_time { get; set; }
        public long exp { get; set; }
        public string role { get; set; }
        public string client_id { get; set; }
    }
}