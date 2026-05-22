namespace SC.SenseTower.Admin.Models.Users
{
    public class UserBanViewModel
    {
        public Guid UserId { get; set; }

        public DateTime LockoutEnd { get; set; } = DateTime.UtcNow.AddDays(1);

        public bool IsPermanent { get; set; }
    }
}
