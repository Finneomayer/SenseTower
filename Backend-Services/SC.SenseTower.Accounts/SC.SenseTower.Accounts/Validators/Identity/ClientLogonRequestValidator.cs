using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Requests;

namespace SC.SenseTower.Accounts.Validators.Identity
{
    public class ClientLogonRequestValidator : AbstractValidator<ClientLogonRequest>
    {
        public ClientLogonRequestValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty().WithMessage("Не указан код клиента.");
        }
    }
}
