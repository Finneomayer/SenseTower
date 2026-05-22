using FluentValidation;
using MongoDB.Driver;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Data;
using SC.SenseTower.Accounts.Data.Models;

namespace SC.SenseTower.Accounts.Validators.Wallets
{
    public class UpdateWalletCommandValidator : AbstractValidator<UpdateWalletCommand>
    {
        private readonly AccountsDbContext context;
        
        private Wallet? wallet = null;

        public UpdateWalletCommandValidator(AccountsDbContext dbContext)
        {
            context = dbContext;
            RuleFor(x => x.WalletId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не задан идентификатор кошелька")
                .MustAsync(async (walletId, cancellationToken) =>
                {
                    wallet ??= await context.Wallets.Find(x => x.Id == walletId).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                    return wallet != null;
                }).WithMessage($"Указанный кошелёк не найден.");
            RuleFor(x => x.OwnerId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан владелец кошелька.")
                .MustAsync(async (c, ownerId, cancellationToken) =>
                {
                    wallet ??= await context.Wallets.Find(x => x.Id == c.WalletId).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                    return wallet?.OwnerId == ownerId;
                }).WithMessage("Нельзя изменить кошелёк другого владельца.");
            RuleFor(x => x.IsActive)
                .MustAsync(async (c, isActive, cancellationToken) =>
                {
                    wallet ??= await context.Wallets.Find(x => x.Id == c.WalletId).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                    return wallet == null || wallet.IsActive == isActive || wallet.IsConfirmed;
                }).WithMessage("Нельзя изменить активность неподтверждённого кошелька.");
        }
    }
}
