using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Admin.Dto.Identity;
using SC.SenseTower.Admin.Exceptions;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.Logon
{
    public class LogonCommandHandler : BaseHandler, IRequestHandler<LogonCommand, LogonResponseDto>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AccountsHttpService accountsHttpService;

        public LogonCommandHandler(
            ILogger<LogonCommandHandler> logger,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AccountsHttpService accountsHttpService) : base(logger, mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.accountsHttpService = accountsHttpService;
        }

        public async Task<LogonResponseDto> Handle(LogonCommand request, CancellationToken cancellationToken)
        {
            var result = await accountsHttpService.Logon(request.UserName, request.Password, cancellationToken);
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user == null)
                throw new Exception("Неверные учётные данные.");
            if (!await userManager.IsInRoleAsync(user, RoleNames.VR_ADMIN))
                throw new NotAdminException("Для доступа к приложению нужны права администратора.");

            var signInResult = await signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    throw new Exception("Аккаунт заблокирован.");
                throw new Exception("Неверные учётные данные.");
            }

            return result ?? new LogonResponseDto();
        }
    }
}
