using MediatR;
using SC.SenseTower.Accounts.Dto.Identity;

namespace SC.SenseTower.Accounts.Cqrs.Requests
{
    public class ClientLogonRequest : IRequest<ClientLogonResultDto>
    {
        /// <summary>
        /// Идентификатор приложения-клиента.
        /// </summary>
        public string ClientId { get; set; } = null!;
    }
}
