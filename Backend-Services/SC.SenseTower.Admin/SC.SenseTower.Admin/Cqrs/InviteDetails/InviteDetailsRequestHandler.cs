using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Invites;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using System.Linq;

namespace SC.SenseTower.Admin.Cqrs.InviteDetails
{
    public class InviteDetailsRequestHandler : BaseHandler, IRequestHandler<InviteDetailsRequest, InviteDetailsDto>
    {
        private readonly InvitesService invitesService;
        private readonly IdentityService identityService;

        public InviteDetailsRequestHandler(
            ILogger<InviteDetailsRequestHandler> logger,
            IMapper mapper,
            InvitesService invitesService,
            IdentityService identityService) : base(logger, mapper)
        {
            this.invitesService = invitesService;
            this.identityService = identityService;
        }

        public async Task<InviteDetailsDto> Handle(InviteDetailsRequest request, CancellationToken cancellationToken)
        {
            var invite = await invitesService.Get(request.InviteId, cancellationToken);
            var result = Mapper.Map<InviteDetailsDto>(invite);

            var userIds = new List<Guid?>();
            if (result.AuthorId != null)
                userIds.Add(result.AuthorId.Value);
            if (result.IssuerId != null && result.IssuerId != result.AuthorId)
                userIds.Add(result.IssuerId.Value);
            if (result.UserId != null && result.UserId != result.AuthorId && result.UserId != result.IssuerId)
                userIds.Add(result.UserId.Value);
            if (userIds.Count > 0)
            {
                var users = await identityService.GetUserLookups(userIds, cancellationToken);
                if (users != null)
                {
                    if (result.AuthorId != null)
                        result.AuthorName = users.FirstOrDefault(r => r.Id == result.AuthorId.Value)?.Name;
                    if (result.IssuerId != null)
                        result.IssuerName = users.FirstOrDefault(r => r.Id == result.IssuerId.Value)?.Name;
                    if (result.UserId != null)
                        result.UserName = users.FirstOrDefault(r => r.Id == result.UserId.Value)?.Name;
                }
            }

            return result;
        }
    }
}
