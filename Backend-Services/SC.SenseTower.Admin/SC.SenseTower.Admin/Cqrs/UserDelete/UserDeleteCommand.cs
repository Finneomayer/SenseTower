using MediatR;

namespace SC.SenseTower.Admin.Cqrs.UserDelete
{
    public class UserDeleteCommand : IRequest<Unit>
    {
        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public Guid UserId { get; set; }
    }
}
