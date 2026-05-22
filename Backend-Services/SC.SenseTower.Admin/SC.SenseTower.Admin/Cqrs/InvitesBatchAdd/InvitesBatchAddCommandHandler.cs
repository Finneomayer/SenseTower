using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.InvitesBatchAdd
{
    public class InvitesBatchAddCommandHandler : BaseHandler, IRequestHandler<InvitesBatchAddCommand, string[]>
    {
        private readonly InvitesService invitesService;

        public InvitesBatchAddCommandHandler(
            ILogger<InvitesBatchAddCommandHandler> logger,
            IMapper mapper,
            InvitesService invitesService) : base(logger, mapper)
        {
            this.invitesService = invitesService;
        }

        public async Task<string[]> Handle(InvitesBatchAddCommand request, CancellationToken cancellationToken)
        {
            return await invitesService.CreateBatch(request.AuthorId, request.UserId, request.Quantity, cancellationToken);
        }
    }
}
