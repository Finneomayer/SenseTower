using FluentValidation;

namespace SC.SenseTower.Spaces.State.CheckIsUserImSpace
{
    public class CheckUserInSpaceRequestValidator : AbstractValidator<CheckIsUserInSpaceRequest>
    {
        public CheckUserInSpaceRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Не задан пользователь");
            RuleFor(x => x.SpaceId).NotEmpty().WithMessage("Не задано помещение");
        }
    }
}
