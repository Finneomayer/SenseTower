using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Admin.Dto.Images;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
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

        public async Task<IEnumerable<ImageInfoDto>> Lookup(string? accessToken, Guid? userId, CancellationToken cancellationToken)
        {
            var result = await Get<ImageInfoDto[]>(accessToken, endpointsSettings.ImagesUserGetUrl, new { userId }, cancellationToken);
            return result ?? Array.Empty<ImageInfoDto>();
        }
    }
}
