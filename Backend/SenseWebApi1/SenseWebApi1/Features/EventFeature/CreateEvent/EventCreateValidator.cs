using FluentValidation;

namespace SenseWebApi1.Features.EventFeature.CreateEvent;

public class EventCreateValidator : AbstractValidator<EventCreateCommand>
{
        
    public EventCreateValidator()
    {
            
        RuleFor(p => p.EventName).MinimumLength(5).WithMessage("Имя не может быть меньше чем 5 символов")
            .NotEmpty().WithMessage("Имя не может быть пустым").WithErrorCode("400"); 
        // ReSharper disable once ConvertClosureToMethodGroup
        // ReSharper disable once IdentifierTypo
        RuleFor(p => p.AreaId).NotEmpty().WithMessage("Пустое id пространства").WithErrorCode("400"); 
        RuleFor(p => p.Beginning).NotEmpty().WithMessage("Пустой момент начала").Must((eventOnj, state) => !(state > eventOnj.End)).WithName("Момент начала  позже чем момент окончания").WithErrorCode("400");
        RuleFor(p => p.End).NotEmpty().WithMessage("Пустой момент окончания").WithErrorCode("400"); 
        RuleFor(p => p.Description).NotNull().WithMessage("Пустое описание").WithErrorCode("400"); 
        // ReSharper disable once ConvertClosureToMethodGroup
        // ReSharper disable once IdentifierTypo
        RuleFor(p => p.ImageId).NotEmpty().WithMessage("Пустое id изображения").WithErrorCode("400");
        RuleFor(p => p.IsHavePlaces).NotEmpty().NotNull().WithMessage("Пустое поле наличия мест").WithErrorCode("400");
        RuleFor(p => p.Cost).NotNull().WithMessage("Пустое поле цены").Must(p => p >= 0)
            .WithMessage("Цена не может быть меньше нуля");
    }
}