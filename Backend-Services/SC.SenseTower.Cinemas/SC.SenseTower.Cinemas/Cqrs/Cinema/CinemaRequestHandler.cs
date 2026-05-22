using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Dto.Cinemas;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.Cinema
{
    public class CinemaRequestHandler : BaseHandler, IRequestHandler<CinemaRequest, CinemaDto>
    {
        private readonly CinemasService cinemasService;

        public CinemaRequestHandler(
            ILogger<CinemaRequestHandler> logger,
            IMapper mapper,
            CinemasService cinemasService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
        }

        public async Task<CinemaDto> Handle(CinemaRequest request, CancellationToken cancellationToken)
        {
            var data = await cinemasService.Get(request.CinemaId, cancellationToken);
            var result = Mapper.Map<CinemaDto>(data);
            return result;
        }
    }
}
