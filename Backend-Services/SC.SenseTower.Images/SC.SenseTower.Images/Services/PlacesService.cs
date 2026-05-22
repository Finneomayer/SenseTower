using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Images.Dto.Images;
using SC.SenseTower.Images.Settings;

namespace SC.SenseTower.Images.Services
{
    public class PlacesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings settings;

        public PlacesService(
            ILogger<PlacesService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            settings = options.Value;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>?> GetByIds(string? accessToken, Guid?[] placeIds, CancellationToken cancellationToken)
        {
            var result = await Post<LookupItemDto<Guid>[]>(accessToken, settings.UserPlacesByIds, new { placeIds }, cancellationToken);
            return result;
        }

        public async Task UpdateImagesData(string? accessToken, Guid? placeId, ImageInfoDto image, bool isDeleted, CancellationToken cancellationToken)
        {
            var commands = new UpdateImagesRequest
            {
                PlaceId = placeId,
                UpdateImages = new[]
                {
                    new UpdateImagesDto
                    {
                        ImageInfo = image,
                        IsDeleted = isDeleted
                    }
                }
            };

            await Post<LookupItemDto<UpdateImagesDto>>(accessToken, settings.UpdateImagesData, new { commands }, cancellationToken);
        }
    }
}
