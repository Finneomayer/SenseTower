using FluentValidation;

namespace SC.SenseTower.Galleries.Cqrs.CreateGalleryImage
{
    public class CreateGalleryImageCommandValidator : AbstractValidator<CreateGalleryImageCommand>
    {
        public CreateGalleryImageCommandValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для добавления изображения");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Пользователь не определен");
            RuleFor(x => x.ImageId)
                .Must((c, imageId) =>
                {
                    return true;
                }).WithMessage("Нужно либо выбрать существующее изображение, либо загрузить новое");
        }
    }
}
