using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.State.GetUsersInSpace
{
    public class GetUsersInSpaceRequestHandler : BaseHandler, IRequestHandler<GetUsersInSpaceRequest, Guid[]>
    {
        private readonly UserInSpaceService userInSpaceService;

        public GetUsersInSpaceRequestHandler(
            ILogger<GetUsersInSpaceRequestHandler> logger,
            IMapper mapper,
            UserInSpaceService userInSpaceService) : base(logger, mapper)
        {
            this.userInSpaceService = userInSpaceService;
        }

        public async Task<Guid[]> Handle(GetUsersInSpaceRequest request, CancellationToken cancellationToken)
        {
            var result = userInSpaceService.GetUsersInSpace(request.SpaceId);
            return await Task.FromResult(result);
        }
    }
}
