using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Requests;
using SC.SenseTower.Spaces.Dto.Spaces;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class SpacesByOwnerRequestHandler : BaseHandler, IRequestHandler<SpacesByOwnerRequest, IEnumerable<SpaceDto>>
    {
        private readonly ISpacesService spacesService;

        public SpacesByOwnerRequestHandler(
            ILogger<SpacesByOwnerRequestHandler> logger,
            IMapper mapper,
            ISpacesService spacesService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
        }

        public async Task<IEnumerable<SpaceDto>> Handle(SpacesByOwnerRequest request, CancellationToken cancellationToken)
        {
            var data = await spacesService.GetByOwnerId(request.UserId, cancellationToken);
            var result = Mapper.Map<SpaceDto[]>(data);
            return result ?? Array.Empty<SpaceDto>();
        }
    }
}
