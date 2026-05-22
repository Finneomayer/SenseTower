namespace SC.SenseTower.TowerEvents.Settings
{
    public class HttpConnectionSettings
    {
        public int MaxAttempts { get; set; } = 5;

        public int BreakAfter { get; set; } = 3;

        public int BreakForSeconds { get; set; } = 30;
    }
}
