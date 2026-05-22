namespace SC.SenseTower.Accounts.Dto.Identity
{
    public class RefreshUserTokenResultDto
    {
        /// <summary>
        /// Токен доступа.
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// Токен обновления.
        /// </summary>
        public string RefreshToken { get; set; } = null!;
    }
}
