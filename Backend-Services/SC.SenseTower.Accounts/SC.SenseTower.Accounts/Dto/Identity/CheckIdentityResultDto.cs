namespace SC.SenseTower.Accounts.Dto.Identity
{
    /// <summary>
    /// Результат проверки токена пользователя.
    /// </summary>
    public class CheckIdentityResultDto
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Признак валидности токена.
        /// </summary>
        public bool IsTokenValid { get; set; }

        /// <summary>
        /// Признак удалённого пользователя.
        /// </summary>
        public bool IsUserDeleted { get; set; }

        /// <summary>
        /// Признак заблокированного пользователя.
        /// </summary>
        public bool IsUserBlocked { get; set; }

        /// <summary>
        /// Список сообщений об ошибках, найденных при проверке токена.
        /// </summary>
        public List<string> Errors { get; set; } = new();
    }
}
