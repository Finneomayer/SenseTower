using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class GenerateInvitesCommandHandler : BaseHandler, IRequestHandler<GenerateInvitesCommand, string[]>
    {
        private readonly InvitesService invitesService;

        public GenerateInvitesCommandHandler(
            ILogger<GenerateInvitesCommandHandler> logger,
            IMapper mapper,
            InvitesService invitesService) : base(logger, mapper)
        {
            this.invitesService = invitesService;
        }

#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
        public async Task<string[]> Handle(GenerateInvitesCommand request, CancellationToken cancellationToken)
        {
            return await invitesService.CreateUserInvites(request.UserId.Value, request.UserId.Value, request.Quantity, cancellationToken);
        }
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.
    }
}
