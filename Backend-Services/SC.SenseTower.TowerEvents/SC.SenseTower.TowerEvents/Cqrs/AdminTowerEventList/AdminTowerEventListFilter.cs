using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.AdminTowerEventList
{
    public class AdminTowerEventListFilter : AbstractFilter<Data.Models.TowerEvent>
    {
        public TowerEventState? State { get; set; }

        public string? Title { get; set; }

        public DateTimeOffset? From { get; set; }

        public DateTimeOffset? UpTo { get; set; }

        public Guid? SpaceId { get; set; }

        public async override Task<FilterDefinition<Data.Models.TowerEvent>> Filter(BaseDbService? service, CancellationToken cancellationToken)
        {
            var filter = Builders<Data.Models.TowerEvent>.Filter.Empty;
            if (State != null)
                filter = Builders<Data.Models.TowerEvent>.Filter.And(filter, Builders<Data.Models.TowerEvent>.Filter.Eq(x => x.State, State));
            if (From != null)
                filter = Builders<Data.Models.TowerEvent>.Filter.And(filter, Builders<Data.Models.TowerEvent>.Filter.Where(x => x.UpTo > From));
            if (UpTo != null)
                filter = Builders<Data.Models.TowerEvent>.Filter.And(filter, Builders<Data.Models.TowerEvent>.Filter.Where(x => x.Date < UpTo));
            if (!string.IsNullOrWhiteSpace(Title))
                filter = Builders<Data.Models.TowerEvent>.Filter.And(filter, Builders<Data.Models.TowerEvent>.Filter.Where(x => x.Title.StartsWith(Title.Trim())));
            if (SpaceId != null)
                filter = Builders<Data.Models.TowerEvent>.Filter.And(filter, Builders<Data.Models.TowerEvent>.Filter.Where(x => x.Space != null && x.Space.Id == SpaceId));
            return await Task.FromResult(filter);
        }
    }
}
