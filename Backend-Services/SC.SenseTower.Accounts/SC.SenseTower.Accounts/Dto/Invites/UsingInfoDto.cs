namespace SC.SenseTower.Accounts.Dto.Invites
{
    public class UsingInfoDto
    {
        /// <summary>
        /// Универсальные дата и время использования приглашения.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Идентификатор пользователя, использовавшего приглашение.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Имя пользователя, использовавшего приглашение.
        /// </summary>
        public string? UserName { get; set; }
    }
}
