using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class RecallInviteCommandHandler : BaseHandler, IRequestHandler<RecallInviteCommand, bool>
    {
        private readonly InvitesService invitesService;

        public RecallInviteCommandHandler(
            ILogger<RecallInviteCommandHandler> logger,
            IMapper mapper,
            InvitesService invitesService) : base(logger, mapper)
        {
            this.invitesService = invitesService;
        }

        public async Task<bool> Handle(RecallInviteCommand request, CancellationToken cancellationToken)
        {
            return await invitesService.Recall(request.InviteId, request.Reason, cancellationToken);
        }
    }
}
