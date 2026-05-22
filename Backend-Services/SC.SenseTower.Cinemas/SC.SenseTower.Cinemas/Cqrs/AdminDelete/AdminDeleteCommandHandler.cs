using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.AdminDelete
{
    public class AdminDeleteCommandHandler : BaseHandler, IRequestHandler<AdminDeleteCommand, Unit>
    {
        private readonly CinemasService cinemasService;

        public AdminDeleteCommandHandler(
            ILogger<AdminDeleteCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
        }

        public async Task<Unit> Handle(AdminDeleteCommand request, CancellationToken cancellationToken)
        {
            await cinemasService.DeleteAdministrator(request.CinemaId, request.UserId, cancellationToken);
            return Unit.Value;
        }
    }
}
