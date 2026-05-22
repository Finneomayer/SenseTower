using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.State.RegisterUserInSpace
{
    public class RegisterUserInSpaceCommandHandler : BaseHandler, IRequestHandler<RegisterUserInSpaceCommand, Unit>
    {
        private readonly UserInSpaceService userInSpaceService;

        public RegisterUserInSpaceCommandHandler(
            ILogger<RegisterUserInSpaceCommandHandler> logger,
            IMapper mapper,
            UserInSpaceService userInSpaceService) : base(logger, mapper)
        {
            this.userInSpaceService = userInSpaceService;
        }

        public async Task<Unit> Handle(RegisterUserInSpaceCommand request, CancellationToken cancellationToken)
        {
            userInSpaceService.RegisterUserInSpace(request.SpaceId, request.UserId);
            return await Task.FromResult(Unit.Value);
        }
    }
}
