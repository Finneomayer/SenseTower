using Unity.Netcode;

namespace Assets.Mechanics.NetworkInteraction.Services
{
    public interface IGrabInteraction
    {
        public void ChangeOwnership(NetworkObject networkObject,ulong newValue);
    }
}