using FluentValidation;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventTopList
{
    public class TowerEventTopListRequestValidator : AbstractValidator<TowerEventTopListRequest>
    {
        public TowerEventTopListRequestValidator()
        {
            RuleFor(x => x.Limit).Must((c, limit) => limit == null || limit > 0).WithMessage("Максимальное число записей должно быть больше 0");
        }
    }
}
