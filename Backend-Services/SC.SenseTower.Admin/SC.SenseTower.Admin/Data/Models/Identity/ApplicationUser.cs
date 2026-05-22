using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Admin.Constants;

namespace SC.SenseTower.Admin.Data.Models.Identity
{
    [CollectionName(DbConstants.COLLECTION_USERS)]
    [BsonIgnoreExtraElements]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public DateTime? AccessGrantedTo { get; set; }
    }
}
