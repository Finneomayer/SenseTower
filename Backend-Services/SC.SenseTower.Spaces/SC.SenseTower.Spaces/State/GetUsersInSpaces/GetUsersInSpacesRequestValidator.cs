namespace SC.SenseTower.Spaces.State.GetUsersInSpaces;

using FluentValidation;

    public class CheckUserInSpaceRequestValidator : AbstractValidator<GetUsersInSpacesRequest>
    {
        public CheckUserInSpaceRequestValidator()
        {
            RuleFor(x => x.GetCount)
                .GreaterThan(0).WithMessage("Максимальное число пользователей для возврата должно быть больше нуля");
        }
    }
