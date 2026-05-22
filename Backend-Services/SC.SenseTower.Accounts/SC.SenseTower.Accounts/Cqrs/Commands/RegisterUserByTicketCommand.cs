using MediatR;
using SC.SenseTower.Accounts.Dto.Identity;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class RegisterUserByTicketCommand : IRequest<LogonResultDto>
    {
        /// <summary>
        /// Имя входа.
        /// </summary>
        public string Login { get; set; } = null!;

        /// <summary>
        /// Регистрационная почта.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Пароль.
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// Идентификатор гостевого приглашения.
        /// </summary>
        public string TicketId { get; set; } = null!;
    }
}
