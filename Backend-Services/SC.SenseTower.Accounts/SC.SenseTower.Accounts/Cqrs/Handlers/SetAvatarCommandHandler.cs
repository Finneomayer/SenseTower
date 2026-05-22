using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class SetAvatarCommandHandler : BaseHandler, IRequestHandler<SetAvatarCommand, bool>
    {
        private readonly AccountsService accountsService;

        public SetAvatarCommandHandler(
            ILogger<SetAvatarCommandHandler> logger,
            IMapper mapper,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.accountsService = accountsService;
        }

        public async Task<bool> Handle(SetAvatarCommand request, CancellationToken cancellationToken)
        {
            await accountsService.SetAvatar(request.UserId, request.AvatarNumber, cancellationToken);
            return true;
        }
    }
}
