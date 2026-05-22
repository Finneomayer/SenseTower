using SC.SenseTower.Accounts.Dto.Invites;

namespace SC.SenseTower.Accounts.Dto.UserInfo
{
    public class UserInviteDto
    {
        /// <summary>
        /// Идентификатор приглашения.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Информация об использовании приглашения.
        /// </summary>
        public UsingInfoDto? UsingInfo { get; set; }

        /// <summary>
        /// Информация об отзыве приглашения.
        /// </summary>
        public RecallInfoDto? RecallInfo { get; set; }
    }
}
