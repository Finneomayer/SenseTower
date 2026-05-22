using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Galleries.Constants;
using SC.SenseTower.Galleries.Data.Models;

namespace SC.SenseTower.Galleries.Data
{
    public class GalleriesDbContext : BaseDbContext
    {
        private IMongoCollection<Gallery>? galleries;
        public IMongoCollection<Gallery> Galleries
        {
            get
            {
                galleries ??= GetDbCollection<Gallery>(DbConstants.COLLECTION_GALLERIES);
                return galleries;
            }
        }

        public GalleriesDbContext(IOptions<MongoDbConfig> options) : base(options) { }
    }
}
