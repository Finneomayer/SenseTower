using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Dto.Users
{
    public class UserDetailsDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool EmailConfirmed { get; set; }

        public Guid? RoleId { get; set; }
        public LookupItemDto<Guid>[] AvailableRoles { get; set; } = Array.Empty<LookupItemDto<Guid>>();

        public DateTime? CreatedAt { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public UserInviteListItemDto[] Invites { get; set; } = Array.Empty<UserInviteListItemDto>();

        public UserWalletListItemDto[] Wallets { get; set; } = Array.Empty<UserWalletListItemDto>();

        public IEnumerable<LookupItemDto<Guid>> Places { get; set; } = Array.Empty<LookupItemDto<Guid>>();
    }
}
