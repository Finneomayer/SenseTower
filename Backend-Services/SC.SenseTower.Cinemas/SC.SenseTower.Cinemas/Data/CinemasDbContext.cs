using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Cinemas.Constants;

namespace SC.SenseTower.Cinemas.Data
{
    public class CinemasDbContext : BaseDbContext
    {
        private IMongoCollection<Cinema>? cinemas;
        public IMongoCollection<Cinema> Cinemas
        {
            get
            {
                cinemas ??= GetDbCollection<Cinema>(DbConstants.COLLECTION_CINEMAS);
                return cinemas;
            }
        }

        public CinemasDbContext(IOptions<MongoDbConfig> options) : base(options) { }
    }
}
