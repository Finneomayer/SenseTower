using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Models;
using SC.SenseTower.MyPlaces.Cqrs.Requests;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class AdminPlacesPageRequestHandler : BaseHandler, IRequestHandler<AdminPlacesPageRequest, PagedDataDto<PlaceDto>>
    {
        private readonly PlacesService placesService;
        private readonly ImagesService imagesService;
        private readonly AccountsService accountsService;

        public AdminPlacesPageRequestHandler(
            ILogger<AdminPlacesPageRequestHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            ImagesService imagesService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.imagesService = imagesService;
            this.accountsService = accountsService;
        }

        public async Task<PagedDataDto<PlaceDto>> Handle(AdminPlacesPageRequest request, CancellationToken cancellationToken)
        {
            var data = await placesService.Get(request.Filters, request.Sorting, request.Offset, request.PageSize, cancellationToken);
            var items = Mapper.Map<PlaceDto[]>(data);

            var imageIds = data
                .Where(x => x.DoorImageId != null)
                .Select(x => x.DoorImageId.Value)
                .Distinct()
                .ToArray();
            if (imageIds.Any())
            {
                var images = await imagesService.GetByIds(request.AccessToken, imageIds, cancellationToken);
                if (images?.Any() ?? false)
                {
                    items.ForEach(x =>
                    {
                        x.DoorImage = images.FirstOrDefault(p => p.Id == x.DoorImage.Id) ?? x.DoorImage;
                    });
                }
            }

            var userIds = data
                .Where(x => x.OwnerId != null)
                .Select(x => x.OwnerId.Value)
                .Distinct()
                .ToArray();
            if (userIds.Any())
            {
                var users = await accountsService.UsersLookup(request.AccessToken, userIds, cancellationToken);
                if (users?.Any() ?? false)
                {
                    items.ForEach(x =>
                    {
                        x.OwnerName = users.FirstOrDefault(u => u.Id == x.OwnerId)?.Name;
                    });
                }
            }

            var result = new PagedDataDto<PlaceDto>
            {
                Items = items,
                TotalCount = await placesService.Count(request.Filters, cancellationToken)
            };
            return result;
        }
    }
}
