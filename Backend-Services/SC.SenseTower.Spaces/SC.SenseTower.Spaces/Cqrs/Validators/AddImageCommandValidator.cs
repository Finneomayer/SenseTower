using FluentValidation;
using SC.SenseTower.Spaces.Cqrs.Commands;

namespace SC.SenseTower.Spaces.Cqrs.Validators
{
    public class AddImageCommandValidator : AbstractValidator<AddImageCommand>
    {
        public AddImageCommandValidator()
        {
            RuleFor(x => x.SpaceId).NotEmpty().WithMessage("Не задан идентификатор помещения");
            RuleFor(x => x.ImageId).NotEmpty().WithMessage("Не задан идентификатор изображения");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Не определен пользователь");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для добавления изображения");
        }
    }
}
