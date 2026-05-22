using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Spaces.Constants;
using SC.SenseTower.Spaces.Data.Models;

namespace SC.SenseTower.Spaces.Data
{
    public class SpacesDbContext : BaseDbContext
    {
        private IMongoCollection<Space>? spaces;
        public IMongoCollection<Space> Spaces
        {
            get
            {
                if (spaces == null)
                    spaces = GetDbCollection<Space>(DbConstants.COLLECTION_SPACES);
                return spaces;
            }
        }

        public SpacesDbContext(IOptions<MongoDbConfig> options) : base(options) { }
    }
}
