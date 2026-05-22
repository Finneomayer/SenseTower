using MediatR;
using SC.SenseTower.Admin.Dto.Identity;

namespace SC.SenseTower.Admin.Cqrs.UserStatistics
{
    public class UserStatisticsQuery : IRequest<UserStatisticsDto>
    {
    }
}
