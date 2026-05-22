using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Requests;
using SC.SenseTower.Spaces.Dto.Spaces;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class SpaceRequestHandler : BaseHandler, IRequestHandler<SpaceRequest, SpaceDto>
    {
        private readonly ISpacesService spacesService;

        public SpaceRequestHandler(
            ILogger<SpaceRequestHandler> logger,
            IMapper mapper,
            ISpacesService spacesService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
        }

        public async Task<SpaceDto> Handle(SpaceRequest request, CancellationToken cancellationToken)
        {
            var data = await spacesService.Get(request.SpaceId, cancellationToken);
            var result = Mapper.Map<SpaceDto>(data);
            return result;
        }
    }
}
