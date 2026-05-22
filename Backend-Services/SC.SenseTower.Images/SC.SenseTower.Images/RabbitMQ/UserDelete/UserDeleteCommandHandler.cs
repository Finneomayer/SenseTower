using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.RabbitMQ.UserDelete
{
    public class UserDeleteCommandHandler : BaseHandler, IRequestHandler<UserDeleteCommand, Unit>
    {
        private readonly ImageFilesService filesService;
        private readonly ImageStorageService storageService;

        public UserDeleteCommandHandler(
            ILogger<UserDeleteCommandHandler> logger,
            IMapper mapper,
            ImageFilesService filesService,
            ImageStorageService storageService) : base(logger, mapper)
        {
            this.filesService = filesService;
            this.storageService = storageService;
        }

        public async Task<Unit> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var files = await filesService.GetByUser(request.UserId, false, null, cancellationToken);
                foreach (var file in files)
                {
                    await storageService.Delete(file.FileName, cancellationToken);
                }
                await filesService.DeleteByUser(request.UserId, cancellationToken);
            }
            catch (Exception ex)
            {
                //todo: сделать передачу предупреждений на фронт
                Logger.LogError(ex.Message);
            }
            return Unit.Value;
        }
    }
}
