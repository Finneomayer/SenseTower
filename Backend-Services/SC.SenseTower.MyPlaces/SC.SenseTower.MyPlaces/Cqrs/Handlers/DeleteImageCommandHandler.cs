using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.RabbitMQ;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class DeleteImageCommandHandler : BaseHandler, IRequestHandler<DeleteImageCommand, Unit>
    {
        private readonly PlacesService placesService;
        private readonly ImagesService imagesService;
        private readonly AccountsService accountsService;
        private readonly RabbitMQService rabbitMQService;

        public DeleteImageCommandHandler(
            ILogger<DeleteImageCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            RabbitMQService rabbitMQService,
            ImagesService imagesService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.rabbitMQService = rabbitMQService;
            this.imagesService = imagesService;
            this.accountsService = accountsService;
        }

        public async Task<Unit> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            if (await placesService.DeleteImage(request.PlaceId, request.ImageId, cancellationToken))
            {
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
                if (placeDto.OwnerId != null)
                {
                    var owner = await accountsService.UsersLookup(request.AccessToken, new Guid[] { placeDto.OwnerId.Value }, cancellationToken);
                    placeDto.OwnerName = owner?.FirstOrDefault()?.Name;
                }
                await rabbitMQService.SendUpdatePlaceMessage(placeDto, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
