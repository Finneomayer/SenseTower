using FluentValidation;

namespace SenseWebApi1.Features.EventFeature.UpdateEvent;

// ReSharper disable once UnusedType.Global
public class EventUpdateCommandValidator : AbstractValidator<EventUpdateCommand>
{
    public  EventUpdateCommandValidator()
    {
            
            
            
        RuleFor(p => p.EventName).NotNull().NotEmpty().WithMessage("Неверное имя").WithErrorCode("400"); 
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedParameter.Local
        RuleFor(p => p.AreaId).NotEmpty().WithMessage("Пустое id пространства").WithErrorCode("400");
        RuleFor(p => p.Beginning).NotEmpty().WithMessage("Пустой момент начала").Must((eventOnj, state) => !(state > eventOnj.End)).WithName("Момент начала  позже чем момент окончания").WithErrorCode("400");
        RuleFor(p => p.End).NotEmpty().WithMessage("Пустой момент окончания").WithErrorCode("400");
        RuleFor(p => p.Description).NotNull().WithMessage("Пустое описание").WithErrorCode("400");
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedParameter.Local
        RuleFor(p => p.ImageId).NotEmpty().WithMessage("Пустое id изображения").WithErrorCode("400");
        RuleFor(p=>p.IsHavePlaces).NotNull().WithMessage("Пустое значение есть ли билеты").WithErrorCode("400");
            
    }
}