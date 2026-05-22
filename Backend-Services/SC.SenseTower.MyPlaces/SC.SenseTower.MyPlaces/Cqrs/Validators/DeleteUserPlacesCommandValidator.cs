using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Commands;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class DeleteUserPlacesCommandValidator : AbstractValidator<DeleteUserPlacesCommand>
    {
        public DeleteUserPlacesCommandValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty().WithMessage("Не указан идентификатор владельца.");
        }
    }
}
