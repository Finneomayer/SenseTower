using MediatR;
using SC.SenseTower.Accounts.Dto.Identity;

namespace SC.SenseTower.Accounts.Cqrs.Requests
{
    public class RefreshUserTokenRequest : IRequest<RefreshUserTokenResultDto>
    {
        /// <summary>
        /// Токен обновления.
        /// </summary>
        public string RefreshToken { get; set; } = null!;
    }
}
