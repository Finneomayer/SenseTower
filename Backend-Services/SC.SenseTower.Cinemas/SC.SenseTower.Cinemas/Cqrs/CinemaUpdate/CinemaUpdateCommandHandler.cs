using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaUpdate
{
    public class CinemaUpdateCommandHandler : BaseHandler, IRequestHandler<CinemaUpdateCommand, Unit>
    {
        private readonly CinemasService cinemasService;
        private readonly SpacesService spacesService;

        public CinemaUpdateCommandHandler(
            ILogger<CinemaUpdateCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService,
            SpacesService spacesService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
            this.spacesService = spacesService;
        }

        public async Task<Unit> Handle(CinemaUpdateCommand request, CancellationToken cancellationToken)
        {
            var spaceDto = await spacesService.GetSpace(request.AccessToken, request.SpaceId, cancellationToken);
            var space = Mapper.Map<Space>(spaceDto);
            await cinemasService.Update(request.Id, request.Name, space, cancellationToken);
            return Unit.Value;
        }
    }
}
