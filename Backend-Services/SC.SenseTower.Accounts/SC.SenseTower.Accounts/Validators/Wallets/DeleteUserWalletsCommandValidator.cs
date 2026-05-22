using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Commands;

namespace SC.SenseTower.Accounts.Validators.Wallets
{
    public class DeleteUserWalletsCommandValidator : AbstractValidator<DeleteUserWalletsCommand>
    {
        public DeleteUserWalletsCommandValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty().WithMessage("Не указан идентификатор владельца кошельков.");
        }
    }
}
