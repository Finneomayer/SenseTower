using Assets.Scripts.Models;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.API
{
    public interface IRegistrationInSpacesService
    {
        public UniTask<bool> Register(string spaceId);
        public UniTask<AccessResultDto> CheckAccess(string spaceId);
        public UniTask<AccessResultDto[]> CheckAccess(string[] spaceIds);
        public UniTask<AccessResultDto[]> CheckUsersAccessServer(string spaceId, CheckingTokenDto[] usersData);
    }
}