using Cysharp.Threading.Tasks;
using System;

namespace Assets.Scripts.Event
{
    public interface ITowerEventService
    {
        /// <summary>
        /// Получаем будущие и текущие события в башне отсортированные по времени (ближайшие первые)
        /// </summary>
        /// <param name="getCount"></param>
        /// <returns></returns>
        UniTask<TowerEvent[]> GetEvents(int? getCount);
        UniTask<TowerEvent[]> GetEvents(TowerEventsFilter filter);
        UniTask<TowerEvent> GetEvent(Guid eventId);
    }
}