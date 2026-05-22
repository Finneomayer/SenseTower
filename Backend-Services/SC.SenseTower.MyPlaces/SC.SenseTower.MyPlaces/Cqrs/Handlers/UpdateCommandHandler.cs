using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.RabbitMQ;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class UpdateCommandHandler : BaseHandler, IRequestHandler<UpdateCommand, Unit>
    {
        private readonly PlacesService placesService;
        private readonly SpacesService spacesService;
        private readonly RabbitMQService rabbitMQService;
        private readonly ImagesService imagesService;
        private readonly AccountsService accountsService;

        public UpdateCommandHandler(
            ILogger<UpdateCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            SpacesService spacesService,
            RabbitMQService rabbitMQService,
            ImagesService imagesService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.spacesService = spacesService;
            this.rabbitMQService = rabbitMQService;
            this.imagesService = imagesService;
            this.accountsService = accountsService;
        }

        public async Task<Unit> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            var spaceDto = request.SpaceId == null
                ? null
                : await spacesService.GetSpace(request.AccessToken, request.SpaceId.Value, cancellationToken);
            var space = Mapper.Map<Space>(spaceDto);

            await placesService.Update(
                request.Id,
                request.OwnerId,
                request.PlaceName,
                request.PlaceNumber,
                request.PublicAccessType,
                request.DoorImageId,
                space,
                cancellationToken);

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
            if (request.OwnerId != null)
            {
                var owner = await accountsService.UsersLookup(request.AccessToken, new Guid[] { request.OwnerId.Value }, cancellationToken);
                placeDto.OwnerName = owner?.FirstOrDefault()?.Name;
            }
            await rabbitMQService.SendUpdatePlaceMessage(placeDto, cancellationToken);

            return Unit.Value;
        }
    }
}
