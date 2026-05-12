using System;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Event
{
    public interface ITicketService
    {
        public UniTask<bool> BuyTicket(Guid eventId, Guid userId);
    }
}