using FluentValidation;
using SC.SenseTower.Admin.Services;

namespace SC.SenseTower.Admin.Cqrs.TowerEvent
{
    public class TowerEventRequestValidator : AbstractValidator<TowerEventRequest>
    {
        public TowerEventRequestValidator(TowerEventsService towerEventsService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не выбрано событие")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    return await towerEventsService.Exists(c.AccessToken, id, cancellationToken);
                }).WithMessage("Событие не найдено");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для редактирования события");
        }
    }
}
