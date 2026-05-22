using FluentValidation;
using SC.SenseTower.Cinemas.Services;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaUpdate
{
    public class CinemaUpdateCommandValidator : AbstractValidator<CinemaUpdateCommand>
    {
        public CinemaUpdateCommandValidator(CinemasService cinemasService, SpacesService spacesService)
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано название кинотеатра")
                .MustAsync(async (c, name, cancellationToken) =>
                {
                    var cinemas = await cinemasService.Get(cancellationToken);
                    return !cinemas.Any(x => x.Id != c.Id && x.Name == name);
                }).WithMessage("Кинотеатр с таким названием уже существует");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для выполнения операции");
            RuleFor(x => x.SpaceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано пространство")
                .MustAsync(async (c, spaceId, cancellationToken) =>
                {
                    var space = await spacesService.GetSpace(c.AccessToken, spaceId, cancellationToken);
                    return space != null;
                }).WithMessage("Указанное пространство не существует");
        }
    }
}
