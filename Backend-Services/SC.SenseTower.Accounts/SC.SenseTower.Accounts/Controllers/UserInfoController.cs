using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Accounts.Constants;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Dto.UserInfo;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Accounts.Controllers
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserInfoController : BaseController
    {
        public UserInfoController(IMediator mediator, ILogger<UserInfoController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Проверка имени входа на возможность использования.
        /// </summary>
        /// <param name="login">Имя входа.</param>
        /// <returns>True, если имя ещё не занято, и его можно использовать.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает результат проверки.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException, AllowAnonymous]
        public async Task<ActionResult<bool>> IsLoginFree(string login)
        {
            return Ok(await Mediator.Send(new CheckLoginRequest { Login = login }));
        }

        /// <summary>
        /// Проверка регистрационной почты на возможность использования.
        /// </summary>
        /// <param name="email">Регистрационная почта.</param>
        /// <returns>True, если почту можно использовать.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает результат проверки.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException, AllowAnonymous]
        public async Task<ActionResult<bool>> IsEmailFree(string email)
        {
            return Ok(await Mediator.Send(new CheckEmailRequest { Email = email }));
        }

        /// <summary>
        /// Получение информации о текущем пользователе.
        /// </summary>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Информация о пользователе.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает информацию о пользователе.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<UserInfoDto?>> GetInfo(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetCurrentUserInfoRequest { Token = GetToken() }, cancellationToken));
        }

        /// <summary>
        /// Получение информации о пользователе по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Информация о пользователе.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает информацию о пользователе.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}"), CommonException]
        public async Task<ActionResult<UserInfoDto?>> Get(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetUserInfoRequest
            {
                AccessToken = GetToken(),
                UserId = id
            }, cancellationToken));
        }

        /// <summary>
        /// Получение списка пользователей по их идентификаторам.
        /// </summary>
        /// <param name="request">Массив идентификаторов пользователей. Если null, то все пользователи.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Массив имен пользователей.</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<LookupItemDto<Guid>[]?>> Lookup(LookupUsersRequest request, CancellationToken cancellationToken)
        {
            request.AccessToken = GetToken();
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        /// <summary>
        /// Установка/сброс аватара пользователя.
        /// </summary>
        /// <param name="command">Параметры задания аватара.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Признак успешного завершения операции.</returns>
        /// <remarks></remarks>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<bool>> SetAvatar(SetAvatarCommand command, CancellationToken cancellationToken)
        {
            command.CurrentUserId = GetUserId();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Получение списка пользователей по их идентификаторам.
        /// </summary>
        /// <param name="userIds">Массив идентификаторов пользователей.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Список пользователей.</returns>
        /// <remarks></remarks>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetByIds(Guid[] userIds, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new UsersByIdsRequest { UserIds = userIds, AccessToken = GetToken() }, cancellationToken));
        }
    }
}
