using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.API
{
    public interface IUsersInSpacesService
    {
        UniTask<UsersInSpaceResponse> GetUsersInSpaces(int? count);
    }
}