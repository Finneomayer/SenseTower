namespace SC.SenseTower.Admin.Dto.Users
{
    public class UserGridItemDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool EmailConfirmed { get; set; }

        public string? PhoneNumber { get; set; }

        public string RoleName { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? AccessGrantedTo { get; set; }
    }
}
