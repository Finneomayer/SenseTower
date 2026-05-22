using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Common.Data
{
    public class BaseDbContext : MongoDbContext
    {
        public BaseDbContext(IOptions<MongoDbConfig> options) : base(new MongoClient(options.Value.ConnectionString), options.Value.DatabaseName)
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.CSharpLegacy;
        }

        public IMongoCollection<T> GetDbCollection<T>(string collectionName) => Database.GetCollection<T>(collectionName);
    }
}
