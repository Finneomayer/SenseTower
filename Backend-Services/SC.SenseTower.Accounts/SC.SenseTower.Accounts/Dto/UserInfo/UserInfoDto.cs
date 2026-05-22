namespace SC.SenseTower.Accounts.Dto.UserInfo
{
    public class UserInfoDto
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
        /// Имя входа.
        /// </summary>
        public string Login { get; set; } = null!;

        /// <summary>
        /// Регистрационная почта.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Индекс аватара.
        /// </summary>
        public int? AvatarNumber { get; set; }

        /// <summary>
        /// Приглашения, сгенерированные для пользователя.
        /// </summary>
        public UserInviteDto[] Invites { get; set; } = Array.Empty<UserInviteDto>();
    }
}
