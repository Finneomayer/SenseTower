using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Accounts.Constants;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Dto.Identity;
using SC.SenseTower.Accounts.Models.Identity;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Exceptions;

namespace SC.SenseTower.Accounts.Controllers
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IdentityController : BaseMvcController
    {
        public IdentityController(
            IMediator mediator,
            ILogger<IdentityController> logger,
            IConfiguration configuration) : base(mediator, logger, configuration)
        {
        }

        [HttpGet, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult RegisterUser()
        {
            return View(new RegisterUserViewModel
            {
                Method = "post",
                Url = StaticRootUrl + ApiConstants.API_ROOT_SEGMENT + "identity/registeruser",
                ButtonText = "Ввести"
            });
        }

        [HttpPost, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model, CancellationToken cancellationToken)
        {
            model.Method = "post";
            model.Url = StaticRootUrl + ApiConstants.API_ROOT_SEGMENT + "identity/registeruser";
            model.ButtonText = "Ввести";
            if (ModelState.IsValid)
            {
                var registerMethod = await Mediator.Send(new RegistrationMethodRequest { Code = model.Code }, cancellationToken);
                if (!string.IsNullOrEmpty(registerMethod))
                {
                    switch (registerMethod)
                    {
                        case "ticket":
                            model.Method = "get";
                            model.Url = StaticRootUrl + ApiConstants.API_ROOT_SEGMENT + "identity/registerbyticket";
                            model.ButtonText = "Регистрация по гостевому приглашению";
                            break;
                        case "invite":
                            model.Method = "get";
                            model.Url = StaticRootUrl + ApiConstants.API_ROOT_SEGMENT + "identity/register";
                            model.ButtonText = "Регистрация по постоянному приглашению";
                            break;
                        default:
                            ModelState.AddModelError("", "Ошибка при определении метода регистрации. Обратитесь в службу техподдержки.");
                            break;
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неверный код билета / приглашения");
                }
            }

            return View(model);
        }

        [HttpGet, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Register(string? code)
        {
            var model = new RegisterByInviteViewModel
            {
                InviteId = code ?? string.Empty,
                FromUI = true
            };
            return View(model);
        }

        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        /// <param name="model">Регистрационные данные пользователя.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Информация о новом пользователе.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает информацию о новом пользователе или страницу результата, если регистрация производилась из браузера.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException, AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] RegisterByInviteViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var command = new RegisterUserCommand
                {
                    Email = model.Email,
                    Login = model.Login,
                    Password = model.Password,
                    InviteId = model.InviteId,
                    Wallets = model.Wallets
                };
                if (model.FromUI)
                {
                    try
                    {
                        await Mediator.Send(command, cancellationToken);
                        ViewBag.Message = "Вы успешно зарегистрировались!";
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
                else
                {
                    return Ok(await Mediator.Send(command, cancellationToken));
                }
            }
            return View(model);
        }

        [HttpGet, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult RegisterByTicket(string? code)
        {
            var model = new RegisterByTicketViewModel
            {
                TicketId = code ?? string.Empty
            };
            return View(model);
        }

        [HttpPost, CommonException, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ByTicket([FromForm] RegisterByTicketViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var command = new RegisterUserByTicketCommand
                {
                    Email = model.Email,
                    Login = model.Login,
                    Password = model.Password,
                    TicketId = model.TicketId
                };
                try
                {
                    await Mediator.Send(command, cancellationToken);
                    ViewBag.Message = "Вы успешно зарегистрировались!";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View("RegisterByTicket", model);
        }

        [HttpGet, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Logon()
        {
            var model = new LogonViewModel();
            return View(model);
        }

        /// <summary>
        /// Вход пользователя в приложение.
        /// </summary>
        /// <param name="command">Идентификационные данные пользователя.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Информация о пользователе.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает информацию о пользователе.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException, AllowAnonymous]
        public async Task<ActionResult<LogonResultDto>> Logon(LogonCommand command, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Вход пользователя в приложение.
        /// </summary>
        /// <param name="command">Идентификационные данные пользователя.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Информация о пользователе.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает информацию о пользователе.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException, AllowAnonymous]
        public async Task<ActionResult<LogonResultDto>> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new LogonCommand
            {
                Login = command.Login,
                Password = command.Password
            }, cancellationToken));
        }

        /// <summary>
        /// Удаление аккаунта пользователя.
        /// </summary>
        /// <param name="command">Идентификатор аккаунта пользователя.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Признак успешного завершения операции.</returns>
        /// <remarks>Возвращает признак успешного выполнения операции.</remarks>
        /// <response code="200">True, если операция была выполнена успешно.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException, Authorize(Roles = RoleNames.VR_ADMIN)]
        public async Task<ActionResult<bool>> Delete(DeleteAccountCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Вывод формы подтверждения регистрационного email.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="code">Токен подтверждения.</param>
        /// <returns>Веб-форма.</returns>
        [HttpGet, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ConfirmEmail(Guid userId, string code)
        {
            ViewBag.UserId = userId;
            ViewBag.Code = code;
            return View();
        }

        /// <summary>
        /// Подтверждение регистрационного email и вывод формы с результатами.
        /// </summary>
        /// <param name="command">Данные для подтверждения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Веб-форма с результатом выполнения операции подтверждения email.</returns>
        /// <remarks></remarks>
        [HttpPost, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            return View("EmailConfirmed", await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Вывод формы запроса на восстановление пароля.
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ResetRequest()
        {
            return View(new ResetRequestViewModel());
        }

        /// <summary>
        /// Отправка email для восстановления пароля.
        /// </summary>
        /// <param name="model">Логин или email пользователя.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <exception cref="ScException"></exception>
        [HttpPost, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ResetRequest(ResetRequestViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await Mediator.Send(new ResetRequestCommand { LoginOrEmail = model.LoginOrEmail ?? string.Empty }, cancellationToken))
                    {
                        throw new ScException("Ошибка отправки email для сброса пароля, попробуйте выполнить операцию позже. В случае повторной ошибки обратитесь в службу техподдержки.");
                    }
                    return View("ResetRequestSent");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Вывод формы изменения пароля.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, меняющего пароль.</param>
        /// <param name="code">Токен сброса пароля.</param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ResetPassword(Guid userId, string code)
        {
            return View(new ResetPasswordViewModel { UserId = userId, Token = code });
        }

        /// <summary>
        /// Изменение пароля.
        /// </summary>
        /// <param name="model">Данные для смены пароля.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
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

        /// <summary>
        /// Получение токена приложения-клиента.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает токены клиента.</returns>
        [HttpPost, AllowAnonymous, CommonException]
        public async Task<ActionResult<ClientLogonResultDto>> ClientLogon(ClientLogonRequest request, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        /// <summary>
        /// Проверка токена доступа и связанного с ним пользователя.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает результаты проверки.</returns>
        [HttpPost, AllowAnonymous, CommonException]
        public async Task<ActionResult<CheckIdentityResultDto>> Check(CheckIdentityRequest request, CancellationToken cancellationToken)
        {
            request.AccessToken = GetToken();
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        /// <summary>
        /// Обновление токена доступа пользователя.
        /// </summary>
        /// <param name="request">Токен обновления.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает токены пользователя.</returns>
        [HttpPost, AllowAnonymous, CommonException]
        public async Task<ActionResult<RefreshUserTokenResultDto>> Refresh(RefreshUserTokenRequest request, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        #region Unity

        /// <summary>
        /// Запрос выбора способа регистрации пользователя (Unity).
        /// </summary>
        /// <param name="code">Код приглашения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Строковое обозначение метода (invite или ticket).</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="204">Код не найден в базе или недействителен.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException, AllowAnonymous]
        public async Task<ActionResult<string>> RegistrationMethod(string code, CancellationToken cancellationToken)
        {
            var request = new RegistrationMethodRequest { Code = code };
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        /// <summary>
        /// Регистрация пользователя по приглашению (Unity).
        /// </summary>
        /// <param name="command">Параметры регистрации.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Информация о новом пользователе.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException, AllowAnonymous]
        public async Task<ActionResult<LogonResultDto>> RegisterByInvite([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Регистрация пользователя по гостевому приглашению (Unity).
        /// </summary>
        /// <param name="command">Параметры регистрации.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Информация о новом пользователе.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException, AllowAnonymous]
        public async Task<ActionResult<LogonResultDto>> RegisterByTicket([FromBody] RegisterUserByTicketCommand command, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Запрос сброса пароля (Unity).
        /// </summary>
        /// <param name="command">Параметры запроса.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut, CommonException, AllowAnonymous]
        public async Task<ActionResult<bool>> ResetRequest([FromBody] ResetRequestCommand command, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        #endregion
    }
}
