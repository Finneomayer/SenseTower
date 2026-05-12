using Cysharp.Threading.Tasks;

namespace Assets.Mechanics.MetaAvatars.Scripts
{
    public interface IOculusAuthService
    {
        UniTask<ulong> LogInUser();
        UniTask<bool> IsCdnAvailable();
        UniTask<bool> IsUserHasAvatar(ulong userId);

    }
}