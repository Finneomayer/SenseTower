using FluentValidation;

namespace SC.SenseTower.Images.Cqrs.UserImages
{
    public class UserImagesRequestValidator : AbstractValidator<UserImagesRequest>
    {
        public UserImagesRequestValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty().WithMessage("Пользователь не авторизован");
        }
    }
}
