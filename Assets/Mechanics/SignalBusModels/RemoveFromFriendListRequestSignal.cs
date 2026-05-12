using System;

namespace Mechanics.SignalBusModels
{
    public class RemoveFromFriendListRequestSignal
    {
        public Guid UserId;
        public string UserName;
        public int AvatarId;
        public bool IsRequest;
    }
}