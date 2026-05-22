using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.TowerEvents.Dto.Images;
using SC.SenseTower.TowerEvents.Settings;

namespace SC.SenseTower.TowerEvents.Services
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

        public async Task<ImageInfoResponseDto?> Get(string accessToken, Guid imageId, CancellationToken cancellationToken)
        {
            var result = (await Post<ImageInfoResponseDto[]>(accessToken, endpointsSettings.GetImageInfoByIdUrl, new { ids = new Guid[] { imageId } }, cancellationToken))?.FirstOrDefault();
            return result;
        }
    }
}
