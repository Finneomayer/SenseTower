using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Data.Models;
using SC.SenseTower.TowerEvents.RabbitMQ;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventCreate
{
    public class TowerEventCreateCommandHandler : BaseHandler, IRequestHandler<TowerEventCreateCommand, Guid>
    {
        private readonly TowerEventsService towerEventsService;
        private readonly SpacesService spacesService;
        private readonly ImagesService imagesService;
        private readonly RabbitMQService rabbitMQService;

        public TowerEventCreateCommandHandler(
            ILogger<TowerEventCreateCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService,
            SpacesService spacesService,
            ImagesService imagesService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
            this.spacesService = spacesService;
            this.imagesService = imagesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Guid> Handle(TowerEventCreateCommand request, CancellationToken cancellationToken)
        {
            var towerEvent = Mapper.Map<Data.Models.TowerEvent>(request);
            if (request.SpaceId != null)
            {
                try
                {
                    var spaceDto = await spacesService.Get(request.AccessToken, request.SpaceId.Value, cancellationToken);
                    if (spaceDto != null)
                    {
                        var space = Mapper.Map<Space>(spaceDto);
                        towerEvent.Space = space;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }
            }
            if (request.ImageId != null && request.ImageId != default)
            {
                try
                {
                    var imageInfoDto = await imagesService.Get(request.AccessToken, request.ImageId.Value, cancellationToken);
                    if (imageInfoDto != null)
                    {
                        var imageInfo = Mapper.Map<ImageInfo>(imageInfoDto);
                        towerEvent.Image = imageInfo;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }
            }
            var result = await towerEventsService.Create(towerEvent, cancellationToken);
            if (result != default)
            {
                towerEvent.Id = result;
                await rabbitMQService.SendCreateTicketsMessage(towerEvent, request.TicketQuantity, cancellationToken);
            }

            return result;
        }
    }
}
