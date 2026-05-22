using FluentValidation;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventStateUpdate
{
    public class TowerEventStateUpdateCommandValidator : AbstractValidator<TowerEventStateUpdateCommand>
    {
        public TowerEventStateUpdateCommandValidator(TowerEventsService towerEventsService)
        {
            RuleFor(x => x.EventId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор события")
                .MustAsync(async (_, eventId, cancellationToken) =>
                {
                    var result = await towerEventsService.Exists(eventId, cancellationToken);
                    return result;
                }).WithMessage("Событие не найдено");
        }
    }
}
