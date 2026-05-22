using FluentValidation;
using SC.SenseTower.Cinemas.Services;

namespace SC.SenseTower.Cinemas.Cqrs.AdminReplaceAll
{
    public class AdminReplaceAllCommandValidator : AbstractValidator<AdminReplaceAllCommand>
    {
        public AdminReplaceAllCommandValidator(CinemasService cinemasService, AccountsService accountsService)
        {
            RuleFor(x => x.CinemaId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор кинотеатра")
                .MustAsync(async (c, cinemaId, cancellationToken) =>
                {
                    var result = await cinemasService.Exists(cinemaId, cancellationToken);
                    return result;
                }).WithMessage("Кинотеатр не найден");
            RuleFor(x => x.UserIds)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор пользователя")
                .MustAsync(async (c, userIds, cancellationToken) =>
                {
                    var result = await accountsService.Lookup(c.AccessToken, userIds, cancellationToken);
                    return result.Count() == userIds.Length;
                }).WithMessage("Некоторые пользователи не найдены");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Необходимо аторизоваться");
        }
    }
}
