using FluentValidation;

namespace SC.SenseTower.Images.Cqrs.AddImageFile
{
    public class AddImageFileCommandValidator : AbstractValidator<AddImageFileCommand>
    {
        public AddImageFileCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Пользователь не аутентифицирован.");
            RuleFor(x => x.File)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не выбран файл")
                .Must((file) =>
                {
                    return !string.IsNullOrEmpty(file.FileName) && file.Length > 0;
                }).WithMessage("Файл не прочитан");
        }
    }
}
