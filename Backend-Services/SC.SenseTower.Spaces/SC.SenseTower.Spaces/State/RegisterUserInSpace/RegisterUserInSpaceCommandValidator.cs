using FluentValidation;

namespace SC.SenseTower.Spaces.State.RegisterUserInSpace
{
    public class RegisterUserInSpaceCommandValidator : AbstractValidator<RegisterUserInSpaceCommand>
    {
        public RegisterUserInSpaceCommandValidator()
        {
            RuleFor(x => x.SpaceId).NotEmpty().WithMessage("Необходимо указать идентификатор пространства");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Пользователь не аутентифицирован");
        }
    }
}
