using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.RecallInvite
{
    public class RecallInviteCommandHandler : BaseHandler, IRequestHandler<RecallInviteCommand, Unit>
    {
        private readonly InvitesService invitesService;

        public RecallInviteCommandHandler(
            ILogger<RecallInviteCommandHandler> logger,
            IMapper mapper,
            InvitesService invitesService) : base(logger, mapper)
        {
            this.invitesService = invitesService;
        }

        public async Task<Unit> Handle(RecallInviteCommand request, CancellationToken cancellationToken)
        {
            await invitesService.Recall(request.Id, request.Reason, cancellationToken);
            return Unit.Value;
        }
    }
}
