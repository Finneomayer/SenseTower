using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.TowerEvents.Constants;
using SC.SenseTower.TowerEvents.Data.Models;

namespace SC.SenseTower.TowerEvents.Data
{
    public class TowerEventsDbContext : BaseDbContext
    {
        private IMongoCollection<TowerEvent>? towerEvents;
        public IMongoCollection<TowerEvent> TowerEvents
        {
            get
            {
                towerEvents ??= GetDbCollection<TowerEvent>(DbConstants.COLLECTION_TOWER_EVENTS);
                return towerEvents;
            }
        }

        public TowerEventsDbContext(IOptions<MongoDbConfig> options) : base(options) { }
    }
}
