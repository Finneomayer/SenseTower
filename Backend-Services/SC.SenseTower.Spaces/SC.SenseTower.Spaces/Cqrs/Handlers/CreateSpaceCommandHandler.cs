using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class CreateSpaceCommandHandler : BaseHandler, IRequestHandler<CreateSpaceCommand, Guid>
    {
        private readonly ISpacesService spacesService;
        private readonly AccountsService accountsService;
        private readonly ImagesService imagesService;

        public CreateSpaceCommandHandler(
            ILogger<CreateSpaceCommandHandler> logger,
            IMapper mapper,
            ISpacesService spacesService,
            AccountsService accountsService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
            this.accountsService = accountsService;
            this.imagesService = imagesService;
        }

        public async Task<Guid> Handle(CreateSpaceCommand request, CancellationToken cancellationToken)
        {
            var space = Mapper.Map<Space>(request);

            if (request.SpaceOwnerId != null)
            {
                var userInfo = await accountsService.GetInfo(request.AccessToken, request.SpaceOwnerId.Value, cancellationToken);
                space.SpaceOwner = Mapper.Map<UserInfo>(userInfo);
            }

            if (request.DoorImageId != null)
            {
                var imageInfo = await imagesService.GetInfo(request.AccessToken, request.DoorImageId.Value, cancellationToken);
                space.DoorImage = Mapper.Map<ImageInfo>(imageInfo);
            }

            var result = await spacesService.Create(space, cancellationToken);
            return result;
        }
    }
}
