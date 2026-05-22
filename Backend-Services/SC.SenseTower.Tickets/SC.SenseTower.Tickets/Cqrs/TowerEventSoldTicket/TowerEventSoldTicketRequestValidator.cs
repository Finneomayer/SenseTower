using FluentValidation;

namespace SC.SenseTower.Tickets.Cqrs.TowerEventSoldTicket
{
    public class TowerEventSoldTicketRequestValidator : AbstractValidator<TowerEventSoldTicketRequest>
    {
        public TowerEventSoldTicketRequestValidator()
        {
            RuleFor(x => x.EventId).NotEmpty().WithMessage("Не указан идентификатор события");
        }
    }
}
