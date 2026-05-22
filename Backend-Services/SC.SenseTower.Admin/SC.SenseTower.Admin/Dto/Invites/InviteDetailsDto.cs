namespace SC.SenseTower.Admin.Dto.Invites
{
    public class InviteDetailsDto
    {
        public string Id { get; set; } = null!;

        public Guid? IssuerId { get; set; }

        public string? IssuerName { get; set; }

        public Guid? AuthorId { get; set; }

        public string? AuthorName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UsingDate { get; set; }

        public Guid? UserId { get; set; }

        public string? UserName { get; set; }

        public DateTime? RecallDate { get; set; }

        public string? RecallReason { get; set; }
    }
}
