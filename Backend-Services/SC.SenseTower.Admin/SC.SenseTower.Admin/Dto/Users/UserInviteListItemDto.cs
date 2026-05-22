namespace SC.SenseTower.Admin.Dto.Users
{
    public class UserInviteListItemDto
    {
        public string Id { get; set; } = null!;

        public string? UserName { get; set; }

        public bool IsRecalled { get; set; }
    }
}
