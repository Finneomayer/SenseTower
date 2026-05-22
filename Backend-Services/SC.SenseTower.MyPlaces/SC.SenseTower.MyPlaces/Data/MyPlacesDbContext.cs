using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.MyPlaces.Constants;
using SC.SenseTower.MyPlaces.Data.Models;

namespace SC.SenseTower.MyPlaces.Data
{
    public class MyPlacesDbContext : BaseDbContext
    {
        private IMongoCollection<Counter>? counters;
        public IMongoCollection<Counter> Counters
        {
            get
            {
                counters ??= GetDbCollection<Counter>(DbConstants.COLLECTION_COUNTERS);
                return counters;
            }
        }

        private IMongoCollection<Place>? places;
        public IMongoCollection<Place> Places
        {
            get
            {
                places ??= GetDbCollection<Place>(DbConstants.COLLECTION_PLACES);
                return places;
            }
        }

        public MyPlacesDbContext(IOptions<MongoDbConfig> options) : base(options) { }
    }
}
