using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.RabbitMQ.ImagesDelete
{
    public class ImagesDeleteCommandHandler : BaseHandler, IRequestHandler<ImagesDeleteCommand, Unit>
    {
        private readonly ImageStorageService imageStorageService;
        private readonly ImageFilesService imageFilesService;


        public ImagesDeleteCommandHandler(
            ILogger<ImagesDeleteCommandHandler> logger,
            IMapper mapper,
            ImageStorageService imageStorageService,
            ImageFilesService imageFilesService) : base(logger, mapper)
        {
            this.imageStorageService = imageStorageService;
            this.imageFilesService = imageFilesService;
        }

        public async Task<Unit> Handle(ImagesDeleteCommand request, CancellationToken cancellationToken)
        {
            foreach (var id in request.ImageIds)
            {
                try
                {
                    var imageFile = await imageFilesService.Get(id, cancellationToken);
                    var ext = Path.GetExtension(imageFile?.FileName);
                    await imageFilesService.Delete(id, cancellationToken);
                    await imageStorageService.Delete($"{id}_full{ext}", cancellationToken);
                    await imageStorageService.Delete($"{id}_pre{ext}", cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Ошибка удаления изображения {id}: {ex.Message}");
                }
            }
            return Unit.Value;
        }
    }
}
