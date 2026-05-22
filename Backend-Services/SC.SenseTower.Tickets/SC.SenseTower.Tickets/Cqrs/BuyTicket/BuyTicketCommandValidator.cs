using FluentValidation;

namespace SC.SenseTower.Tickets.Cqrs.BuyTicket
{
    public class BuyTicketCommandValidator : AbstractValidator<BuyTicketCommand>
    {
        public BuyTicketCommandValidator()
        {
            RuleFor(x => x.EventId).NotEmpty().WithMessage("Не указан идентификатор мероприятия");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Не указан идентификатор пользователя");
        }
    }
}
