using MediatR;

namespace SC.SenseTower.Admin.Cqrs.UserBan
{
    public class UserBanCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }

        public DateTime LockoutEnd { get; set; }

        public bool IsPermanent { get; set; }
    }
}
