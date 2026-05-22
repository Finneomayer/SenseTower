using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Requests;
using SC.SenseTower.Spaces.Dto.Spaces;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class AllSpacesRequestHandler : BaseHandler, IRequestHandler<AllSpacesRequest, IEnumerable<SpaceItemDto>?>
    {
        private readonly ISpacesService spacesService;

        public AllSpacesRequestHandler(
            ILogger<AllSpacesRequestHandler> logger,
            IMapper mapper,
            ISpacesService spacesService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
        }

        public async Task<IEnumerable<SpaceItemDto>?> Handle(AllSpacesRequest request, CancellationToken cancellationToken)
        {
            var data = await spacesService.GetAll(cancellationToken);
            var result = Mapper.Map<SpaceItemDto[]>(data);
            return result;
        }
    }
}
