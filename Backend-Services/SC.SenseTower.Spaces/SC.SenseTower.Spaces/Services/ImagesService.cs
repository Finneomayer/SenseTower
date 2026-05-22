using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Spaces.Dto.Spaces;
using SC.SenseTower.Spaces.Settings;

namespace SC.SenseTower.Spaces.Services
{
    public class ImagesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public ImagesService(
            ILogger<ImagesService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<ImageInfoDto?> GetInfo(string accessToken, Guid imageId, CancellationToken cancellationToken)
        {
            var data = await Post<ImageInfoDto[]>(accessToken, endpointsSettings.ImagesGetByIds, new { ids = new Guid[] { imageId } }, cancellationToken);
            return data != null && data.Length > 0 ? data[0] : null;
        }

        public async Task<ImageInfoDto[]> GetByIds(string? accessToken, Guid[] imageIds, CancellationToken cancellationToken)
        {
            var data = await Post<ImageInfoDto[]>(accessToken, endpointsSettings.ImagesGetByIds, new { ids = imageIds }, cancellationToken);
            return data ?? Array.Empty<ImageInfoDto>();
        }
    }
}
