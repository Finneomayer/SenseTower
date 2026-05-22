using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Images.Dto.Images;
using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.Cqrs.UpdateImage
{
    public class UpdateImageCommandHandler : BaseHandler, IRequestHandler<UpdateImageCommand, Unit>
    {
        private readonly ImageFilesService imageFilesService;
        private readonly PlacesService placesService;

        public UpdateImageCommandHandler(
            ILogger<UpdateImageCommandHandler> logger,
            IMapper mapper,
            ImageFilesService imageFilesService,
            PlacesService placesService) : base(logger, mapper)
        {
            this.imageFilesService = imageFilesService;
            this.placesService = placesService;
        }

        public async Task<Unit> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
        {
            var image = await imageFilesService.Update(request.Id, request.Name, request.AccessToken, cancellationToken);
            return Unit.Value;
        }
    }
}
