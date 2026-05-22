using FluentValidation;
using SC.SenseTower.Halls.Cqrs.Commands;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class RemoveSpaceCommandValidator : AbstractValidator<RemoveSpaceCommand>
    {
        public RemoveSpaceCommandValidator()
        {
            RuleFor(x => x.HallId).NotEmpty().WithMessage("Не указан идентификатор холла");
            RuleFor(x => x.SpaceId).NotEmpty().WithMessage("Не указан идентификатор пространства");
        }
    }
}
