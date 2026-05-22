using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Data;

namespace SC.SenseTower.Admin.Data.Contexts
{
    public class IdentityDbContext : BaseDbContext
    {
        private IMongoCollection<ApplicationRole>? roles;
        public IMongoCollection<ApplicationRole> Roles => roles ??= GetDbCollection<ApplicationRole>(DbConstants.COLLECTION_ROLES);

        private IMongoCollection<ApplicationUser>? users;
        public IMongoCollection<ApplicationUser> Users => users ??= GetDbCollection<ApplicationUser>(DbConstants.COLLECTION_USERS);

        public IdentityDbContext(IOptions<IdentityDbConfig> options) : base(options) { }
    }
}
