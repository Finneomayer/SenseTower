using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class SetAvatarCommand : IRequest<bool>
    {
        /// <summary>
        /// Идентификатор пользователя, которому устанавливается аватар.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Индекс аватара.
        /// </summary>
        public int? AvatarNumber { get; set; }

        /// <summary>
        /// Идентификатор текущего пользователя (заполняется сервером).
        /// </summary>
        public Guid CurrentUserId { get; set; }
    }
}
