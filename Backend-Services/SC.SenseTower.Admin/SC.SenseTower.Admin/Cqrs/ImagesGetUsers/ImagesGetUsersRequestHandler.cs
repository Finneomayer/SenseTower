using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Images;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.ImagesGetUsers
{
    public class ImagesGetUsersRequestHandler : BaseHandler, IRequestHandler<ImagesGetUsersRequest, IEnumerable<ImageInfoDto>>
    {
        private readonly ImagesService imagesService;

        public ImagesGetUsersRequestHandler(
            ILogger<ImagesGetUsersRequestHandler> logger,
            IMapper mapper,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.imagesService = imagesService;
        }

        public async Task<IEnumerable<ImageInfoDto>> Handle(ImagesGetUsersRequest request, CancellationToken cancellationToken)
        {
            var result = await imagesService.Lookup(request.AccessToken, null, cancellationToken);
            return result;
        }
    }
}
