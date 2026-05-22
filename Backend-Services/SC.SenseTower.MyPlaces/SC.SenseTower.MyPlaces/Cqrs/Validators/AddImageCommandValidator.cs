using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Commands;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class AddImageCommandValidator : AbstractValidator<AddImageCommand>
    {
        public AddImageCommandValidator()
        {
            RuleFor(x => x.PlaceId).NotEmpty().WithMessage("Не задан идентификатор помещения");
            RuleFor(x => x.ImageId).NotEmpty().WithMessage("Не задан идентификатор изображения");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Не определен пользователь");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для добавления изображения");
        }
    }
}
