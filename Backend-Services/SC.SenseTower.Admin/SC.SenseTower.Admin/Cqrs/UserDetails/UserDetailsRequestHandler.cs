using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Admin.Dto.Users;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.UserDetails
{
    public class UserDetailsRequestHandler : BaseHandler, IRequestHandler<UserDetailsRequest, UserDetailsDto>
    {
        private readonly IdentityService identityService;
        private readonly InvitesService invitesService;
        private readonly WalletsService walletsService;
        private readonly PlacesService placesService;

        public UserDetailsRequestHandler(
            ILogger<UserDetailsRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService,
            InvitesService invitesService,
            WalletsService walletsService,
            PlacesService placesService) : base(logger, mapper)
        {
            this.identityService = identityService;
            this.invitesService = invitesService;
            this.walletsService = walletsService;
            this.placesService = placesService;
        }

        public async Task<UserDetailsDto> Handle(UserDetailsRequest request, CancellationToken cancellationToken)
        {
            var user = await identityService.Get(request.UserId, cancellationToken);
            var result = Mapper.Map<ApplicationUser, UserDetailsDto>(user, o =>
            {
                o.AfterMap((s, d) =>
                {
                    d.AvailableRoles = identityService.GetRoleLookups(null, cancellationToken).GetAwaiter().GetResult();

                    var invites = invitesService.GetUserInvites(s.Id, cancellationToken).GetAwaiter().GetResult();
                    var userIds = invites.Select(r => r.UsingInfo.UserId).Distinct().ToArray();
                    var userLookups = userIds.Length > 0
                        ? identityService.GetUserLookups(userIds, cancellationToken).GetAwaiter().GetResult()
                        : Array.Empty<LookupItemDto<Guid>>();
                    d.Invites = Mapper.Map<IEnumerable<Invite>, UserInviteListItemDto[]>(invites, o =>
                    {
                        o.AfterMap((s, d) =>
                        {
                            foreach (var item in d)
                            {
                                var src = s.First(r => r.Id == item.Id);
                                if (src?.UsingInfo?.UserId == null)
                                    continue;
                                item.UserName = userLookups.FirstOrDefault(r => r.Id == src.UsingInfo.UserId)?.Name;
                            }
                        });
                    });

                    var wallets = walletsService.GetUserWallets(s.Id, cancellationToken).GetAwaiter().GetResult();
                    d.Wallets = Mapper.Map<UserWalletListItemDto[]>(wallets);

                    d.Places = placesService.GetUserPlaces(request.AccessToken, request.RefreshToken, s.Id, cancellationToken).GetAwaiter().GetResult();
                });
            });

            return result;
        }
    }
}
