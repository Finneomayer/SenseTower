using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository;
using SC.SenseTower.Auth.Constants;
using SC.SenseTower.Auth.Models;
using SC.SenseTower.Auth.Settings;

namespace SC.SenseTower.Auth.Data
{
    public class AuthDbContext : MongoDbContext
    {
        private IMongoCollection<ApplicationUser> users;
        public IMongoCollection<ApplicationUser> Users
        {
            get
            {
                if (users == null)
                    users = Database.GetCollection<ApplicationUser>(DbConstants.COLLECTION_USERS);
                return users;
            }
        }

        private IMongoCollection<ApplicationRole> roles;
        public IMongoCollection<ApplicationRole> Roles
        {
            get
            {
                if (roles == null)
                    roles = Database.GetCollection<ApplicationRole>(DbConstants.COLLECTION_ROLES);
                return roles;
            }
        }

        public AuthDbContext(IOptions<MongoDbConfig> options) : base(new MongoClient(options.Value.ConnectionString), options.Value.DatabaseName)
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.CSharpLegacy;
        }
    }
}
