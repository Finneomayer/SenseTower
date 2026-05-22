using FluentValidation;

namespace SenseWebApi1.Features.TicketFeature.GiveTicketForUser;

// ReSharper disable once UnusedType.Global
public class GiveTicketForUserValidator : AbstractValidator<GiveTicketForUserCommand>
{
    public GiveTicketForUserValidator()
    {
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedParameter.Local
        RuleFor(p => p.TicketId).NotNull().NotEmpty().WithMessage("Пустое id события").WithErrorCode("400");
        RuleFor(p => p.OwnerId).NotNull().NotEmpty().WithMessage("Пустое id пользователя").WithErrorCode("400");
            
    }
}