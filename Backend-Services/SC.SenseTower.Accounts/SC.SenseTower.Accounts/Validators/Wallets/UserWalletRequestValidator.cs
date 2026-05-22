using FluentValidation;
using MongoDB.Driver;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Data;
using SC.SenseTower.Accounts.Data.Models;

namespace SC.SenseTower.Accounts.Validators.Wallets
{
    public class UserWalletRequestValidator : AbstractValidator<UserWalletRequest>
    {
        private Wallet? wallet = null;
        public UserWalletRequestValidator(AccountsDbContext context)
        {
            RuleFor(x => x.WalletId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор кошелька.")
                .MustAsync(async (walletId, cancellationToken) =>
                {
                    wallet ??= await context.Wallets.Find(r => r.Id == walletId).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                    return wallet != null;
                }).WithMessage("Кошелёк не найден.");
            RuleFor(x => x.OwnerId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не определён владелец кошелька.")
                .MustAsync(async (c, ownerId, cancellationToken) =>
                {
                    wallet ??= await context.Wallets.Find(r => r.Id == c.WalletId).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                    return wallet.OwnerId == ownerId;
                }).WithMessage("Указанный кошелёк вам не принадлежит.");
        }
    }
}
