using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Constants;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.AddTestData
{
    public class AddTestDataCommandHandler : BaseHandler, IRequestHandler<AddTestDataCommand, Unit>
    {
        private readonly CinemasService cinemasService;

        public AddTestDataCommandHandler(
            ILogger<AddTestDataCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
        }

        public async Task<Unit> Handle(AddTestDataCommand request, CancellationToken cancellationToken)
        {
            foreach (var cinema in FakeData.Cinemas)
            {
                await cinemasService.Create(cinema, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
