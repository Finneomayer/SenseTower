using Cysharp.Threading.Tasks;

namespace Broadcasting
{
    public interface IBroadcastingService
    {
        public UniTask<bool> CheckCapability();

        public void StartBroadcasting();

        public void BroadcastAuth();
    }
}
