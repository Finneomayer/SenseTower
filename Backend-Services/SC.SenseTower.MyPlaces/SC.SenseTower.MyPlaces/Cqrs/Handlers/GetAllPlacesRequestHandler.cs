using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.MyPlaces.Cqrs.Requests;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.Services;
using SC.SenseTower.MyPlaces.Settings;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class GetAllPlacesRequestHandler : BaseHandler, IRequestHandler<GetAllPlacesRequest, PlaceDto[]?>
    {
        private readonly PlacesService placesService;
        private readonly AccountsService accountsService;
        private readonly ImagesService imagesService;

        public GetAllPlacesRequestHandler(
            ILogger<GetAllPlacesRequestHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            AccountsService accountsService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.accountsService = accountsService;
            this.imagesService = imagesService;
        }

        public async Task<PlaceDto[]?> Handle(GetAllPlacesRequest request, CancellationToken cancellationToken)
        {
            var data = await placesService.GetAllPlaces(cancellationToken);
            if (data == null)
                throw new ScException("Ошибка чтения списка помещений.");
            var result = Mapper.Map<PlaceDto[]>(data);

            var userIds = result
                .Where(r => r.OwnerId != null)
                .Select(r => r.OwnerId.Value)
                .Distinct()
                .ToArray();
            if (userIds.Length != 0)
            {
                var users = await accountsService.UsersLookup(request.AccessToken, userIds, cancellationToken);
                if (users != null && users.Length > 0)
                {
                    result.Where(r => r.OwnerId != null).ForEach(r =>
                    {
                        var user = users.FirstOrDefault(u => u.Id == r.OwnerId);
                        if (user != null)
                        {
                            r.OwnerName = user.Name;
                        }
                    });
                }
            }

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
