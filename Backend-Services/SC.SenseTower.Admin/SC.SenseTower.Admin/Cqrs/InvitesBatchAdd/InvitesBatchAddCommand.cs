using MediatR;

namespace SC.SenseTower.Admin.Cqrs.InvitesBatchAdd
{
    public class InvitesBatchAddCommand : IRequest<string[]>
    {
        public Guid AuthorId { get; set; } = default;

        public Guid UserId { get; set; } = default;

        public int Quantity { get; set; }
    }
}
