using FluentValidation;

namespace SC.SenseTower.Admin.Cqrs.TowerEventUpdate
{
    public class TowerEventUpdateCommandValidator : AbstractValidator<TowerEventUpdateCommand>
    {
        public TowerEventUpdateCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Не указано название события");
            RuleFor(x => x.From)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано начало события")
                .Must((c, from) => from > DateTime.UtcNow).WithMessage("Нельзя изменять событие, которое уже идёт или прошло");
            RuleFor(x => x.UpTo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано завершение события")
                .Must((c, upTo) => upTo > c.From).WithMessage("Завершение события должно быть позже начала");
        }
    }
}
