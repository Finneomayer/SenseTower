using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.UserUnban
{
    public class UserUnbanCommandHandler : BaseHandler, IRequestHandler<UserUnbanCommand, Unit>
    {
        private readonly IdentityService identityService;

        public UserUnbanCommandHandler(
            ILogger<UserUnbanCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<Unit> Handle(UserUnbanCommand request, CancellationToken cancellationToken)
        {
            await identityService.UnbanById(request.UserId, cancellationToken);
            return Unit.Value;
        }
    }
}
