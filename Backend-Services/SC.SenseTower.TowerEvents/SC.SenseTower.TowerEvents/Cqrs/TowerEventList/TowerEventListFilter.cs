using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventList
{
    public class TowerEventListFilter : AbstractFilter<Data.Models.TowerEvent>
    {
        /// <summary>
        /// Массив идентификаторов пространств, к которм привязаны кинотеатры.
        /// </summary>
        public Guid[]? Spaces { get; set; }

        /// <summary>
        /// Дата и время начала события.
        /// </summary>
        public DateTimeOffset? From { get; set; }

        /// <summary>
        /// Дата и время окончания события.
        /// </summary>
        public DateTimeOffset? UpTo { get; set; }

        /// <summary>
        /// Число секунд после окончания события, когда билет на него ещё действителен.
        /// </summary>
        public int? UpToPlusSecondsNow { get; set; }

        /// <summary>
        /// Число секунд до начала события, когда билет на него становится действительным.
        /// </summary>
        public int? FromMinusSecondsNow { get; set; }

        /// <summary>
        /// Массив состояний события.
        /// </summary>
        public TowerEventState[]? States { get; set; }

        public async override Task<FilterDefinition<Data.Models.TowerEvent>> Filter(BaseDbService? service, CancellationToken cancellationToken)
        {
            var filter = Builders<Data.Models.TowerEvent>.Filter.Empty;
            if (From != null || FromMinusSecondsNow != null)
            {
                var from = (From ?? DateTimeOffset.UtcNow).AddSeconds(-FromMinusSecondsNow ?? 0);
                filter = Builders<Data.Models.TowerEvent>.Filter.And(filter, Builders<Data.Models.TowerEvent>.Filter.Where(x => x.UpTo > from));
            }
            if (UpTo != null || UpToPlusSecondsNow != null)
            {
                var upTo = (UpTo ?? From ?? DateTimeOffset.UtcNow).AddSeconds(UpToPlusSecondsNow ?? 0);
                filter = Builders<Data.Models.TowerEvent>.Filter.And(filter, Builders<Data.Models.TowerEvent>.Filter.Where(x => x.Date < upTo));
            }
            if (Spaces != null)
                filter = Builders<Data.Models.TowerEvent>.Filter.And(filter, Builders<Data.Models.TowerEvent>.Filter.Where(x => x.Space != null && Spaces.Contains(x.Space.Id)));
            if (States != null)
                filter = Builders<Data.Models.TowerEvent>.Filter.And(filter, Builders<Data.Models.TowerEvent>.Filter.Where(x => States.Contains(x.State)));
            return await Task.FromResult(filter);
        }
    }
}
