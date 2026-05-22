using MediatR;
using SC.SenseTower.Accounts.Dto.Identity;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class RegisterUserCommand : IRequest<LogonResultDto>
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
        /// Пароль
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// Идентификатор приглашения.
        /// </summary>
        public string InviteId { get; set; } = null!;

        /// <summary>
        /// Список кошельков пользователя.
        /// </summary>
        public IEnumerable<AddWalletCommand>? Wallets { get; set; }
    }
}
