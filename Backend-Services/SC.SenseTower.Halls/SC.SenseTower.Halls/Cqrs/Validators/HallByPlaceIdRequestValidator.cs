using FluentValidation;
using SC.SenseTower.Halls.Cqrs.Requests;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class HallByPlaceIdRequestValidator : AbstractValidator<HallByPlaceIdRequest>
    {
        public HallByPlaceIdRequestValidator()
        {
            RuleFor(x => x.PlaceId).NotEmpty().WithMessage("Не задан идентификатор помещения");
        }
    }
}
