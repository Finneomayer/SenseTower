using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.MyPlaces.Cqrs.Requests;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class UserPlacesRequestHandler : BaseHandler, IRequestHandler<UserPlacesRequest, PlaceDto[]?>
    {
        private readonly PlacesService placesService;
        private readonly ImagesService imagesService;

        public UserPlacesRequestHandler(
            ILogger<UserPlacesRequestHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.imagesService = imagesService;
        }

        public async Task<PlaceDto[]?> Handle(UserPlacesRequest request, CancellationToken cancellationToken)
        {
            var data = await placesService.GetUserPlaces(request.UserId, cancellationToken);
            var result = Mapper.Map<PlaceDto[]>(data.OrderBy(x => x.PlaceName));

            var imageIds = result
                .Where(x => x.Images != null)
                .SelectMany(x => x.Images)
                .Where(x => x.Value.Id != default)
                .Select(x => x.Value.Id)
                .Distinct()
                .ToList();
            imageIds.AddRange(result
                .Where(x => x.DoorImage.Id != default && !imageIds.Contains(x.DoorImage.Id))
                .Select(x => x.DoorImage.Id));
            if (!imageIds.Any())
            {
                return result;
            }

            var images = await imagesService.GetByIds(request.AccessToken, imageIds.ToArray(), cancellationToken);
            if (images != null && images.Any())
            {
                result.ForEach(x =>
                {
                    var image = images.FirstOrDefault(i => i.Id == x.DoorImage.Id);
                    x.DoorImage.FileUrl = image?.FileUrl ?? string.Empty;
                    x.DoorImage.Name = image?.Name ?? string.Empty;
                    x.DoorImage.PreviewUrl = image?.PreviewUrl ?? string.Empty;
                    x.Images?.ForEach(d =>
                    {
                        image = images.FirstOrDefault(i => i.Id == d.Value.Id);
                        d.Value.FileUrl = image?.FileUrl ?? string.Empty;
                        d.Value.Name = image?.Name ?? string.Empty;
                        d.Value.PreviewUrl = image?.PreviewUrl ?? string.Empty;
                    });
                });
            }

            return result;
        }
    }
}
