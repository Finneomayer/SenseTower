using AutoMapper;
using Flurl;
using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Dto.Images;
using SC.SenseTower.MyPlaces.Settings;

namespace SC.SenseTower.MyPlaces.Services
{
    public class ImagesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings settings;

        public ImagesService(
            ILogger<ImagesService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            settings = options.Value;
        }

        public async Task<IEnumerable<ImageInfoDto>?> GetByIds(string? accessToken, Guid[] imageIds, CancellationToken cancellationToken)
        {
            var result = await Post<ImageInfoDto[]>(accessToken, settings.ImagesGetByIds, new { ids = imageIds }, cancellationToken);
            return result;
        }
    }
}
