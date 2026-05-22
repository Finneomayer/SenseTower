using FluentValidation;

namespace SC.SenseTower.Admin.Cqrs.TowerEventAddTickets
{
    public class TowerEventAddTicketsCommandValidator : AbstractValidator<TowerEventAddTicketsCommand>
    {
        public TowerEventAddTicketsCommandValidator()
        {
            RuleFor(x => x.EventId).NotEmpty().WithMessage("Не указан идентификатор события");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Количество билетов должно быть больше 0");
        }
    }
}
