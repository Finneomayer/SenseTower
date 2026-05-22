using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Commands;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class DeleteImageCommandValidator : AbstractValidator<DeleteImageCommand>
    {
        public DeleteImageCommandValidator()
        {
            RuleFor(x => x.PlaceId).NotEmpty().WithMessage("Не задан идентификатор помещения");
            RuleFor(x => x.ImageId).NotEmpty().WithMessage("Не задан идентификатор изображения");
        }
    }
}
