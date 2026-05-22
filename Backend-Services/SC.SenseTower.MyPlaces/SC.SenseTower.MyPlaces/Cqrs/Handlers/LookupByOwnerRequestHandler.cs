using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;
using SC.SenseTower.MyPlaces.Cqrs.Requests;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class LookupByOwnerRequestHandler : BaseHandler, IRequestHandler<LookupByOwnerRequest, IEnumerable<LookupItemDto<Guid>>>
    {
        private readonly PlacesService placesService;

        public LookupByOwnerRequestHandler(
            ILogger<LookupByOwnerRequest> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>> Handle(LookupByOwnerRequest request, CancellationToken cancellationToken)
        {
            var data = await placesService.GetByOwnerId(request.OwnerId, cancellationToken);
            var result = Mapper.Map<LookupItemDto<Guid>[]>(data.OrderBy(x => x.PlaceName));
            return result ?? Array.Empty<LookupItemDto<Guid>>();
        }
    }
}
