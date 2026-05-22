namespace SC.SenseTower.Admin.Dto.Invites
{
    public class InviteGridItemDto
    {
        public string Id { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public string? IssuerName { get; set; }

        public string? UserName { get; set; }

        public DateTime? UsedAt { get; set; }

        public bool IsRecalled { get; set; }
    }
}
