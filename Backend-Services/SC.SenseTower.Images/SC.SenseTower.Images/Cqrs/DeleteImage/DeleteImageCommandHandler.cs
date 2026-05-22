using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.Cqrs.DeleteImage
{
    public class DeleteImageCommandHandler : BaseHandler, IRequestHandler<DeleteImageCommand, Unit>
    {
        private readonly ImageStorageService imageStorageService;
        private readonly ImageFilesService imageFilesService;

        public DeleteImageCommandHandler(
            ILogger<DeleteImageCommandHandler> logger,
            IMapper mapper,
            ImageStorageService imageStorageService,
            ImageFilesService imageFilesService) : base(logger, mapper)
        {
            this.imageStorageService = imageStorageService;
            this.imageFilesService = imageFilesService;
        }

        public async Task<Unit> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            var imageFile = await imageFilesService.Get(request.Id, cancellationToken);
            var ext = Path.GetExtension(imageFile?.FileName);
            await imageFilesService.Delete(request.Id, cancellationToken);
            await imageStorageService.Delete($"{request.Id}_full{ext}", cancellationToken);
            await imageStorageService.Delete($"{request.Id}_pre{ext}", cancellationToken);
            return Unit.Value;
        }
    }
}
