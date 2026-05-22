using FluentValidation;
using SC.SenseTower.Halls.Cqrs.Commands;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class AddSpaceCommandValidator : AbstractValidator<AddSpaceCommand>
    {
        public AddSpaceCommandValidator()
        {
            RuleFor(x => x.HallId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор холла")
                ;
            RuleFor(x => x.SpaceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор пространства")
                ;
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("Недостаточно прав для выполнения операции");
        }
    }
}
