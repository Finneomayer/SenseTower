using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Requests;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class PlaceRequestHandler : BaseHandler, IRequestHandler<PlaceRequest, PlaceDto?>
    {
        private readonly PlacesService placesService;
        private readonly AccountsService accountsService;
        private readonly ImagesService imagesService;

        public PlaceRequestHandler(
            ILogger<PlaceRequestHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            AccountsService accountsService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.accountsService = accountsService;
            this.imagesService = imagesService;
        }

        public async Task<PlaceDto?> Handle(PlaceRequest request, CancellationToken cancellationToken)
        {
            var result = await placesService.GetFullPlaceById(request.PlaceId, cancellationToken);
            if (result == null)
                return null;

            var imageIds = result.Images
                .Where(x => string.IsNullOrEmpty(x.Value.Name) && x.Value.Id != default)
                .Select(x => x.Value.Id)
                .Distinct()
                .ToList();
            if (result.DoorImage.Id != default && !imageIds.Contains(result.DoorImage.Id))
            {
                imageIds.Add(result.DoorImage.Id);
            }
            var images = await imagesService.GetByIds(request.AccessToken, imageIds.ToArray(), cancellationToken);

            if (images != null && images.Any())
            {
                if (result.DoorImage.Id != default)
                {
                    var image = images.FirstOrDefault();
                    if (image != null)
                    {
                        result.DoorImage.Name = image.Name;
                        result.DoorImage.FileUrl = image.FileUrl;
                        result.DoorImage.PreviewUrl = image.PreviewUrl;
                    }
                }

                if (result.Images != null && result.Images.Any())
                {
                    foreach (var item in result.Images.Where(x => string.IsNullOrEmpty(x.Value.Name) && x.Value.Id != default).ToArray())
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

            if (result.OwnerId != null)
            {
                var users = await accountsService.UsersLookup(request.AccessToken, new[] { result.OwnerId.Value } , cancellationToken);
                if (users != null && users.Any())
                {
                    result.OwnerName = users.First().Name;
                }
            }

            return result;
        }
    }
}
