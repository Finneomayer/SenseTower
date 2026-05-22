using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Requests
{
    public class RegistrationMethodRequest : IRequest<string?>
    {
        public string Code { get; set; } = null!;
    }
}
