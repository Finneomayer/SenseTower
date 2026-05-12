using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Space
{
    public interface ISpaceService
    {
        LocalSpace Get(SpaceType type, string id);
        LocalSpace[] GetAllSpaces();
        /// <summary>
        /// Возвращаем всех пользователей во всех помещениях сортированных по алфавиту по нику пользователя
        /// </summary>
        /// <returns></returns>
        UniTask<UsersInSpaceResponse> GetUsersInSpaces(int? getCount);

        public void ReloadSpaces();
    }
}