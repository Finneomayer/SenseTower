using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class UpdateSpaceCommandHandler : BaseHandler, IRequestHandler<UpdateSpaceCommand, Unit>
    {
        private readonly ISpacesService spacesService;
        private readonly AccountsService accountsService;
        private readonly ImagesService imagesService;
        private readonly IRabbitMQService rabbitMQService;

        public UpdateSpaceCommandHandler(
            ILogger<UpdateSpaceCommandHandler> logger,
            IMapper mapper,
            ISpacesService spacesService,
            AccountsService accountsService,
            ImagesService imagesService,
            IRabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
            this.accountsService = accountsService;
            this.imagesService = imagesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(UpdateSpaceCommand request, CancellationToken cancellationToken)
        {
            var space = await spacesService.Get(request.Id, cancellationToken);
            Mapper.Map(request, space);

            if (request.SpaceOwnerId != null)
            {
                var userInfo = await accountsService.GetInfo(request.AccessToken, request.SpaceOwnerId.Value, cancellationToken);
                space.SpaceOwner = Mapper.Map<UserInfo>(userInfo);
            }
            else
                space.SpaceOwner = null;

            if (request.DoorImageId != null)
            {
                var imageInfo = await imagesService.GetInfo(request.AccessToken, request.DoorImageId.Value, cancellationToken);
                space.DoorImage = Mapper.Map<ImageInfo>(imageInfo);
            }
            else
                space.DoorImage = null;

            await spacesService.Update(space, cancellationToken);
            await rabbitMQService.SendUpdateSpaceMessage(space, cancellationToken);

            return Unit.Value;
        }
    }
}
