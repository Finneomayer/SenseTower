using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class AddWalletCommand : IRequest<string>
    {
        /// <summary>
        /// Идентификатор владельца.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Идентификатор кошелька.
        /// </summary>
        public string? WalletId { get; set; }

        /// <summary>
        /// Имя кошелька.
        /// </summary>
        public string? Name { get; set; }
    }
}
