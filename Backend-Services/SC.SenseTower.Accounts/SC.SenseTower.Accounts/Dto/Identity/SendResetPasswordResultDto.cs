namespace SC.SenseTower.Accounts.Dto.Identity
{
    public class SendResetPasswordResultDto
    {
        public Guid UserId { get; set; }

        public string Token { get; set; } = null!;
    }
}
