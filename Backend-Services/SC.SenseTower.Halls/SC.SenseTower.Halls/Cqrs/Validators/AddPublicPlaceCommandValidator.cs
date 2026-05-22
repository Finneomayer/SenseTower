using FluentValidation;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class AddPublicPlaceCommandValidator : AbstractValidator<AddPublicPlaceCommand>
    {
        public AddPublicPlaceCommandValidator(HallsService hallsService, SpacesService spacesService)
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
            RuleFor(x => x.SpaceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано пространство")
                .MustAsync(async (c, spaceId, cancellationToken) =>
                {
                    var space = await spacesService.GetSpace(c.AccessToken, spaceId, cancellationToken);
                    return space != null;
                }).WithMessage("Пространство не найдено");
        }
    }
}
