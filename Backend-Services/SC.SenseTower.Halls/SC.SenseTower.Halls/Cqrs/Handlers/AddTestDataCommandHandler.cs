using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Constants;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class AddTestDataCommandHandler : BaseHandler, IRequestHandler<AddTestDataCommand, Unit>
    {
        private readonly HallsService hallsService;

        public AddTestDataCommandHandler(
            ILogger<AddTestDataCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<Unit> Handle(AddTestDataCommand request, CancellationToken cancellationToken)
        {
            foreach (var hall in FakeData.Halls)
            {
                var entity = await hallsService.Get(hall.Id, cancellationToken);
                //if (entity == null)
                //    await hallsService.Create(hall, cancellationToken);
                //else
                //    await hallsService.Update(hall, cancellationToken);
                if (entity != null)
                {
                    entity.PublicPlaces = hall.PublicPlaces;
                    await hallsService.Update(hall, cancellationToken);
                }
            }
            return Unit.Value;
        }
    }
}
