using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.RabbitMQ;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class UpdateUserPlaceCommandHandler : BaseHandler, IRequestHandler<UpdateUserPlaceCommand, Unit>
    {
        private readonly PlacesService placesService;
        private readonly RabbitMQService rabbitMQService;
        private readonly ImagesService imagesService;

        public UpdateUserPlaceCommandHandler(
            ILogger<UpdateUserPlaceCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            RabbitMQService rabbitMQService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.rabbitMQService = rabbitMQService;
            this.imagesService = imagesService;
        }

        public async Task<Unit> Handle(UpdateUserPlaceCommand request, CancellationToken cancellationToken)
        {
            await placesService.UpdateAccessType(request.AccessToken, request.Id, request.PublicAccessType, cancellationToken);
            var place = await placesService.Get(request.Id, cancellationToken);
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
