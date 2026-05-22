using MediatR;

namespace SC.SenseTower.Admin.Cqrs.ResetPassword
{
    public class ResetPasswordCommand : IRequest<Unit>
    {
        public Guid? UserId { get; set; }

        public string? Token { get; set; }

        public string? Password { get; set; }
    }
}
