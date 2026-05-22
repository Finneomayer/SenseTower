using SC.SenseTower.Admin.Dto.Identity;

namespace SC.SenseTower.Admin.Models.Home
{
    public class HomeViewModel
    {
        public UserStatisticsDto UserStatistics { get; set; } = new();
    }
}
