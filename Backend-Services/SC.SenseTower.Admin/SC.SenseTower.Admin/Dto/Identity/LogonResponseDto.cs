namespace SC.SenseTower.Admin.Dto.Identity
{
    public class LogonResponseDto
    {
        public Guid UserId { get; set; }

        public string Role { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string AccessToken { get; set; } = null!;

        public string? RefeshToken { get; set; }
    }
}
