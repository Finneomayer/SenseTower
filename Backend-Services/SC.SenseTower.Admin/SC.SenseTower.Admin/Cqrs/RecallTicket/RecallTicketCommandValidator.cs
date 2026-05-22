using FluentValidation;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Admin.Services;
using System.Net.Sockets;

namespace SC.SenseTower.Admin.Cqrs.RecallTicket
{
    public class RecallTicketCommandValidator : AbstractValidator<RecallTicketCommand>
    {
        private Ticket? ticket = null;

        public RecallTicketCommandValidator(GuestInvitesService ticketsService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан код билета.")
                .MustAsync(async (c, ticketId, cancellationToken) =>
                {
                    ticket ??= await ticketsService.Get(ticketId, cancellationToken);
                    return ticket != null;
                }).WithMessage("Билет с таким кодом не существует.")
                .MustAsync(async (c, ticketId, cancellationToken) =>
                {
                    ticket ??= await ticketsService.Get(ticketId, cancellationToken);
                    return ticket.UsingInfo?.Date == null;
                }).WithMessage("Билет уже использован, невозможно отозвать.")
                .MustAsync(async (c, ticketId, cancellationToken) =>
                {
                    ticket ??= await ticketsService.Get(ticketId, cancellationToken);
                    return !(ticket.RecallInfo?.IsRecalled ?? false);
                }).WithMessage("Билет уже отозван, невозможно отозвать повторно.");
        }
    }
}
