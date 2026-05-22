using MediatR;
using SC.SenseTower.Accounts.Dto.UserInfo;

namespace SC.SenseTower.Accounts.Cqrs.Requests
{
    public class GetCurrentUserInfoRequest : IRequest<UserInfoDto?>
    {
        public string? Token { get; set; }
    }
}
