using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.AvailableSpaces
{
    public class AvailableSpacesRequestHandler : BaseHandler, IRequestHandler<AvailableSpacesRequest, IEnumerable<LookupItemDto<Guid>>>
    {
        private readonly SpacesService spacesService;

        public AvailableSpacesRequestHandler(
            ILogger<AvailableSpacesRequestHandler> logger,
            IMapper mapper,
            SpacesService spacesService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>> Handle(AvailableSpacesRequest request, CancellationToken cancellationToken)
        {
            var result = await spacesService.Lookup(request.AccessToken, request.SpaceType, cancellationToken);
            return result;
        }
    }
}
