namespace SC.SenseTower.Admin.Dto.Identity
{
    public class RefreshTokenResultDto
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
