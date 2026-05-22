using FluentValidation;
using SC.SenseTower.Spaces.Cqrs.Commands;

namespace SC.SenseTower.Spaces.Cqrs.Validators
{
    public class DeleteImageCommandValidator : AbstractValidator<DeleteImageCommand>
    {
        public DeleteImageCommandValidator()
        {
            RuleFor(x => x.SpaceId).NotEmpty().WithMessage("Не задан идентификатор помещения");
            RuleFor(x => x.ImageId).NotEmpty().WithMessage("Не задан идентификатор изображения");
        }
    }
}
