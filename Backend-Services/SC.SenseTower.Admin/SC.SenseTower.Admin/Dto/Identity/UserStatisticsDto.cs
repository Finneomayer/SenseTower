namespace SC.SenseTower.Admin.Dto.Identity
{
    public class UserStatisticsDto
    {
        public long TotalCount { get; set; } = 0;

        public long LastMonthCount { get; set; } = 0;
        public long PrevMonthCount { get; set; } = 0;

        public long LastWeekCount { get; set; } = 0;
        public long PrevWeekCount { get; set; } = 0;

        public long TodayCount { get; set; } = 0;
        public long YesterdayCount { get; set; }
    }
}
