using FluentValidation;
using SC.SenseTower.Cinemas.Services;

namespace SC.SenseTower.Cinemas.Cqrs.AdminList
{
    public class AdminListCommandValidator : AbstractValidator<AdminListCommand>
    {
        public AdminListCommandValidator(CinemasService cinemasService)
        {
            RuleFor(x => x.CinemaId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор кинотеатра")
                .MustAsync(async (c, cinemaId, cancellationToken) =>
                {
                    var result = await cinemasService.Exists(cinemaId, cancellationToken);
                    return result;
                }).WithMessage("Кинотеатр не найден");
        }
    }
}
