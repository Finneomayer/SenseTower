using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Data.Models;
using SC.SenseTower.TowerEvents.RabbitMQ;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventUpdate
{
    public class TowerEventUpdateCommandHandler : BaseHandler, IRequestHandler<TowerEventUpdateCommand, Unit>
    {
        private readonly TowerEventsService towerEventsService;
        private readonly SpacesService spacesService;
        private readonly ImagesService imagesService;
        private readonly RabbitMQService rabbitMQService;

        public TowerEventUpdateCommandHandler(
            ILogger<TowerEventUpdateCommandHandler> logger,
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

        public async Task<Unit> Handle(TowerEventUpdateCommand request, CancellationToken cancellationToken)
        {
            var towerEvent = await towerEventsService.Get(request.Id, cancellationToken);
            Mapper.Map(request, towerEvent);

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
            else
            {
                towerEvent.Space = null;
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
            else
            {
                towerEvent.Image = null;
            }

            await towerEventsService.Update(towerEvent, cancellationToken);
            await rabbitMQService.SendTowerEventUpdateMessage(towerEvent, cancellationToken);
            return Unit.Value;
        }
    }
}
