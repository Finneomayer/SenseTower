using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Dto.UserInfo;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Extensions;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class GetCurrentUserInfoRequestHandler : BaseHandler, IRequestHandler<GetCurrentUserInfoRequest, UserInfoDto?>
    {
        private readonly IdentityService identityService;
        private readonly InvitesService invitesService;
        private readonly AccountsService accountsService;

        public GetCurrentUserInfoRequestHandler(
            ILogger<GetCurrentUserInfoRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService,
            InvitesService invitesService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.identityService = identityService;
            this.invitesService = invitesService;
            this.accountsService = accountsService;
        }

        public async Task<UserInfoDto?> Handle(GetCurrentUserInfoRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await identityService.GetIdentityInfo(request.Token ?? "", cancellationToken);
            var result = Mapper.Map<UserInfoDto>(userInfo);

            var account = await accountsService.Get(userInfo.UserId, cancellationToken);
            result.AvatarNumber = account.AvatarNumber;

            var invites = await invitesService.GetUserInvites(result.UserId, cancellationToken);
            result.Invites = Mapper.Map<UserInviteDto[]>(invites);
            var userIds = result.Invites
                .Where(r => r.UsingInfo != null && r.UsingInfo.UserId != null)
                .Select(r => r.UsingInfo?.UserId ?? default)
                .Distinct()
                .ToArray();
            if (userIds.Length > 0)
            {
                var users = await identityService.LookupUsers(userIds, null, request.Token, cancellationToken );
                if (users != null && users.Length > 0)
                {
                    result.Invites
                        .ForEach(r =>
                        {
                            if (r.UsingInfo != null)
                            {
                                r.UsingInfo.UserName = users.FirstOrDefault(u => u.Id == r.UsingInfo.UserId)?.Name;
                            }
                        });
                }
            }

            return result;
        }
    }
}
