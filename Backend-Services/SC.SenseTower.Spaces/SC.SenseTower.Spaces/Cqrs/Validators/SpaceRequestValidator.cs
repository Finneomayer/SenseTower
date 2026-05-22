using FluentValidation;
using SC.SenseTower.Spaces.Cqrs.Requests;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Validators
{
    public class SpaceRequestValidator : AbstractValidator<SpaceRequest>
    {
        public SpaceRequestValidator(ISpacesService spacesService)
        {
            RuleFor(x => x.SpaceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор пространства")
                .MustAsync(async (c, spaceId, cancellationToken) =>
                {
                    var space = await spacesService.Get(spaceId, cancellationToken);
                    return space != null;
                }).WithMessage("Пространство не найдено");
        }
    }
}
