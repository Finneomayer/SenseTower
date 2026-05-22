using MediatR;

namespace SC.SenseTower.Admin.Cqrs.UserUnban
{
    public class UserUnbanCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}
