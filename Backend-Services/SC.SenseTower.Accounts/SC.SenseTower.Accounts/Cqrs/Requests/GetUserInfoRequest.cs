using MediatR;
using SC.SenseTower.Accounts.Dto.UserInfo;

namespace SC.SenseTower.Accounts.Cqrs.Requests
{
    public class GetUserInfoRequest : IRequest<UserInfoDto?>
    {
        public Guid UserId { get; set; }

        public string? AccessToken { get; set; }
    }
}
