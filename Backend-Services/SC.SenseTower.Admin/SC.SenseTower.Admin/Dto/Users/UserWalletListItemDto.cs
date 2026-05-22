namespace SC.SenseTower.Admin.Dto.Users
{
    public class UserWalletListItemDto
    {
        public string Id { get; set; } = null!;

        public string? Name { get; set; }

        public bool IsConfirmed { get; set; } = false;

        public bool IsActive { get; set; } = false;
    }
}
