using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Dto.UserInfo;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class GetUserInfoRequestHandler : BaseHandler, IRequestHandler<GetUserInfoRequest, UserInfoDto?>
    {
        private readonly IdentityService identityService;

        public GetUserInfoRequestHandler(
            ILogger<GetUserInfoRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<UserInfoDto?> Handle(GetUserInfoRequest request, CancellationToken cancellationToken)
        {
            var result = await identityService.GetUserInfo(request.AccessToken, request.UserId, cancellationToken);
            return result;
        }
    }
}
