using FluentValidation;
using MongoDB.Driver;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Data;

namespace SC.SenseTower.Accounts.Validators.Wallets
{
    public class ConfirmWalletCommandValidator : AbstractValidator<ConfirmWalletCommand>
    {
        public ConfirmWalletCommandValidator(AccountsDbContext context)
        {
            RuleFor(x => x.WalletId)
                .NotEmpty().WithMessage("Не указан идентификатор кошелька.")
                .MustAsync(async (walletId, cancellationToken) =>
                {
                    var wallet = await context.Wallets.Find(r => r.Id == walletId).FirstOrDefaultAsync(cancellationToken);
                    return wallet != null;
                }).WithMessage("Указанный кошелёк не найден.");
        }
    }
}
