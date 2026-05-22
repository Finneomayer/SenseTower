using FluentValidation;
using SC.SenseTower.Spaces.Constants;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Validators
{
    public class ReplaceImagesCommandValidator : AbstractValidator<ReplaceImagesCommand>
    {
        private Space? space = null;

        public ReplaceImagesCommandValidator(ISpacesService spacesService)
        {
            RuleFor(x => x.SpaceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано помещение")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    space ??= await spacesService.Get(id, cancellationToken);
                    return space != null;
                }).WithMessage("Помещение не найдено")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    if (c.Role == RoleNames.VR_ADMIN)
                        return true;
                    space ??= await spacesService.Get(id, cancellationToken);
                    return space?.SpaceOwner?.UserId == c.UserId;
                }).WithMessage("Вы не владелец помещения");
        }
    }
}
