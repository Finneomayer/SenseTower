using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class DeleteAccountCommand : IRequest<bool>
    {
        /// <summary>
        /// Идентификатор пользователя для удаления.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Токен текущего пользователя (заполняется сервером).
        /// </summary>
        public string? AccessToken { get; set; }
    }
}
