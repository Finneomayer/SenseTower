using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Dto.Cinemas;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaBySpace
{
    public class CinemaBySpaceRequestHandler : BaseHandler, IRequestHandler<CinemaBySpaceRequest, CinemaDto?>
    {
        private readonly CinemasService cinemasService;

        public CinemaBySpaceRequestHandler(
            ILogger<CinemaBySpaceRequestHandler> logger,
            IMapper mapper,
            CinemasService cinemasService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
        }

        public async Task<CinemaDto?> Handle(CinemaBySpaceRequest request, CancellationToken cancellationToken)
        {
            var data = await cinemasService.GetBySpaceId(request.SpaceId, cancellationToken);
            var result = Mapper.Map<CinemaDto>(data);
            return result;
        }
    }
}
