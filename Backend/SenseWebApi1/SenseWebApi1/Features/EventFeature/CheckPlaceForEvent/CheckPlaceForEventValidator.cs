using FluentValidation;

namespace SenseWebApi1.Features.EventFeature.CheckPlaceForEvent;

// ReSharper disable once UnusedType.Global
public class CheckPlaceForEventValidator: AbstractValidator<CheckPlaceForEventCommand>
{
    public CheckPlaceForEventValidator()
    {
        RuleFor(p => p.place).NotNull().NotEmpty().WithMessage("Место не может быть пустым").WithErrorCode("400"); 
        RuleFor(p=>p.eventId).NotNull().NotEmpty().WithMessage("id мероприятия не может быть пустым").WithErrorCode("400"); 
    }
}