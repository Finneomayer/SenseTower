using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Constants;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class AddTestDataCommandHandler : BaseHandler, IRequestHandler<AddTestDataCommand, Unit>
    {
        private readonly PlacesService placesService;

        public AddTestDataCommandHandler(
            ILogger<AddTestDataCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<Unit> Handle(AddTestDataCommand request, CancellationToken cancellationToken)
        {
            foreach (var place in FakeData.Places)
            {
                if (!await placesService.Exists(place.Id, cancellationToken))
                    await placesService.Create(place, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
