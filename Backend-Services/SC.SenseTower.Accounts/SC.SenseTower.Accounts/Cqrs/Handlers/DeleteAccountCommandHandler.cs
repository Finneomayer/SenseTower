using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class DeleteAccountCommandHandler : BaseHandler, IRequestHandler<DeleteAccountCommand, bool>
    {
        private readonly IdentityService identityService;

        public DeleteAccountCommandHandler(
            ILogger<DeleteAccountCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var result = await identityService.DeleteUser(request.AccessToken, request.UserId, cancellationToken);
            return result;
        }
    }
}
