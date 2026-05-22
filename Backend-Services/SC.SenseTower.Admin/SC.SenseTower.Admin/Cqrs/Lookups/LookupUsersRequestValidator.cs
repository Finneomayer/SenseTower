using FluentValidation;
using SC.SenseTower.Admin.Constants;

namespace SC.SenseTower.Admin.Cqrs.Lookups
{
    public class LookupUsersRequestValidator : AbstractValidator<LookupUsersRequest>
    {
        public LookupUsersRequestValidator()
        {
            RuleFor(x => x.Role)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указана роль пользователя.")
                .Must((role) => role == RoleNames.VR_ADMIN || role == RoleNames.VR_USER).WithMessage("Недопустимая роль пользователя.");
        }
    }
}
