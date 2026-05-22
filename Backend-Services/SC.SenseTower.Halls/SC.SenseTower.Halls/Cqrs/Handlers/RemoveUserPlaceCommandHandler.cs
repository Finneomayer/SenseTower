using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class RemoveUserPlaceCommandHandler : BaseHandler, IRequestHandler<RemoveUserPlaceCommand, Unit>
    {
        private readonly HallsService hallsService;

        public RemoveUserPlaceCommandHandler(
            ILogger<RemoveUserPlaceCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<Unit> Handle(RemoveUserPlaceCommand request, CancellationToken cancellationToken)
        {
            var hall = await hallsService.Get(request.HallId, cancellationToken);
            hall.UserPlaces = hall.UserPlaces
                .Where(x => x.Id != request.PlaceId)
                .ToArray();
            await hallsService.Update(hall, cancellationToken);
            return Unit.Value;
        }
    }
}
