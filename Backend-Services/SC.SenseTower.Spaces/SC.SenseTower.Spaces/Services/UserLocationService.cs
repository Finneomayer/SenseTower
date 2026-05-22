using AutoMapper;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Spaces.Data.Models;
using System.Collections.Concurrent;

namespace SC.SenseTower.Spaces.Services
{
    public class UserLocationService : BaseService, IUserLocationService
    {
        private const int TIME_LIMIT = 10;

        private readonly ConcurrentDictionary<string, DateTime> usersInSpaces = new();

        public UserLocationService(ILogger<UserLocationService> logger, IMapper mapper) : base(logger, mapper)
        {
        }

        public void RegisterUsersInSpace(Guid spaceId, Guid[] userIds)
        {
            Logger.LogInformation($"register users in space: {spaceId} - {userIds.Length}");
            DateTime currTimestamp;
            var timestamp = DateTime.UtcNow;
            foreach (var userId in userIds)
            {
                var key = $"{spaceId}_{userId}";
                var cnt = 0;
                if (usersInSpaces.ContainsKey(key))
                {
                    while (!usersInSpaces.TryGetValue(key, out currTimestamp) && cnt < 5)
                    {
                        cnt++;
                    }
                    if (cnt == 5)
                    {
                        Logger.LogError($"Не получено значение временной метки нахождения пользователя {userId} в пространстве {spaceId}");
                        continue;
                    }
                    cnt = 0;
                    while (!usersInSpaces.TryUpdate(key, timestamp, currTimestamp) && cnt < 5)
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
                    while (!usersInSpaces.TryAdd(key, timestamp) && cnt < 5)
                    {
                        cnt++;
                    }
                    if (cnt == 5)
                    {
                        Logger.LogError($"Не добавлена информация о нахождении пользователя {userId} в пространстве {spaceId}");
                    }
                }
            }
        }

        public bool IsUserInSpace(Guid spaceId, Guid userId)
        {
            var timeLimit = DateTime.UtcNow.AddSeconds(-TIME_LIMIT);
            var key = $"{spaceId}_{userId}";
            if (usersInSpaces.ContainsKey(key))
            {
                var cnt = 0;
                DateTime timestamp;
                while (!usersInSpaces.TryGetValue(key, out timestamp) && cnt < 5)
                {
                    cnt++;
                }
                if (cnt == 5)
                {
                    var message = $"Не получена информация о нахождении пользователя {userId} в пространстве {spaceId}";
                    Logger.LogError(message);
                    throw new ScException(message);
                }

                if (timestamp > timeLimit)
                {
                    return true;
                }

                cnt = 0;
                while (!usersInSpaces.TryRemove(key, out _) && cnt < 5)
                {
                    cnt++;
                }
                if (cnt == 5)
                {
                    Logger.LogError($"Не удалена информация о нахождении пользователя {userId} в пространстве {spaceId}");
                }
                return false;
            }

            return false;
        }

        public List<(Guid userId, Guid spaceId)> GetUsersInSpaces()
        {
            var users = usersInSpaces.Select(d => (d.Key, d.Value)).ToList();

            // clear
            foreach (var user in users)
            {
                var ids = SplitUserKey(user.Key);
                IsUserInSpace(ids.spaceId, ids.userId); //это удаление
            }

            return usersInSpaces
                .Select(d => SplitUserKey(d.Key)).ToList();
        }

        private (Guid userId, Guid spaceId) SplitUserKey(string key)
        {
            var splitted = key.Split('_');
            if (splitted.Length != 2)
            {
                Logger.Log(LogLevel.Error, "wrong record in user in space table " + key);
                return (Guid.Empty, Guid.Empty);
            }

            return (Guid.Parse(splitted[1]), Guid.Parse(splitted[0]));
        }
    }
}
