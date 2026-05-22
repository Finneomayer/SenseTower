using MongoDB.Driver;
using SC.SenseTower.Auth.Data;
using SC.SenseTower.Auth.Models;

namespace SC.SenseTower.Auth.Services
{
    public class UsersService
    {
        private readonly AuthDbContext context;

        public UsersService(AuthDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<UserInfo>> GetByIds(Guid[] userIds, CancellationToken cancellationToken = default)
        {
            var usersFilter = Builders<ApplicationUser>.Filter.Where(x => userIds.Contains(x.Id));
            var users = await context.Users.Find(usersFilter).ToListAsync(cancellationToken);

            var roleIds = users
                .SelectMany(x => x.Roles)
                .Distinct()
                .ToArray();
            var rolesFilter = Builders<ApplicationRole>.Filter.Where(x => roleIds.Contains(x.Id));
            var roles = await context.Roles.Find(rolesFilter).ToListAsync(cancellationToken);
            var result = users
                .Select(x => new UserInfo
                {
                    AccessGrantedTo = x.AccessGrantedTo,
                    Email = x.Email,
                    Login = x.UserName,
                    Role = roles.FirstOrDefault(r => r.Id == x.Roles.FirstOrDefault())?.Name,
                    UserId = x.Id
                })
                .ToArray();
            return result;
        }
    }
}
