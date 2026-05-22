using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Dto.Cinemas;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaList
{
    public class CinemaListRequestHandler : BaseHandler, IRequestHandler<CinemaListRequest, IEnumerable<CinemaDto>>
    {
        private readonly CinemasService cinemasService;

        public CinemaListRequestHandler(
            ILogger<CinemaListRequest> logger,
            IMapper mapper,
            CinemasService cinemasService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
        }

        public async Task<IEnumerable<CinemaDto>> Handle(CinemaListRequest request, CancellationToken cancellationToken)
        {
            var data = await cinemasService.Get(cancellationToken);
            var result = Mapper.Map<CinemaDto[]>(data);
            return result;
        }
    }
}
