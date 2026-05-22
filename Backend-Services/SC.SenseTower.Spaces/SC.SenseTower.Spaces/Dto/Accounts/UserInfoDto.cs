namespace SC.SenseTower.Spaces.Dto.Accounts
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
        /// Регистрационная почта пользователя
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Индекс аватара пользователя.
        /// </summary>
        public int? AvatarNumber { get; set; }
    }
}
