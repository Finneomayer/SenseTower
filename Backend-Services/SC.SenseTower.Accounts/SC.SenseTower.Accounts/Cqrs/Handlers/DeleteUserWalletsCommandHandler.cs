using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class DeleteUserWalletsCommandHandler : BaseHandler, IRequestHandler<DeleteUserWalletsCommand, bool>
    {
        private readonly WalletsService walletsService;

        public DeleteUserWalletsCommandHandler(
            ILogger<DeleteUserWalletsCommandHandler> logger,
            IMapper mapper,
            WalletsService walletsService) : base(logger, mapper)
        {
            this.walletsService = walletsService;
        }

        public async Task<bool> Handle(DeleteUserWalletsCommand request, CancellationToken cancellationToken)
        {
            return await walletsService.DeleteByUser(request.OwnerId, cancellationToken).ConfigureAwait(false);
        }
    }
}
