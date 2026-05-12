using Assets.Mechanics.FriendsList.Models;
using Cysharp.Threading.Tasks;

namespace Assets.Mechanics.FriendsList
{
    public interface IFriendsService
    {
        public UniTask<GetFriendDTO[]> GetFriendsList(string userId);
        public UniTask<bool> DeleteFriend(string userId, string friendId);
        public UniTask<bool> MakeTwoUsersFriendServer(string firstUserToken, string secondUserToken);
    }
}