using MediatR;
using SC.SenseTower.Accounts.Dto.Identity;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class ConfirmEmailCommand : IRequest<OperationResultDto>
    {
        public Guid UserId { get; set; }

        public string Code { get; set; } = null!;
    }
}
