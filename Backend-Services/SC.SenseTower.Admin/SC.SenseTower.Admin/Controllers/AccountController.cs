using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Admin.Cqrs.Logon;
using SC.SenseTower.Admin.Cqrs.Logout;
using SC.SenseTower.Admin.Cqrs.ResetPassword;
using SC.SenseTower.Admin.Cqrs.ResetRequest;
using SC.SenseTower.Admin.Models.Account;

namespace SC.SenseTower.Admin.Controllers
{
    [Authorize]
    public class AccountController : BaseMvcController
    {
        protected override string ActiveMenuItem => "";

        public AccountController(
            IMediator Mediator,
            ILogger<AccountController> logger,
            IConfiguration configuration) : base(Mediator, logger, configuration)
        {
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Logon(string? returnUrl)
        {
            ViewBag.StaticRootUrl = StaticRootUrl;
            return View(new LogonViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Logon(LogonViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var command = new LogonCommand
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    RememberMe = model.RememberMe
                };
                try
                {
                    var result = await Mediator.Send(command, cancellationToken);
                    HttpContext.Response.Cookies.Append("SC.SenseTower.Admin.Token", result?.AccessToken ?? string.Empty);
                    HttpContext.Response.Cookies.Append("SC.SenseTower.Admin.Refresh", result?.RefeshToken ?? string.Empty);
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && model.ReturnUrl.First() == '/')
                        model.ReturnUrl = model.ReturnUrl[1..];
                    return Redirect(StaticRootUrl + (string.IsNullOrEmpty(model.ReturnUrl) ? "home/" : model.ReturnUrl));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            await Mediator.Send(new LogoutCommand(), cancellationToken);
            return Redirect(StaticRootUrl + "account/logon?returnUrl=%2F");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ResetRequest()
        {
            return View(new ResetRequestViewModel());
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ResetRequest(ResetRequestViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var callbackUrl = HttpContext.Request.PathBase.ToString();
                try
                {
                    await Mediator.Send(new ResetRequestCommand { LoginOrEmail = model.LoginOrEmail, CallbackUrl = callbackUrl }, cancellationToken);
                    return View("ResetRequestSent");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ResetPassword(Guid userId, string code)
        {
            return View(new ResetPasswordViewModel { UserId = userId, Token = code });
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return View("PasswordChanged", await Mediator.Send(new ResetPasswordCommand { Password = model.Password, Token = model.Token, UserId = model.UserId }, cancellationToken));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult AccessDenied(string? returnUrl)
        {
            return View();
        }
    }
}
