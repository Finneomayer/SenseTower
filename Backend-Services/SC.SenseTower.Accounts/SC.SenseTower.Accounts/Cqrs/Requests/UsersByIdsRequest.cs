using MediatR;
using SC.SenseTower.Accounts.Dto.UserInfo;

namespace SC.SenseTower.Accounts.Cqrs.Requests
{
    public class UsersByIdsRequest : IRequest<IEnumerable<UserInfoDto>>
    {
        public Guid[]? UserIds { get; set; }

        public string AccessToken { get; set; } = null!;
    }
}
