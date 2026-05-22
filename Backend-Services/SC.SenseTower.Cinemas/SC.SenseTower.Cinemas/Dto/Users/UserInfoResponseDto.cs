namespace SC.SenseTower.Cinemas.Dto.Users
{
    public class UserInfoResponseDto
    {
        public Guid UserId { get; set; }

        public string Role { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string Email { get; set; } = null!;
    }
}
