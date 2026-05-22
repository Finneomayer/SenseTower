using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Commands;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class DeletePlaceCommandValidator : AbstractValidator<DeletePlaceCommand>
    {
        public DeletePlaceCommandValidator()
        {
            RuleFor(x => x.PlaceId).NotEmpty().WithMessage("Не указано помещение");
        }
    }
}
