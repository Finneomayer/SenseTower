using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Commands;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class DeleteUserPlaceCommandValidator : AbstractValidator<DeleteUserPlaceCommand>
    {
        public DeleteUserPlaceCommandValidator()
        {
            RuleFor(x => x.PlaceId).NotEmpty().WithMessage("Не указано помещение");
        }
    }
}
