using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Identity;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.UserStatistics
{
    public class UserStatisticsQueryHandler : BaseHandler, IRequestHandler<UserStatisticsQuery, UserStatisticsDto>
    {
        private readonly IdentityService identityService;

        public UserStatisticsQueryHandler(
            ILogger<UserStatisticsQueryHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<UserStatisticsDto> Handle(UserStatisticsQuery request, CancellationToken cancellationToken)
        {
            return await identityService.GetUserStatistics(cancellationToken);
        }
    }
}
