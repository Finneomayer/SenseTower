using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Dto.UserInfo;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class UsersByIdsRequestHandler : BaseHandler, IRequestHandler<UsersByIdsRequest, IEnumerable<UserInfoDto>>
    {
        private readonly IdentityService identityService;

        public UsersByIdsRequestHandler(
            ILogger<UsersByIdsRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<IEnumerable<UserInfoDto>> Handle(UsersByIdsRequest request, CancellationToken cancellationToken)
        {
            var data = await identityService.GetByIds(request.AccessToken, request.UserIds, cancellationToken);
            var result = Mapper.Map<UserInfoDto[]>(data);
            return result ?? Array.Empty<UserInfoDto>();
        }
    }
}
