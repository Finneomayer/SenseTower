using MediatR;
using SC.SenseTower.Admin.Dto.Identity;

namespace SC.SenseTower.Admin.Cqrs.Logon
{
    public class LogonCommand : IRequest<LogonResponseDto>
    {
        public string? UserName { get; set; }

        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
