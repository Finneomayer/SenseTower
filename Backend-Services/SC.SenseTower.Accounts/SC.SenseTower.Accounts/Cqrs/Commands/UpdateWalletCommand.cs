using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class UpdateWalletCommand : IRequest<bool>
    {
        /// <summary>
        /// Идентификатор кошелька.
        /// </summary>
        public string WalletId { get; set; } = null!;

        /// <summary>
        /// Название кошелька.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Признак активного кошелька.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Идентификатор пользователя-владельца.
        /// </summary>
        public Guid? OwnerId { get; set; }
    }
}
