using MediatR;

namespace SC.SenseTower.Admin.Cqrs.SetPassword
{
    public class SetPasswordCommand : IRequest<Unit>
    {
        public Guid? UserId { get; set; }

        public string? CurrentPassword { get; set; }

        public string? Password { get; set; }
    }
}
