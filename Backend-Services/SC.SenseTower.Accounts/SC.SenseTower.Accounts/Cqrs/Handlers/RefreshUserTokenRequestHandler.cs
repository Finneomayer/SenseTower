using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Dto.Identity;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class RefreshUserTokenRequestHandler : BaseHandler, IRequestHandler<RefreshUserTokenRequest, RefreshUserTokenResultDto>
    {
        private readonly IdentityService identityService;

        public RefreshUserTokenRequestHandler(
            ILogger<RefreshUserTokenRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<RefreshUserTokenResultDto> Handle(RefreshUserTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await identityService.Refresh(request.RefreshToken, cancellationToken);
            return result;
        }
    }
}
