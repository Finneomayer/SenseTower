namespace SC.SenseTower.Auth.Models
{
    public class SendResetPasswordResultDto
    {
        public Guid UserId { get; set; }

        public string Token { get; set; } = null!;
    }
}
