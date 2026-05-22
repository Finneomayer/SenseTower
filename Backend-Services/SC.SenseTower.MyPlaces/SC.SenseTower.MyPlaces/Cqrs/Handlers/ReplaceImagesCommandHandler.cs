using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.RabbitMQ;
using SC.SenseTower.MyPlaces.Services;
using System.Linq;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class ReplaceImagesCommandHandler : BaseHandler, IRequestHandler<ReplaceImagesCommand, Unit>
    {

        private readonly PlacesService placesService;
        private readonly ImagesService imagesService;
        private readonly AccountsService accountsService;
        private readonly RabbitMQService rabbitMQService;

        public ReplaceImagesCommandHandler(
            ILogger<ReplaceImagesCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            ImagesService imagesService,
            AccountsService accountsService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.imagesService = imagesService;
            this.accountsService = accountsService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(ReplaceImagesCommand request, CancellationToken cancellationToken)
        {
            var imageIds = request.Images.Select(x => x.ImageId).Distinct().ToArray();
            var images = await imagesService.GetByIds(request.AccessToken, imageIds, cancellationToken);
            var pictures = request.Images
                .Select(x =>
                {
                    var image = images?.FirstOrDefault(i => i.Id == x.ImageId);
                    var item = new Picture
                    {
                        Location = x.Location,
                        Image = new ImageInfo
                        {
                            Id = x.ImageId,
                            FileUrl = image?.FileUrl ?? string.Empty,
                            Name = image?.Name ?? string.Empty,
                            PreviewUrl = image?.PreviewUrl ?? string.Empty
                        }
                    };
                    return item;
                })
                .ToArray();
            await placesService.ReplaceImages(request.PlaceId, pictures, cancellationToken);

            var place = await placesService.Get(request.PlaceId, cancellationToken);
            var placeDto = Mapper.Map<PlaceDto>(place);
            if (placeDto.DoorImage.Id != default)
            {
                var doorImages = await imagesService.GetByIds(request.AccessToken, new Guid[] { placeDto.DoorImage.Id }, cancellationToken);
                var imageInfo = doorImages?.FirstOrDefault();
                if (imageInfo != null)
                {
                    placeDto.DoorImage.Name = imageInfo.Name;
                    placeDto.DoorImage.PreviewUrl = imageInfo.PreviewUrl;
                    placeDto.DoorImage.FileUrl = imageInfo.FileUrl;
                }
            }
            if (place.OwnerId != null)
            {
                var owner = await accountsService.UsersLookup(request.AccessToken, new Guid[] { place.OwnerId.Value }, cancellationToken);
                placeDto.OwnerName = owner?.FirstOrDefault()?.Name;
            }
            await rabbitMQService.SendUpdatePlaceMessage(placeDto, cancellationToken);

            return Unit.Value;
        }
    }
}
