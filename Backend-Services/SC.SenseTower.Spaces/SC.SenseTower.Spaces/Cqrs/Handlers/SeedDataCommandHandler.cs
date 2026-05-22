using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class SeedDataCommandHandler : BaseHandler, IRequestHandler<SeedDataCommand, Unit>
    {
        private readonly ISpacesService spacesService;
        private readonly AccountsService accountsService;
        private readonly PlacesService placesService;
        private readonly ImagesService imagesService;
        private readonly IRabbitMQService rabbitMQService;

        public SeedDataCommandHandler(
            ILogger<SeedDataCommandHandler> logger,
            IMapper mapper,
            ISpacesService spacesService,
            AccountsService accountsService,
            PlacesService placesService,
            ImagesService imagesService,
            IRabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
            this.accountsService = accountsService;
            this.placesService = placesService;
            this.imagesService = imagesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(SeedDataCommand request, CancellationToken cancellationToken)
        {
            var spaces = await spacesService.GetAll(cancellationToken);
            foreach (var space in spaces)
            {
                space.IsPrivate = space.SpaceType == SpaceType.MySpace || space.SpaceType == SpaceType.MyGallery || space.SpaceType == SpaceType.InfrastructureScene;

                var place = await placesService.GetBySpaceId(request.AccessToken, space.Id, cancellationToken);
                if (place != null)
                {
                    Mapper.Map(place, space);

                    if (place.OwnerId != null)
                    {
                        try
                        {
                            var userInfo = await accountsService.GetInfo(request.AccessToken, place.OwnerId.Value, cancellationToken);
                            space.SpaceOwner = Mapper.Map<UserInfo>(userInfo);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Ошибка при получении информации о пользователе {0}", place.OwnerId.Value);
                        }
                    }

                    if (place.DoorImage.Id != default)
                    {
                        try
                        {
                            var imageInfo = await imagesService.GetInfo(request.AccessToken, place.DoorImage.Id, cancellationToken);
                            space.DoorImage = Mapper.Map<ImageInfo>(imageInfo);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Ошибка при получении информации об изображении {0}", place.DoorImage.Id);
                        }
                    }
                }

                await spacesService.Update(space, cancellationToken);
                await rabbitMQService.SendUpdateSpaceMessage(space, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
