using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Dto.Halls;
using SC.SenseTower.Halls.Dto.Places;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class AddUserPlaceCommandHandler : BaseHandler, IRequestHandler<AddUserPlaceCommand, Unit>
    {
        private readonly HallsService hallsService;
        private readonly MyPlacesService placesService;

        public AddUserPlaceCommandHandler(
            ILogger<AddUserPlaceCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService,
            MyPlacesService placesService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
            this.placesService = placesService;
        }

        public async Task<Unit> Handle(AddUserPlaceCommand request, CancellationToken cancellationToken)
        {
            var userPlace = (await placesService.GetPlacesByIds(request.AccessToken, new[] { request.PlaceId }, cancellationToken)).First();
            var place = Mapper.Map<Place>(userPlace);

            var hall = await hallsService.Get(request.HallId, cancellationToken);
            hall.UserPlaces = hall.UserPlaces
                .Union(new[] { place })
                .ToArray();
            await hallsService.Update(hall, cancellationToken);

            return Unit.Value;
        }
    }
}
