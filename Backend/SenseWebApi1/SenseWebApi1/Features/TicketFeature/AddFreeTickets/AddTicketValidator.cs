using FluentValidation;

namespace SenseWebApi1.Features.TicketFeature.AddFreeTickets;

// ReSharper disable once UnusedType.Global
public class AddFreeTicketValidator:AbstractValidator<AddTicketsCommand>
{
    public AddFreeTicketValidator()
    {
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedParameter.Local
        RuleFor(p => p.EventId).NotNull().NotEmpty().WithMessage("Пустое id события").WithErrorCode("400");
        RuleFor(p=>p.Countoftickets).NotNull().NotEmpty().WithMessage("Сколько именно вы хотите добавить билетов ?").LessThan(100).WithErrorCode("400");
    }
}