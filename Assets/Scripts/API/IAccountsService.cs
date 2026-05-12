using System;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.API
{
    public interface IAccountsService
    {
        UniTask<UserLookupInfo[]> GetUserLookupInfo(string userId);
        UniTask<UserLookupInfo[]> GetUsersLookupInfo(string[] userIds = null);
        UniTask<int> GetThisUserHours();
        UniTask<bool> GetIsThisUserFullFledged();
        UniTask<bool> GetIsThisUserSeller();
        UniTask<bool> GetHasThisUserInitialBonus();
        UniTask<bool> SetThisUserHasInitialBonus();
        UniTask<DateTimeOffset> GetBonusInitialDate();

        public event Action BecameFullFledged;
    }
}