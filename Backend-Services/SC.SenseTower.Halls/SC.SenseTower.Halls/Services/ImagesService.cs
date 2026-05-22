using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Settings;

namespace SC.SenseTower.Halls.Services
{
    public class ImagesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public ImagesService(
            ILogger<BaseHttpService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<ImageInfo?> GetInfo(string accessToken, Guid imageId, CancellationToken cancellationToken)
        {
            var result = (await Post<ImageInfo[]?>(accessToken, endpointsSettings.ImagesByIdsUrl, new[] { imageId }, cancellationToken))?.FirstOrDefault();
            return result;
        }
    }
}
