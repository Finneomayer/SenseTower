using FluentValidation;
using MongoDB.Driver;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Data;

namespace SC.SenseTower.Accounts.Validators.Wallets
{
    public class AddWalletCommandValidator : AbstractValidator<AddWalletCommand>
    {
        public AddWalletCommandValidator(AccountsDbContext context)
        {
            RuleFor(x => x.WalletId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор кошелька.")
                .MustAsync(async (walletId, cancellationToken) =>
                {
                    var wallet = await context.Wallets.Find(x => x.Id == walletId).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                    return wallet == null;
                }).WithMessage("Кошелёк с таким идентификатором уже существует.");
            RuleFor(x => x.OwnerId).NotEmpty().WithMessage("Не указан владелец кошелька.");
        }
    }
}
