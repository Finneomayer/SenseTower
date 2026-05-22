using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class DeleteHallCommandHandler : BaseHandler, IRequestHandler<DeleteHallCommand, bool>
    {
        private readonly HallsService hallsService;

        public DeleteHallCommandHandler(
            ILogger<DeleteHallCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<bool> Handle(DeleteHallCommand request, CancellationToken cancellationToken)
        {
            return await hallsService.Delete(request.HallId, cancellationToken);
        }
    }
}
