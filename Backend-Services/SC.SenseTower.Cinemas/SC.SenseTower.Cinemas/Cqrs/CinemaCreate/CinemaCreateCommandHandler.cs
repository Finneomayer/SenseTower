using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaCreate
{
    public class CinemaCreateCommandHandler : BaseHandler, IRequestHandler<CinemaCreateCommand, Guid>
    {
        private readonly CinemasService cinemasService;
        private readonly SpacesService spacesService;

        public CinemaCreateCommandHandler(
            ILogger<CinemaCreateCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService,
            SpacesService spacesService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
            this.spacesService = spacesService;
        }

        public async Task<Guid> Handle(CinemaCreateCommand request, CancellationToken cancellationToken)
        {
            var spaceDto = await spacesService.GetSpace(request.AccessToken, request.SpaceId, cancellationToken);
            var cinema = Mapper.Map<Data.Models.Cinema>(request);
            cinema.Space = Mapper.Map<Space>(spaceDto);
            var result = await cinemasService.Create(cinema, cancellationToken);
            return result;
        }
    }
}
