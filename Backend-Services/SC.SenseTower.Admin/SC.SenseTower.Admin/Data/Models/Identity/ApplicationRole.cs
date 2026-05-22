using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Admin.Constants;

namespace SC.SenseTower.Admin.Data.Models.Identity
{
    [CollectionName(DbConstants.COLLECTION_ROLES)]
    public class ApplicationRole : MongoIdentityRole<Guid>
    {

    }
}