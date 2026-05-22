namespace SC.SenseTower.Auth.Models
{
    public class UserInfo
    {
        public Guid UserId { get; set; }

        public string Role { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime? AccessGrantedTo { get; set; }
    }
}
