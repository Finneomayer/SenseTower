using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class GenerateInvitesCommand : IRequest<string[]>
    {
        public Guid? UserId { get; set; }

        public string? Role { get; set; }

        public int Quantity { get; set; }
    }
}
