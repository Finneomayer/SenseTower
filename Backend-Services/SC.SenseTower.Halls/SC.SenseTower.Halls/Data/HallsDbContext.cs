using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Halls.Constants;
using SC.SenseTower.Halls.Data.Models;

namespace SC.SenseTower.Halls.Data
{
    public class HallsDbContext : BaseDbContext
    {
        private IMongoCollection<Hall>? halls;
        public IMongoCollection<Hall> Halls
        {
            get
            {
                if (halls == null)
                    halls = GetDbCollection<Hall>(DbConstants.COLLECTION_HALLS);
                return halls;
            }
        }

        public HallsDbContext(IOptions<MongoDbConfig> options) : base(options) { }
    }
}
