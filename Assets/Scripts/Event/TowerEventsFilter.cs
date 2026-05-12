using System;

namespace Assets.Scripts.Event
{
    public class TowerEventsFilter
    {
        public Guid[] Spaces;
        public int? UpToPlusSecondsNow;
        public int? FromMinusSecondsNow;
        public TowerEventState[] States;
    }
}