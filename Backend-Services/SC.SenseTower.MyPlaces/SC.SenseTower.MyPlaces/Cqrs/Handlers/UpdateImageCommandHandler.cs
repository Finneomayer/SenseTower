using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.RabbitMQ;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class UpdateImageCommandHandler : BaseHandler, IRequestHandler<UpdateImageCommand, Unit>
    {
        private readonly PlacesService placesService;
        private readonly RabbitMQService rabbitMQService;
        private readonly ImagesService imagesService;

        public UpdateImageCommandHandler(
            ILogger<UpdateImageCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            RabbitMQService rabbitMQService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.rabbitMQService = rabbitMQService;
            this.imagesService = imagesService;
        }

        public async Task<Unit> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
        {
            await placesService.UpdateDoorImage(request.AccessToken, request.PlaceId, request.DoorImageId, cancellationToken);

            var place = await placesService.Get(request.PlaceId, cancellationToken);
            var placeDto = Mapper.Map<PlaceDto>(place);
            if (placeDto.DoorImage.Id != default)
            {
                var images = await imagesService.GetByIds(request.AccessToken, new Guid[] { placeDto.DoorImage.Id }, cancellationToken);
                var imageInfo = images?.FirstOrDefault();
                if (imageInfo != null)
                {
                    placeDto.DoorImage.Name = imageInfo.Name;
                    placeDto.DoorImage.PreviewUrl = imageInfo.PreviewUrl;
                    placeDto.DoorImage.FileUrl = imageInfo.FileUrl;
                }
            }

            await rabbitMQService.SendUpdatePlaceMessage(placeDto, cancellationToken);
            return Unit.Value;
        }
    }
}
