using Assets.Scripts.Event;
using Assets.Scripts.Space;
using System;

namespace UI
{
    public sealed class TowerEventMediator
    {
        public event Action<Guid> BuyTicketRequested;
        public event Action<TowerEvent> LoadEventSpaceRequested;

        public void RequestBuyTicket(Guid eventGuid)
        {
            BuyTicketRequested?.Invoke(eventGuid);
        }

        public void RequestLoadEventSpace(TowerEvent towerEvent)
        {
            LoadEventSpaceRequested?.Invoke(towerEvent);
        }
    }
}
