using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Auth.Constants;

namespace SC.SenseTower.Auth.Models
{
    [CollectionName(DbConstants.COLLECTION_USERS)]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public DateTime? AccessGrantedTo { get; set; }
    }
}
