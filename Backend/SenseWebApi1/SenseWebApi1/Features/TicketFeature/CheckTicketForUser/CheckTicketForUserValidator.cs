using FluentValidation;

namespace SenseWebApi1.Features.TicketFeature.CheckTicketForUser;

// ReSharper disable once UnusedType.Global
public class CheckTicketForUserValidator:AbstractValidator<CheckTicketForUserCommand>
{
    public CheckTicketForUserValidator()
    {
        // ReSharper disable once IdentifierTypo
        RuleFor(p => p.EventId).NotNull().NotEmpty().WithMessage("Пустое id события")
            // ReSharper disable once UnusedParameter.Local
            .WithErrorCode("400");
        RuleFor(p => p.OwnerId).NotNull().NotEmpty().WithMessage("Пустое id пользователя").WithErrorCode("400");
    }
        
}