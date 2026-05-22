namespace SC.SenseTower.Halls.Dto.Halls
{
    public class UserInfoDto
    {
        public Guid UserId { get; set; }

        public string Role { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int? AvatarNumber { get; set; }
    }
}
