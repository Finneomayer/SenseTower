using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.Logout
{
    public class LogoutCommandHandler : BaseHandler, IRequestHandler<LogoutCommand, Unit>
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        public LogoutCommandHandler(
            ILogger<LogoutCommandHandler> logger,
            IMapper mapper,
            SignInManager<ApplicationUser> signInManager) : base(logger, mapper)
        {
            this.signInManager = signInManager;
        }

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await signInManager.SignOutAsync();
            return Unit.Value;
        }
    }
}
