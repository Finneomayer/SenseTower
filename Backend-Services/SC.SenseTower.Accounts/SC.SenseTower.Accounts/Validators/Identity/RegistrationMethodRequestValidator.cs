using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Requests;

namespace SC.SenseTower.Accounts.Validators.Identity
{
    public class RegistrationMethodRequestValidator : AbstractValidator<RegistrationMethodRequest>
    {
        public RegistrationMethodRequestValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Необходимо указать код билета / приглашения");
        }
    }
}
