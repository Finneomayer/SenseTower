using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.CinemaDelete
{
    public class CinemaDeleteCommandHandler : BaseHandler, IRequestHandler<CinemaDeleteCommand, Unit>
    {
        private readonly CinemasService cinemasService;

        public CinemaDeleteCommandHandler(
            ILogger<CinemaDeleteCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
        }

        public async Task<Unit> Handle(CinemaDeleteCommand request, CancellationToken cancellationToken)
        {
            await cinemasService.Delete(request.Id, cancellationToken);
            return Unit.Value;
        }
    }
}
