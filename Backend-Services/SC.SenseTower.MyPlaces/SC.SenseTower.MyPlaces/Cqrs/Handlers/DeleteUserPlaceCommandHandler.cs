using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class DeleteUserPlaceCommandHandler : BaseHandler, IRequestHandler<DeleteUserPlaceCommand, bool>
    {
        private readonly PlacesService placesService;

        public DeleteUserPlaceCommandHandler(
            ILogger<DeleteUserPlaceCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<bool> Handle(DeleteUserPlaceCommand request, CancellationToken cancellationToken)
        {
            await placesService.Delete(request.AccessToken, request.PlaceId, cancellationToken);
            return true;
        }
    }
}
