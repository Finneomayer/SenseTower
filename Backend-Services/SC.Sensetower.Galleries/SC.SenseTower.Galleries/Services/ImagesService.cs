using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Galleries.Dto.Images;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Galleries.Settings;

namespace SC.SenseTower.Galleries.Services
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

        public async Task<IEnumerable<ImageInfoDto>> GetByIds(string accessToken, Guid[] imagesIds, CancellationToken cancellationToken)
        {
            var result = await Post<ImageInfoDto[]>(accessToken, endpointsSettings.ImagesByIdsUrl, new { ids = imagesIds }, cancellationToken);
            return result ?? Array.Empty<ImageInfoDto>();
        }

        public async Task<Guid> Create(string accessToken, Guid userId, IFormFile imageFile, CancellationToken cancellationToken)
        {
            var result = await Post<Guid>(accessToken, endpointsSettings.AddImageUrl, new { userId, file = imageFile }, cancellationToken);
            return result;
        }

        public async Task<ImageInfoDto?> CopyImage(string accessToken, Guid imageId, CancellationToken cancellationToken)
        {
            var result = await Post<ImageInfoDto>(accessToken, string.Format(endpointsSettings.ImagesCopyOneUrl, imageId), null, cancellationToken);
            return result;
        }
    }
}
