using FluentValidation;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Features.Likes;

public class LikeCommandValidator : AbstractValidator<LikeCommand>
{
    public LikeCommandValidator(ISpacesService spacesService)
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Не указан идентификатор пространства")
            .MustAsync(async (c, spaceId, cancellationToken) =>
            {
                var space = await spacesService.Get(spaceId, cancellationToken);
                return space != null;
            }).WithMessage("Пространство не найдено");
    }
}