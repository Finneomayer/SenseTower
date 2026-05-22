using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class AddImageCommandHandler : BaseHandler, IRequestHandler<AddImageCommand, Unit>
    {
        private readonly ImagesService imagesService;
        private readonly PlacesService placesService;

        public AddImageCommandHandler(
            ILogger<AddImageCommandHandler> logger,
            IMapper mapper,
            ImagesService imagesService,
            PlacesService placesService) : base(logger, mapper)
        {
            this.imagesService = imagesService;
            this.placesService = placesService;
        }

        public async Task<Unit> Handle(AddImageCommand request, CancellationToken cancellationToken)
        {
            var place = await placesService.Get(request.PlaceId, cancellationToken);
            var imageInfo = (await imagesService.GetByIds(request.AccessToken, new Guid[] { request.ImageId }, cancellationToken))?.First();
            var image = Mapper.Map<ImageInfo>(imageInfo);

            var images = place.Images?.OrderBy(x => x.Location).ToList() ?? new List<Picture>();
            var emptySlot = 0;
            var slotFound = false;
            for (var i = 0; i < images.Count; i++)
            {
                if (i != images[i].Location)
                {
                    emptySlot = i;
                    slotFound = true;
                    break;
                }
            }
            if (!slotFound)
            {
                emptySlot = images.Count;
            }

            var picture = new Picture
            {
                Location = emptySlot,
                Image = image
            };
            images.Add(picture);

            await placesService.ReplaceImages(request.PlaceId, images.OrderBy(x => x.Location).ToArray(), cancellationToken);
            return Unit.Value;
        }
    }
}
