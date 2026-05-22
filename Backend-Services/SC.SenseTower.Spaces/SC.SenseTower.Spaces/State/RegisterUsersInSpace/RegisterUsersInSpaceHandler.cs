using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.State.RegisterUsersInSpace
{
    public class RegisterUsersInSpaceHandler : BaseHandler, IRequestHandler<RegisterUsersInSpaceCommand, Unit>
    {
        private readonly IUserLocationService localDataService;

        public RegisterUsersInSpaceHandler(
            ILogger<RegisterUsersInSpaceHandler> logger,
            IMapper mapper,
            IUserLocationService localDataService) : base(logger, mapper)
        {
            this.localDataService = localDataService;
        }

        public async Task<Unit> Handle(RegisterUsersInSpaceCommand request, CancellationToken cancellationToken)
        {
            localDataService.RegisterUsersInSpace(request.SpaceId, request.UserIds);
            return await Task.FromResult(Unit.Value);
        }
    }
}
