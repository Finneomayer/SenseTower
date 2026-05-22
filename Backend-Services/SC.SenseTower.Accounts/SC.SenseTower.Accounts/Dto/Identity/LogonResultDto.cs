namespace SC.SenseTower.Accounts.Dto.Identity
{
    public class LogonResultDto
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Роль пользователя.
        /// </summary>
        public string Role { get; set; } = null!;

        /// <summary>
        /// Имя входа пользователя.
        /// </summary>
        public string Login { get; set; } = null!;

        /// <summary>
        /// Токен доступа.
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// Токен обновления.
        /// </summary>
        public string? RefeshToken { get; set; }

        /// <summary>
        /// Индекс аватара.
        /// </summary>
        public int? AvatarNumber { get; set; }
    }
}
