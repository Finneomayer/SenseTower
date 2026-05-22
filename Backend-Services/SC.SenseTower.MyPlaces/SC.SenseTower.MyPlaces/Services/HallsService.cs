using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.Settings;
using System.Text.Json;

namespace SC.SenseTower.MyPlaces.Services
{
    public class HallsService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;
        private readonly ImagesService imagesService;
        private readonly AccountsService accountsService;

        public HallsService(
            ILogger<BaseHttpService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options,
            ImagesService imagesService,
            AccountsService accountsService) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
            this.imagesService = imagesService;
            this.accountsService = accountsService;
        }

        public async Task UpdatePlace(string accessToken, Place place, CancellationToken cancellationToken)
        {
            var placeDto = Mapper.Map<PlaceDto>(place);
            var imageIds = placeDto.Images
                .Where(x => string.IsNullOrEmpty(x.Value.Name) && x.Value.Id != default)
                .Select(x => x.Value.Id)
                .Distinct()
                .ToList();
            if (placeDto.DoorImage.Id != default && !imageIds.Contains(placeDto.DoorImage.Id))
            {
                imageIds.Add(placeDto.DoorImage.Id);
            }
            var images = await imagesService.GetByIds(accessToken, imageIds.ToArray(), cancellationToken);

            if (images != null && images.Any())
            {
                if (placeDto.DoorImage.Id != default)
                {
                    var image = images.FirstOrDefault();
                    if (image != null)
                    {
                        placeDto.DoorImage.Name = image.Name;
                        placeDto.DoorImage.FileUrl = image.FileUrl;
                        placeDto.DoorImage.PreviewUrl = image.PreviewUrl;
                    }
                }

                if (placeDto.Images != null && placeDto.Images.Any())
                {
                    foreach (var item in placeDto.Images.Where(x => string.IsNullOrEmpty(x.Value.Name) && x.Value.Id != default).ToArray())
                    {
                        var image = images.FirstOrDefault(x => x.Id == item.Value.Id);
                        if (image != null)
                        {
                            item.Value.Name = image.Name;
                            item.Value.FileUrl = image.FileUrl;
                            item.Value.PreviewUrl = image.PreviewUrl;
                        }
                    }
                }
            }

            if (placeDto.OwnerId != null)
            {
                var users = await accountsService.UsersLookup(accessToken, new[] { placeDto.OwnerId.Value }, cancellationToken);
                var user = users?.FirstOrDefault(x => x.Id == placeDto.OwnerId.Value);
                if (user != null)
                {
                    placeDto.OwnerName = user.Name;
                }
            }

            var json = JsonSerializer.Serialize(placeDto);
            await PostAsJson<bool>(accessToken, endpointsSettings.HallsUpdatePlaceUrl, json, cancellationToken);
        }

        public async Task DeletePlace(string accessToken, Guid[] ids, CancellationToken cancellationToken)
        {
            await Post<bool>(accessToken, endpointsSettings.HallsDeletePlaceUrl, new { placeIds = ids }, cancellationToken);
        }
    }
}
