using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Spaces.Cqrs.Requests;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class LookupRequestHandler : BaseHandler, IRequestHandler<LookupRequest, IEnumerable<LookupItemDto<Guid>>>
    {
        private readonly ISpacesService spacesService;

        public LookupRequestHandler(
            ILogger<LookupRequestHandler> logger,
            IMapper mapper,
            ISpacesService spacesService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>> Handle(LookupRequest request, CancellationToken cancellationToken)
        {
            var result = await spacesService.Lookup(request.SpaceType, cancellationToken);
            return result;
        }
    }
}
