using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Admin.Cqrs;
using SC.SenseTower.Admin.Cqrs.GalleriesPage;
using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
{
    public class GalleriesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public GalleriesService(
            ILogger<BaseHttpService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<PagedDataDto<GalleryItemResponseDto>> GetPagedList(string accessToken, GalleriesPageFilter filter, QuerySorting[] sorting, int page, int pageSize, CancellationToken cancellationToken)
        {
            var request = new PagedDataRequest<GalleriesPageFilter>
            {
                Filters = filter,
                Sorting = sorting,
                Page = page,
                PageSize = pageSize
            };
            var result = await Post<PagedDataDto<GalleryItemResponseDto>>(accessToken, endpointsSettings.GalleryListUrl, request, cancellationToken);
            return result ?? new PagedDataDto<GalleryItemResponseDto>();
        }

        public async Task<bool> Exists(string? accessToken, Guid id, CancellationToken cancellationToken)
        {
            var result = await Get<bool>(accessToken, string.Format(endpointsSettings.GalleryExistsUrl, id), null, cancellationToken);
            return result;
        }

        public async Task<GalleryResponseDto?> GetGallery(string? accessToken, Guid id, CancellationToken cancellationToken)
        {
            var result = await Get<GalleryResponseDto>(accessToken, string.Format(endpointsSettings.GalleryGetUrl, id), null, cancellationToken);
            return result;
        }

        public async Task Update(string accessToken, GalleryUpdateRequestDto gallery, CancellationToken cancellationToken)
        {
            await Post<bool>(accessToken, endpointsSettings.GalleryUpdateUrl, gallery, cancellationToken);
        }

        public async Task<IEnumerable<GalleryImageDto>> GetGalleryImages(string? accessToken, Guid galleryId, CancellationToken cancellationToken)
        {
            var data = await Get<Dictionary<int, GalleryImageResponseDto>>(accessToken, string.Format(endpointsSettings.GalleryImagesUrl, galleryId), null, cancellationToken);
            var result = data?
                .OrderBy(x => x.Key)
                .Select(x => new GalleryImageDto
                {
                    Author = x.Value.Author,
                    Description = x.Value.Description,
                    ImageId = x.Value.Image.Id,
                    ImageName = x.Value.Image.Name,
                    ImageUrl = x.Value.Image.PreviewUrl,
                    Name = x.Value.Name,
                    Position = x.Key,
                    PassepartoutWidthInMeters = x.Value.PassepartoutWidthInMeters,
                    PictureWidthInMeters = x.Value.PictureWidthInMeters
                })
                .ToArray() ?? Array.Empty<GalleryImageDto>();
            return result;
        }

        public async Task AddImage(string accessToken, Guid galleryId, GalleryAddImageDto galleryImage, CancellationToken cancellationToken)
        {
            await Post<bool>(accessToken, string.Format(endpointsSettings.GalleryAddImageUrl, galleryId), galleryImage, cancellationToken);
        }

        public async Task RemoveImage(string accessToken, Guid galleryId, int position, CancellationToken cancellationToken)
        {
            await Post<bool>(accessToken, string.Format(endpointsSettings.GalleryRemoveImageUrl, galleryId, position), null, cancellationToken);
        }

        public async Task UpdateImages(string accessToken, Guid galleryId, GalleryImageDto[] images, CancellationToken cancellationToken)
        {
            await Post<bool>(accessToken, string.Format(endpointsSettings.GalleryReplaceImagesUrl, galleryId), new { images }, cancellationToken);
        }

        public async Task<Guid?> Create(string accessToken, GalleryCreateRequestDto gallery, CancellationToken cancellationToken)
        {
            var result = await Post<Guid>(accessToken, endpointsSettings.GalleryCreateUrl, gallery, cancellationToken);
            return result;
        }

        public async Task Delete(string accessToken, Guid galleryId, CancellationToken cancellationToken)
        {
            await Post<bool>(accessToken, string.Format(endpointsSettings.GalleryDeleteUrl, galleryId), null, cancellationToken);
        }
    }
}
