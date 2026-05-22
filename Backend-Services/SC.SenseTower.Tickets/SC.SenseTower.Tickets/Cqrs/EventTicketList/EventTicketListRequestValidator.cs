using FluentValidation;

namespace SC.SenseTower.Tickets.Cqrs.EventTicketList
{
    public class EventTicketListRequestValidator : AbstractValidator<EventTicketListRequest>
    {
        public EventTicketListRequestValidator()
        {
            RuleFor(x => x.EventId).NotEmpty().WithMessage("Не задан идентификатор мероприятия");
        }
    }
}
