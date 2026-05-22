using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Services.EmailSender;
using System.Text.Encodings.Web;
using System.Web;

namespace SC.SenseTower.Admin.Cqrs.ResetRequest
{
    public class ResetRequestCommandHandler : BaseHandler, IRequestHandler<ResetRequestCommand, Unit>
    {
        private readonly IdentityService identityService;
        private readonly EmailSenderService emailSender;
        private readonly UserManager<ApplicationUser> userManager;

        public ResetRequestCommandHandler(
            ILogger<ResetRequestCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService,
            EmailSenderService emailSenderService,
            UserManager<ApplicationUser> userManager) : base(logger, mapper)
        {
            this.identityService = identityService;
            emailSender = emailSenderService;
            this.userManager = userManager;
        }

        public async Task<Unit> Handle(ResetRequestCommand request, CancellationToken cancellationToken)
        {
            var user = await identityService.GetUserByLoginOrEmail(request.LoginOrEmail ?? string.Empty, cancellationToken);
            if (user == null)
                throw new ScException("Пользователь не найден.");
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{request.CallbackUrl}/account/resetpassword?userId={user.Id}&code={HttpUtility.UrlEncode(token)}";
            var text = $@"Здравствуйте, {user.UserName}!<br/>
<br/>
Это письмо отправлено вам из приложения администрирования <b>SenseTower</b> потому, что вы забыли пароль от вашего аккаунта.<br/>
Пройдите <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>по ссылке</a>, чтобы создать новый пароль.<br/>
Если вы не запрашивали восстановление пароля, то зайдите в настройки своего личного кабинета и смените пароль, так как, возможно, кто-то пытается получить доступ к вашему аккаунту.<br/>
<br/>
С уважением<br/>
Команда SenseTower.";
            await emailSender.SendEmail(user.Email, "Восстановление пароля", text, cancellationToken);
            return Unit.Value;
        }
    }
}
