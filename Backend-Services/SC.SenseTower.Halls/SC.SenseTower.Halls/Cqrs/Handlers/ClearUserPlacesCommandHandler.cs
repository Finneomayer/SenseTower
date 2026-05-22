using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class ClearUserPlacesCommandHandler : BaseHandler, IRequestHandler<ClearUserPlacesCommand, bool>
    {
        private readonly HallsService hallsService;

        public ClearUserPlacesCommandHandler(
            ILogger<ClearUserPlacesCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<bool> Handle(ClearUserPlacesCommand request, CancellationToken cancellationToken)
        {
            await hallsService.ClearUserPlaces(request.PlaceIds, cancellationToken);
            return true;
        }
    }
}
