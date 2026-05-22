using FluentValidation;

namespace SenseWebApi1.Features.TicketFeature.SellTicketForUser;

// ReSharper disable once UnusedType.Global
public class SellTicketForUserValidator:AbstractValidator<SellTicketForUserCommand>
{
    
    public SellTicketForUserValidator()
    {
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedParameter.Local
        RuleFor(p => p.TicketId).NotNull().NotEmpty().WithMessage("Пустое id события").WithMessage("Такого билета нет").WithErrorCode("400");
        RuleFor(p => p.OwnerId).NotNull().NotEmpty().WithMessage("Пустое id пользователя").WithErrorCode("400");
    }
}