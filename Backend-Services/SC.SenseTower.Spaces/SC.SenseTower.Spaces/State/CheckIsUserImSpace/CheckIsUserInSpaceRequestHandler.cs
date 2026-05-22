using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.State.CheckIsUserImSpace
{
    public class CheckIsUserInSpaceRequestHandler : BaseHandler, IRequestHandler<CheckIsUserInSpaceRequest, bool>
    {
        private readonly IUserLocationService localDataService;

        public CheckIsUserInSpaceRequestHandler(
            ILogger<CheckIsUserInSpaceRequestHandler> logger,
            IMapper mapper,
            IUserLocationService localDataService) : base(logger, mapper)
        {
            this.localDataService = localDataService;
        }

        public async Task<bool> Handle(CheckIsUserInSpaceRequest request, CancellationToken cancellationToken)
        {
            var result = localDataService.IsUserInSpace(request.SpaceId, request.UserId);
            return await Task.FromResult(result);
        }
    }
}
