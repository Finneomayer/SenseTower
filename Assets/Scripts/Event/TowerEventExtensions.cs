using System.Collections.Generic;

namespace Assets.Scripts.Event
{
    public static class TowerEventExtensions
    {
        public static TowerEvent GetLatestEvent(this IEnumerable<TowerEvent> towerEvents)
        {
            if (towerEvents == null)
            {
                return null;
            }

            TowerEvent latestTowerEvent = null;
            foreach (var item in towerEvents)
            {
                if (latestTowerEvent == null || latestTowerEvent.From < item.From)
                {
                    latestTowerEvent = item;
                }
            }
            return latestTowerEvent;
        }
    }
}
