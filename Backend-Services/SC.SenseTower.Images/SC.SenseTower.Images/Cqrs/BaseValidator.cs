using FluentValidation;
using SC.SenseTower.Images.Data.Models;
using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.Cqrs
{
    public class BaseValidator<T> : AbstractValidator<T> where T : BaseRequest
    {
        protected ImageFile? imageFile = null;

        public BaseValidator(ImageFilesService imageFilesService)
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Пользователь не авторизован");
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано изображение")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    imageFile ??= await imageFilesService.Get(id, cancellationToken);
                    return imageFile != null;
                }).WithMessage("Изображение не найдено")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    imageFile ??= await imageFilesService.Get(id, cancellationToken);
                    return imageFile?.UserId == c.UserId;
                }).WithMessage("Изображение принадлежит другому пользователю");
        }
    }
}
