using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class RemovePublicPlaceCommandHandler : BaseHandler, IRequestHandler<RemovePublicPlaceCommand, Unit>
    {
        private readonly HallsService hallsService;

        public RemovePublicPlaceCommandHandler(
            ILogger<RemovePublicPlaceCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<Unit> Handle(RemovePublicPlaceCommand request, CancellationToken cancellationToken)
        {
            var hall = await hallsService.Get(request.HallId, cancellationToken);
            hall.PublicPlaces = hall.PublicPlaces
                .Where(x => x.Id != request.SpaceId)
                .ToArray();
            await hallsService.Update(hall, cancellationToken);
            return Unit.Value;
        }
    }
}
