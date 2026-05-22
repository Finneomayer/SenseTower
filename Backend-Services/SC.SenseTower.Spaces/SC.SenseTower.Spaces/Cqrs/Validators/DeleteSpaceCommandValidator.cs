using FluentValidation;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Validators
{
    public class DeleteSpaceCommandValidator : AbstractValidator<DeleteSpaceCommand>
    {
        public DeleteSpaceCommandValidator(ISpacesService spacesService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано пространство")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    var space = await spacesService.Get(id, cancellationToken);
                    return space != null;
                }).WithMessage("Пространство не найдено");
        }
    }
}
