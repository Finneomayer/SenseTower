using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Auth.Constants;

namespace SC.SenseTower.Auth.Models
{
    [CollectionName(DbConstants.COLLECTION_ROLES)]
    public class ApplicationRole : MongoIdentityRole<Guid>
    {

    }
}