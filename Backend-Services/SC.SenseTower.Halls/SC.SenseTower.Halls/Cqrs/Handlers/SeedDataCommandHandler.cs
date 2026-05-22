using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class SeedDataCommandHandler : BaseHandler, IRequestHandler<SeedDataCommand, Unit>
    {
        private readonly HallsService hallsService;
        private readonly SpacesService spacesService;

        public SeedDataCommandHandler(
            ILogger<SeedDataCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService,
            SpacesService spacesService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
            this.spacesService = spacesService;
        }

        public async Task<Unit> Handle(SeedDataCommand request, CancellationToken cancellationToken)
        {
            var halls = await hallsService.GetList(null, cancellationToken);
            foreach (var hall in halls)
            {
                var spaces = new List<LocalSpace>();
                foreach (var place in hall.UserPlaces.Where(x => x.LocalSpace != null))
                {
                    var space = await spacesService.Get(request.AccessToken, place.LocalSpace.Id, cancellationToken);
                    if (space == null)
                        continue;
                    spaces.Add(Mapper.Map<LocalSpace>(space));
                }
                foreach (var publicPlace in hall.PublicPlaces)
                {
                    var space = await spacesService.Get(request.AccessToken, publicPlace.Id, cancellationToken);
                    if (space == null)
                        continue;
                    spaces.Add(Mapper.Map<LocalSpace>(space));
                }
                hall.Spaces = spaces.ToArray();
                if (hall.Space != null)
                {
                    var space = await spacesService.Get(request.AccessToken, hall.Space.Id, cancellationToken);
                    if (space != null)
                        hall.Space = Mapper.Map<LocalSpace>(space);
                }
                await hallsService.Update(hall, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
