using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SC.SenseTower.Auth.Constants;
using SC.SenseTower.Auth.Extensions;
using SC.SenseTower.Auth.Models;
using SC.SenseTower.Auth.Services.EmailSender;
using SC.SenseTower.Auth.Services.RabbitMQ;
using SC.SenseTower.Auth.Settings;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Web;

namespace SC.SenseTower.Auth.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OperationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ServiceEndpointsSettings _endpoints;
        private readonly EmailSenderService _emailSender;
        private readonly RabbitMQService _rabbitMQService;

        public OperationsController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<ServiceEndpointsSettings> options,
            EmailSenderService emailSenderService,
            RabbitMQService rabbitMQService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _endpoints = options.Value;
            _emailSender = emailSenderService;
            _rabbitMQService = rabbitMQService;
        }

        [HttpGet, AllowAnonymous]
        public ViewResult Create() => View();

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.Name,
                    Email = user.Email
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
                appUser.SecurityStamp = Guid.NewGuid().ToString();

                //Adding User to Admin Role
                await _userManager.AddToRoleAsync(appUser, RoleNames.VR_USER);

                if (result.Succeeded)
                    ViewBag.Message = "User Created Successfully";
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult CreateRole() => View();

        [HttpPost]
        public async Task<IActionResult> CreateRole([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = name });
                if (result.Succeeded)
                    ViewBag.Message = "Role Created Successfully";
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Login,
                    AccessGrantedTo = model.AccessGrantedTo
                };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return BadRequest(createResult.GetMessages());

                user = await _userManager.FindByNameAsync(model.Login);
            }
            else
            {
                if (user.AccessGrantedTo == null)
                    return BadRequest("Пользователь с таким email уже зарегистрирован");

                user.AccessGrantedTo = model.AccessGrantedTo;
                user.UserName = model.Login;
                await _userManager.UpdateAsync(user);
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.Password);
            if (!addPasswordResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(addPasswordResult.GetMessages());
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, RoleNames.VR_USER);
            if (!addToRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(addToRoleResult.GetMessages());
            }

            if (model.AccessGrantedTo == null)
            {
                await SendConfirmationEmail(user, cancellationToken);
            }

            return Ok(user.Id);
        }

        private async Task SendConfirmationEmail(ApplicationUser user, CancellationToken cancellationToken)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = $"{_endpoints.AccountsRootUrl}{_endpoints.ConfirmEmailUrl}?userId={user.Id}&code={HttpUtility.UrlEncode(token)}";
            var text = $@"Здравствуйте, {user.UserName}!<br/>
<br/>
Вы зарегистрировались в метавселенной <b>SenseTower</b>.<br/>
Подтвердите ваш регистрационный email <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>по ссылке</a>.<br/>
<br/>
С уважением<br/>
Команда SenseTower.";
            await _emailSender.SendEmailConfirmation(user.Email, user.UserName, "Подтверждение регистрационного email", text, cancellationToken);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(Guid userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return BadRequest("Пользователь не найден.");
            var identityResult = await _userManager.ConfirmEmailAsync(user, code);
            if (!identityResult.Succeeded)
                return BadRequest(string.Join("\n", identityResult.Errors.Select(r => r.Description)));
            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
        {
            var callerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != callerId)
            {
                var caller = await _userManager.FindByIdAsync(callerId.ToString());
                if (!await _userManager.IsInRoleAsync(caller, RoleNames.VR_ADMIN))
                    return BadRequest("Недостаточно прав для удаления пользователя.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return BadRequest("Пользователь не найден.");

            var identityResult = await _userManager.DeleteAsync(user);
            if (!identityResult.Succeeded)
                return BadRequest(identityResult.GetMessages());

            await _rabbitMQService.Send(RabbitMQConstants.DEFAULT_EXCHANGE, $"{RabbitMQConstants.RKEY_IDENTITY}.{RabbitMQConstants.RKEY_USERS}.{RabbitMQConstants.RKEY_DELETE}", new { userId }, cancellationToken);

            return Ok(true);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> SendResetPassword(string loginOrEmail, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(loginOrEmail);
            if (user == null && loginOrEmail.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(loginOrEmail);
            }
            if (user == null)
            {
                return BadRequest("По указанным учётным данным пользователь не найден.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{_endpoints.AccountsRootUrl}{_endpoints.ResetPasswordUrl}?userId={user.Id}&code={HttpUtility.UrlEncode(token)}";
            var text = $@"Здравствуйте, {user.UserName}!<br/>
<br/>
Это письмо отправлено вам из приложения <b>SenseTower</b> потому, что вы забыли пароль от вашего аккаунта.<br/>
Пройдите <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>по ссылке</a>, чтобы создать новый пароль.<br/>
Если вы не запрашивали восстановление пароля, то зайдите в настройки своего личного кабинета и смените пароль, так как, возможно, кто-то пытается получить доступ к вашему аккаунту в <b>SenseTower</b>.<br/>
<br/>
С уважением<br/>
Команда SenseTower.";
            await _emailSender.SendEmailConfirmation(user.Email, user.UserName, "Восстановление пароля", text, cancellationToken);

            var result = new SendResetPasswordResultDto
            {
                Token = token,
                UserId = user.Id
            };
            return Ok(result);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> SetPassword(Guid userId, string password, string code, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return BadRequest("Пользователь не найден");
            if (!await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", code))
                return BadRequest("Невалидный токен сброса пароля");

            var results = new List<string>();
            foreach (var validator in _userManager.PasswordValidators)
            {
                var validationResult = await validator.ValidateAsync(_userManager, user, password);
                if (!validationResult.Succeeded)
                    results.Add(validationResult.GetMessages());
            }
            if (results.Count > 0)
                return BadRequest(string.Join("\n", results));

            var resetResult = await _userManager.ResetPasswordAsync(user, code, password);
            if (!resetResult.Succeeded)
                return BadRequest(resetResult.GetMessages());

            return Ok(true);
        }
    }
}
