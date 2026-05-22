using FluentValidation;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEvent
{
    public class TowerEventRequestValidator : AbstractValidator<TowerEventRequest>
    {
        public TowerEventRequestValidator()
        {
            RuleFor(x => x.EventId).NotEmpty().WithMessage("Не задан идентификатор мероприятия");
        }
    }
}
