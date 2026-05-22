using FluentValidation;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaDelete
{
    public class CinemaDeleteCommandValidator : AbstractValidator<CinemaDeleteCommand>
    {
        public CinemaDeleteCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Не указан идентификатор кинотеатра");
        }
    }
}
