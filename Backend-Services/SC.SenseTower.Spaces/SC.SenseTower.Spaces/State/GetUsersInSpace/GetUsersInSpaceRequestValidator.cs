using FluentValidation;

namespace SC.SenseTower.Spaces.State.GetUsersInSpace
{
    public class GetUsersInSpaceRequestValidator : AbstractValidator<GetUsersInSpaceRequest>
    {
        public GetUsersInSpaceRequestValidator()
        {
            RuleFor(x => x.SpaceId).NotEmpty().WithMessage("Необходимо указать идентификатор пространства");
        }
    }
}
