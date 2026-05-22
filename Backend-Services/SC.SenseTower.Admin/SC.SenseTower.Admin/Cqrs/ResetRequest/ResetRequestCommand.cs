using MediatR;

namespace SC.SenseTower.Admin.Cqrs.ResetRequest
{
    public class ResetRequestCommand : IRequest<Unit>
    {
        public string? LoginOrEmail { get; set; }

        public string CallbackUrl { get; set; } = null!;
    }
}
