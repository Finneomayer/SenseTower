using FluentValidation;
using SC.SenseTower.Admin.Services;

namespace SC.SenseTower.Admin.Cqrs.TicketDetails
{
    public class InviteDetailsRequestValidator : AbstractValidator<TicketDetailsRequest>
    {
        public InviteDetailsRequestValidator(GuestInvitesService invitesService)
        {
            RuleFor(x => x.TicketId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан код билета.")
                .MustAsync(async (c, ticketId, cancellationToken) =>
                {
                    var ticket = await invitesService.Get(ticketId, cancellationToken);
                    return ticket != null;
                }).WithMessage("Билета с таким кодом не существует.");
        }
    }
}
