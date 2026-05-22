namespace SC.SenseTower.Admin.Dto
{
    public class ExternalRequestDto
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
