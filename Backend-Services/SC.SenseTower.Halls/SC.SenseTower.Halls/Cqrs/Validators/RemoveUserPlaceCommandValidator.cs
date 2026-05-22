using FluentValidation;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class RemoveUserPlaceCommandValidator : AbstractValidator<RemoveUserPlaceCommand>
    {
        public RemoveUserPlaceCommandValidator(HallsService hallsService)
        {
            RuleFor(x => x.HallId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан холл")
                .MustAsync(async (c, hallId, cancellationToken) =>
                {
                    var hall = await hallsService.Get(hallId, cancellationToken);
                    return hall != null;
                }).WithMessage("Холл не найден");
        }
    }
}
