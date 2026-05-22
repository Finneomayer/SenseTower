using FluentValidation;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventUpdate
{
    public class TowerEventUpdateCommandValidator : AbstractValidator<TowerEventUpdateCommand>
    {
        private Data.Models.TowerEvent? towerEvent = null;

        public TowerEventUpdateCommandValidator(
            TowerEventsService towerEventsService,
            SpacesService spacesService,
            ImagesService imagesService)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Не указано название мероприятия");
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор мероприятия")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    towerEvent ??= await towerEventsService.Get(id, cancellationToken);
                    return towerEvent != null;
                }).WithMessage("Указанное мероприятие не найдено")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    towerEvent ??= await towerEventsService.Get(id, cancellationToken);
                    return towerEvent == null || towerEvent.State != c.State || towerEvent.State == TowerEventState.Planned;
                }).WithMessage("Изменить можно только запланированное мероприятие");
            RuleFor(x => x.Date)
                .GreaterThan(DateTimeOffset.Now).WithMessage("Начало события должно быть больше текущего времени");
            RuleFor(x => x.UpTo)
                .Cascade(CascadeMode.Stop)
                .Must((c, upTo) => upTo > c.Date).WithMessage("Окончание события должно быть позже начала")
                .Must((c, upTo) => upTo.Subtract(c.Date).TotalDays <= 1).WithMessage("Мероприятие не может длиться больше суток");
            RuleFor(x => x.SpaceId)
                .MustAsync(async (c, spaceId, cancellationToken) =>
                {
                    if (spaceId == null)
                        return true;
                    var space = await spacesService.Get(c.AccessToken, spaceId.Value, cancellationToken);
                    return space != null;
                }).WithMessage("Указанное пространство не найдено");
            RuleFor(x => x.ImageId)
                .MustAsync(async (c, imageId, cancellationToken) =>
                {
                    if (imageId == null)
                        return true;
                    var image = await imagesService.Get(c.AccessToken, imageId.Value, cancellationToken);
                    return image != null;
                }).WithMessage("Указанное изображение не найдено");
        }
    }
}
