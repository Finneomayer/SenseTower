using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class AddTestDataCommandHandler : BaseHandler, IRequestHandler<AddTestDataCommand, Unit>
    {
        private readonly ISpaceService spaceService;
        private readonly ISpacesService spacesService;

        public AddTestDataCommandHandler(
            ILogger<AddTestDataCommandHandler> logger,
            IMapper mapper,
            ISpaceService spaceService,
            ISpacesService spacesService) : base(logger, mapper)
        {
            this.spaceService = spaceService;
            this.spacesService = spacesService;
        }

        public async Task<Unit> Handle(AddTestDataCommand request, CancellationToken cancellationToken)
        {
            var spaces = await spaceService.GetAllSpaces();
            foreach (var space in spaces)
            {
                var entity = Mapper.Map<Space>(space);
                if (await spacesService.Get(space.Id, cancellationToken) == null)
                {
                    await spacesService.Create(entity, cancellationToken);
                }
                else
                {
                    await spacesService.Update(entity, cancellationToken);
                }
            }
            return Unit.Value;
        }
    }
}
