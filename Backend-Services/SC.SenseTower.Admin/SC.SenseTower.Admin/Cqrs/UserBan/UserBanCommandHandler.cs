using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.UserBan
{
    public class UserBanCommandHandler : BaseHandler, IRequestHandler<UserBanCommand, Unit>
    {
        private readonly IdentityService identityService;

        public UserBanCommandHandler(
            ILogger<UserBanCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<Unit> Handle(UserBanCommand request, CancellationToken cancellationToken)
        {
            await identityService.BanById(request.UserId, request.IsPermanent ? DateTime.MaxValue : request.LockoutEnd.ToUniversalTime(), cancellationToken);
            return Unit.Value;
        }
    }
}
