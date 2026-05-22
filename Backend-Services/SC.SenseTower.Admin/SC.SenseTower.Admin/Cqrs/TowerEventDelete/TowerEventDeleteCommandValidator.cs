using FluentValidation;

namespace SC.SenseTower.Admin.Cqrs.TowerEventDelete
{
    public class TowerEventDeleteCommandValidator : AbstractValidator<TowerEventDeleteCommand>
    {
        public TowerEventDeleteCommandValidator()
        {
            RuleFor(x => x.EventId).NotEmpty().WithMessage("Не указан идентификатор события");
        }
    }
}
