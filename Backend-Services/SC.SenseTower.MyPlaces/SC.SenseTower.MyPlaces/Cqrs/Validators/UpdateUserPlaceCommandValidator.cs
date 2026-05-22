using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class UpdateUserPlaceCommandValidator : AbstractValidator<UpdateUserPlaceCommand>
    {
        private Place? place = null;

        public UpdateUserPlaceCommandValidator(PlacesService placesService, AccountsService accountsService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не выбрано помещение.")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    place ??= (await placesService.GetPlacesByIds(new Guid[] { id }, cancellationToken))?.FirstOrDefault();
                    return place != null;
                }).WithMessage("Помещение не найдено.")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    var userInfo = await accountsService.GetInfo(c.AccessToken, cancellationToken);
                    if (userInfo?.Role.ToLower() == "vr_admin")
                        return true;
                    place ??= (await placesService.GetPlacesByIds(new Guid[] { id }, cancellationToken))?.FirstOrDefault();
                    return place?.OwnerId == c.UserId;
                }).WithMessage("Редактировать помещение может только владелец или администратор.");
        }
    }
}
