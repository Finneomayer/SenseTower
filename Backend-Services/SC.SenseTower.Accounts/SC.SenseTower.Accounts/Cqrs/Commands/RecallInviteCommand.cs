using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class RecallInviteCommand : IRequest<bool>
    {
        public string? InviteId { get; set; }

        public string? Reason { get; set; }

        public Guid? UserId { get; set; }

        public string? Role { get; set; }
    }
}
