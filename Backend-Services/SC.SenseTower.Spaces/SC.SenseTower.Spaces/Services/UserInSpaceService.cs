using AutoMapper;
using SC.SenseTower.Common.Services;
using System.Collections.Concurrent;

namespace SC.SenseTower.Spaces.Services
{
    public class UserInSpaceService : BaseService
    {
        private readonly ConcurrentDictionary<Guid, Guid> usersInSpaces = new();

        public UserInSpaceService(ILogger<UserInSpaceService> logger, IMapper mapper) : base(logger, mapper)
        {
        }

        public void RegisterUserInSpace(Guid spaceId, Guid userId)
        {
            Logger.LogDebug($"Регистрация пользователя {userId} в пространстве {spaceId}");

            Guid currSpaceId;
            var cnt = 0;
            if (usersInSpaces.ContainsKey(userId))
            {
                while (!usersInSpaces.TryGetValue(userId, out currSpaceId) && cnt < 5)
                {
                    cnt++;
                }
                if (cnt == 5)
                {
                    Logger.LogError($"Не получено значение временной метки нахождения пользователя {userId} в пространстве {spaceId}");
                    return;
                }
                cnt = 0;
                while (!usersInSpaces.TryUpdate(userId, spaceId, currSpaceId) && cnt < 5)
                {
                    cnt++;
                }
                if (cnt == 5)
                {
                    Logger.LogError($"Не обновлена информация о нахождении пользователя {userId} в пространстве {spaceId}");
                }
            }
            else
            {
                while (!usersInSpaces.TryAdd(userId, spaceId) && cnt < 5)
                {
                    cnt++;
                }
                if (cnt == 5)
                {
                    Logger.LogError($"Не добавлена информация о нахождении пользователя {userId} в пространстве {spaceId}");
                }
            }
        }

        public Guid[] GetUsersInSpace(Guid spaceId)
        {
            var result = usersInSpaces
                .Where(x => x.Value == spaceId)
                .Select(x => x.Key)
                .ToArray();
            return result;
        }
    }
}
