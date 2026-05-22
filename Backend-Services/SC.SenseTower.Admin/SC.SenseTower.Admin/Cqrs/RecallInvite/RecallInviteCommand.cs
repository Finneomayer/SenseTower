using MediatR;

namespace SC.SenseTower.Admin.Cqrs.RecallInvite
{
    public class RecallInviteCommand : IRequest<Unit>
    {
        public string Id { get; set; } = null!;

        public string? Reason { get; set; }
    }
}
