namespace SC.SenseTower.Images.Dto.Accounts
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }

        public string Role { get; set; } = null!;

        public string Login { get; set; } = null!;
    }
}
