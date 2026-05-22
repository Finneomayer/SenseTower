using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Images.Data.Models;
using SC.SenseTower.Images.Dto.Images;
using SC.SenseTower.Images.Services;
using SC.SenseTower.Images.Settings;

namespace SC.SenseTower.Images.Cqrs.UserImages
{
    public class UserImagesRequestHandler : BaseHandler, IRequestHandler<UserImagesRequest, IEnumerable<ImageListItemDto>>
    {
        private readonly ImageFilesService imageFilesService;
        //private readonly AccountsService accountsService;
        private readonly ImageSettings settings;

        public UserImagesRequestHandler(
            ILogger<UserImagesRequestHandler> logger,
            IMapper mapper,
            ImageFilesService imageFilesService,
            //AccountsService accountsService,
            IOptions<ImageSettings> options) : base(logger, mapper)
        {
            this.imageFilesService = imageFilesService;
            //this.accountsService = accountsService;
            settings = options.Value;
        }

        public async Task<IEnumerable<ImageListItemDto>> Handle(UserImagesRequest request, CancellationToken cancellationToken)
        {
            var imageFiles = await imageFilesService.GetByUser(request.OwnerId, request.Role.Equals("vr_admin", StringComparison.InvariantCultureIgnoreCase), request.UserId, cancellationToken);
            var result = Mapper.Map<IEnumerable<ImageFile>, ImageListItemDto[]>(imageFiles, o =>
            {
                o.AfterMap((s, d) =>
                {
                    foreach (var item in d)
                    {
                        item.FileUrl = settings.RootUrl + item.FileUrl;
                        item.PreviewUrl = settings.RootUrl + item.PreviewUrl;
                    }
                });
            });

            return result;
        }
    }
}
