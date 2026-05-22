using MongoDB.Driver;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Cqrs.UsersPage
{
    public class UsersPageFilter : AbstractFilter<ApplicationUser>
    {
        public string? UserName { get; set; }

        public string? Email { get; set; }

        public Guid? RoleId { get; set; }
        public IEnumerable<LookupItemDto<Guid>> AvailableRoles { get; set; } = Array.Empty<LookupItemDto<Guid>>();

        public string? StatusName { get; set; }

        public override async Task<FilterDefinition<ApplicationUser>> Filter(BaseDbService? service, CancellationToken cancellationToken)
        {
            var filter = StatusName switch
            {
                "IsLocked" => Builders<ApplicationUser>.Filter.Where(r => r.LockoutEnd > DateTime.UtcNow),
                "IsNotLocked" => Builders<ApplicationUser>.Filter.Where(r => r.LockoutEnd == null || r.LockoutEnd <= DateTime.UtcNow),
                _ => Builders<ApplicationUser>.Filter.Empty
            };
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                filter = Builders<ApplicationUser>.Filter.And(filter, Builders<ApplicationUser>.Filter.Where(r => r.NormalizedUserName.Contains(UserName.Trim().ToUpperInvariant())));
            }
            if (!string.IsNullOrWhiteSpace(Email))
            {
                filter = Builders<ApplicationUser>.Filter.And(filter, Builders<ApplicationUser>.Filter.Where(r => r.NormalizedEmail.Contains(Email.Trim().ToUpperInvariant())));
            }
            if (RoleId != null)
            {
                filter = Builders<ApplicationUser>.Filter.And(filter, Builders<ApplicationUser>.Filter.Where(r => r.Roles.Contains(RoleId.Value)));
            }
            return await Task.FromResult(filter);
        }
    }
}
