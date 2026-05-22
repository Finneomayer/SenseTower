using FluentValidation;

namespace SC.SenseTower.Admin.Cqrs.TicketsBatchAdd
{
    public class TicketsBatchAddCommandValidator : AbstractValidator<TicketsBatchAddCommand>
    {
        public TicketsBatchAddCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Не указан пользователь.");
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Количество билетов должно быть больше 0.")
                .LessThanOrEqualTo(20).WithMessage("Количество билетов не может превышать 20.");
        }
    }
}
