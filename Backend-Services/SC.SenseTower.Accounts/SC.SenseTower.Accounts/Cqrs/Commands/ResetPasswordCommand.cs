using MediatR;
using SC.SenseTower.Accounts.Dto.Identity;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class ResetPasswordCommand : IRequest<OperationResultDto>
    {
        public Guid? UserId { get; set; }

        public string? Token { get; set; }

        public string? Password { get; set; }
    }
}
