using FluentValidation;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class AddUserPlaceCommandValidator : AbstractValidator<AddUserPlaceCommand>
    {
        public AddUserPlaceCommandValidator(HallsService hallsService, MyPlacesService placesService)
        {
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для выполнения операции");
            RuleFor(x => x.HallId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан холл")
                .MustAsync(async (c, hallId, cancellationToken) =>
                {
                    var hall = await hallsService.Get(hallId, cancellationToken);
                    return hall != null;
                }).WithMessage("Холл не найден");
            RuleFor(x => x.PlaceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано помещение")
                .MustAsync(async (c, placeId, cancellationToken) =>
                {
                    var places = await placesService.GetPlacesByIds(c.AccessToken, new[] { placeId }, cancellationToken);
                    return places.Length > 0;
                }).WithMessage("Помещение не найдено")
                .MustAsync(async (c, placeId, cancellationToken) =>
                {
                    var place = await hallsService.GetByPlaceId(placeId, cancellationToken);
                    return place == null;
                }).WithMessage("Помещение уже добавлено в холл");
        }
    }
}
