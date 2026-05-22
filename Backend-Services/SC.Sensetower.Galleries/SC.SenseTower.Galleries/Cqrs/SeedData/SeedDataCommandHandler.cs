using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.SeedData
{
    public class SeedDataCommandHandler : BaseHandler, IRequestHandler<SeedDataCommand, Unit>
    {
        private readonly GalleriesService galleriesService;

        public SeedDataCommandHandler(
            ILogger<SeedDataCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Unit> Handle(SeedDataCommand request, CancellationToken cancellationToken)
        {
            await galleriesService.SeedData(cancellationToken);
            return Unit.Value;
        }
    }
}
