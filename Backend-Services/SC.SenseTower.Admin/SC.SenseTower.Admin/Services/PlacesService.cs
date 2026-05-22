using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SC.SenseTower.Admin.Cqrs.PlacesPage;
using SC.SenseTower.Admin.Dto.Places;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
{
    public class PlacesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public PlacesService(
            ILogger<PlacesService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>> GetUserPlaces(string accessToken, string refreshToken, Guid ownerId, CancellationToken cancellationToken)
        {
            var result = await Get<LookupItemDto<Guid>[]>(accessToken, string.Format(endpointsSettings.PlacesLookupByOwnerUrl, ownerId), null, cancellationToken);
            return result ?? Array.Empty<LookupItemDto<Guid>>();
        }

        public async Task<PagedDataDto<PlacesPageItemResponseDto>> GetPage(string accessToken, string refreshToken, PlacesPageFilter filters, QuerySorting[] sorting, int page, int pageSize, CancellationToken cancellationToken)
        {
            var result = await PostAsJson<PagedDataDto<PlacesPageItemResponseDto>>(accessToken, endpointsSettings.PlacesPageUrl, new { filters, sorting, page, pageSize }, cancellationToken);
            return result ?? new PagedDataDto<PlacesPageItemResponseDto>();
        }

        public async Task<PlaceResponseDto?> GetPlace(string accessToken, string refreshToken, Guid id, CancellationToken cancellationToken)
        {
            var result = await Get<PlaceResponseDto>(accessToken, string.Format(endpointsSettings.PlacesGetOneUrl, id), null, cancellationToken);
            return result;
        }

        public async Task Update(string accessToken, string refreshToken, PlaceSaveDto placeDto, CancellationToken cancellationToken)
        {
            _ = await Post<object>(accessToken, endpointsSettings.PlacesUpdateUrl, placeDto, cancellationToken);
        }

        public async Task<Guid?> Create(string accessToken, string refreshToken, PlaceSaveDto placeDto, CancellationToken cancellationToken)
        {
            var result = await Post<Guid>(accessToken, endpointsSettings.PlacesCreateUrl, placeDto, cancellationToken);
            return result;
        }

        public async Task Delete(string accessToken, string refreshToken, Guid placeId, CancellationToken cancellationToken)
        {
            _ = await Post<object>(accessToken, string.Format(endpointsSettings.PlacesDeleteUrl, placeId), null, cancellationToken);
        }

        public async Task UpdatePlaceImages(string accessToken, string refreshToken, Guid placeId, PlaceImageSaveDto[] images, CancellationToken cancellationToken)
        {
            _ = await Post<object>(accessToken, endpointsSettings.PlacesReplaceImagesUrl, new { placeId, images }, cancellationToken);
        }

        public async Task<Guid[]> AllocatedSpaceIds(string accessToken, string refreshToken, CancellationToken cancellationToken)
        {
            var result = await Get<Guid[]>(accessToken, endpointsSettings.PlacesAllocatedSpaceIdsUrl, null, cancellationToken);
            return result ?? Array.Empty<Guid>();
        }
    }
}
