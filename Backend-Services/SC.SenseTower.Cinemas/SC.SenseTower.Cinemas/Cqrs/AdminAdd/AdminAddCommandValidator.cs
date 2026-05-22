using FluentValidation;
using SC.SenseTower.Cinemas.Services;

namespace SC.SenseTower.Cinemas.Cqrs.AdminAdd
{
    public class AdminAddCommandValidator : AbstractValidator<AdminAddCommand>
    {
        public AdminAddCommandValidator(CinemasService cinemasService, AccountsService accountsService)
        {
            RuleFor(x => x.CinemaId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор кинотеатра")
                .MustAsync(async (c, cinemaId, cancellationToken) =>
                {
                    var result = await cinemasService.Exists(cinemaId, cancellationToken);
                    return result;
                }).WithMessage("Кинотеатр не найден");
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор пользователя")
                .MustAsync(async (c, userId, cancellationToken) =>
                {
                    var result = await accountsService.Exists(c.AccessToken, userId, cancellationToken);
                    return result;
                }).WithMessage("Пользователь не найден");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Необходимо авторизоваться");
        }
    }
}
