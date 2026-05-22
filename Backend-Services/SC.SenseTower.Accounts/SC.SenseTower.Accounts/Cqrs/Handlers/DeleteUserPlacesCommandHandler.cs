using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class DeleteUserPlacesCommandHandler : BaseHandler, IRequestHandler<DeleteUserPlacesCommand, bool>
    {
        private readonly PlacesService placesService;

        public DeleteUserPlacesCommandHandler(
            ILogger<DeleteUserPlacesCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<bool> Handle(DeleteUserPlacesCommand request, CancellationToken cancellationToken)
        {
            return await placesService.DeleteUserPlaces(request.AccessToken, request.OwnerId, cancellationToken).ConfigureAwait(false);
        }
    }
}
