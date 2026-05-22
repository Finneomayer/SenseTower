using FluentValidation;
using MongoDB.Driver;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Data;
using SC.SenseTower.Accounts.Data.Models;

namespace SC.SenseTower.Accounts.Validators.Wallets
{
    public class DeleteWalletCommandValidator : AbstractValidator<DeleteWalletCommand>
    {
        private Wallet? wallet = null;

        public DeleteWalletCommandValidator(AccountsDbContext context)
        {
            RuleFor(x => x.WalletId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не задан идентификатор кошелька.")
                .MustAsync(async (walletId, cancellationToken) =>
                {
                    wallet ??= await context.Wallets.Find(x => x.Id == walletId).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                    return wallet != null;
                }).WithMessage("Не найден кошелёк с указанным идентификатором.");
            RuleFor(x => x.OwnerId)
                .MustAsync(async (c, ownerId, cancellationToken) =>
                {
                    wallet ??= await context.Wallets.Find(x => x.Id == c.WalletId).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                    return wallet?.OwnerId == ownerId;
                }).WithMessage("Нельзя удалить кошелёк другого владельца.");
        }
    }
}
