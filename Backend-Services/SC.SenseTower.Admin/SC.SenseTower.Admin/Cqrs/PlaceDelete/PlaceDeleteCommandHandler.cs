using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.PlaceDelete
{
    public class PlaceDeleteCommandHandler : BaseHandler, IRequestHandler<PlaceDeleteCommand, Unit>
    {
        private readonly PlacesService placesService;

        public PlaceDeleteCommandHandler(
            ILogger<PlaceDeleteCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<Unit> Handle(PlaceDeleteCommand request, CancellationToken cancellationToken)
        {
            await placesService.Delete(request.AccessToken, request.RefreshToken, request.Id, cancellationToken);
            return Unit.Value;
        }
    }
}
